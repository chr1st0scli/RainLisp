# Integration with .NET

Even though RainLisp is closer to a general purpose language (in principle and not in terms of feature richness),
the core goal of its initial design was to use it like a DSL (Domain Specific Language) and integrate it with existing
.NET systems.

Imagine cases where parts of your computations change frequently in an ad hoc fashion and vary largely between clients.
Your .NET system can specify an overall computational infrastructure and call RainLisp code for smaller configurable parts.

> Currently, .NET code can call RainLisp but not the other way round, which is a future plan.

> The .NET examples below are written in C# but any .NET language can be used.

## Evaluating RainLisp Expressions
`IInterpreter` defines `Evaluate` method overloads for evaluating RainLisp expressions, i.e. a program. These methods typically return an `IEnumerable<EvaluationResult>`
and the actual evaluation of each RainLisp expression occurs on a per request basis, i.e. while the enumeration is enumerated.

```csharp
using RainLisp;
using RainLisp.DotNetIntegration;

const string RAIN_LISP_CODE = @"
(+ 1 1)
(- 10 2)
(* 4 7)
9";

var interpreter = new Interpreter();
var results = interpreter.Evaluate(RAIN_LISP_CODE);

foreach (var result in results)
{
    Console.WriteLine(result.Number());
}
```

`RAIN_LISP_CODE` above contains the RainLisp expressions, each on a new line. Each expression is evaluated at runtime while `results` are enumerated in the `foreach` loop.

> Note that all subsequent examples use the `Execute` overloads which mirror the `Evaluate` ones. `Execute` methods evaluate all RainLisp expressions at once and
return the `EvaluationResult` of the last one.

## One-off Call

Let's suppose there is a custom RainLisp code that specifies a log file's name for a .NET system.
It first prints a message to the standard output and finally returns the log file name by concatenating some strings.
Conventionally, the RainLisp coder knows that the last thing they should return is a string.

```scheme
(define dt (utc-now))
(display "Generating log file name at ")
(display dt)
(newline)
(+ "system-" (datetime-to-string dt "yyyy-MM-dd_HH-mm-ss-fff") ".log")
```

The above code can be stored in a database or some other configuration medium. For the C# example below, assume that the above code is loaded in `RAIN_LISP_CODE`.
Then the .NET system can evaluate it as follows.

```csharp
using RainLisp;
using RainLisp.DotNetIntegration;

var interpreter = new Interpreter();

var result = interpreter.Execute(RAIN_LISP_CODE);
string logFileName = result.String();

Console.WriteLine($"Calculated log file name: {logFileName}.");
```

Notice that only the last evaluation result is taken into consideration, which is expected to be a `string`. This effectively reflects
the programming contract between the two systems.

> Note that exception handling in the C# example is omitted for brevity.

## Consecutive Calls

Now, let's suppose there is a RainLisp procedure that gives a ratio varying between calendar months.
It specifies a `12.42` ratio for January, `31.71` for February and `9.32` for every other month.

```scheme
(define (get-monthly-ratio month)
    (cond ((= month 1) 12.42)
          ((= month 2) 31.71)
          (else 9.32)))
```

That custom ratio is then consumed by a .NET system as part of a more general calculation algorithm.
Below, it is demonstrated how one can make consecutive calls to RainLisp, each building on the previous one in an
additive manner. In simple words, the first call will create the `get-monthly-ratio` procedure
and the second one will call it with an appropriate value.

Once again, assume that the above code is loaded into `RAIN_LISP_CODE`.

```csharp
using RainLisp;
using RainLisp.DotNetIntegration;
using RainLisp.Evaluation;

var interpreter = new Interpreter();

// Create the procedure.
IEvaluationEnvironment? environment = null;
_ = interpreter.Execute(RAIN_LISP_CODE, ref environment);

// Call it for month February.
var result = interpreter.Execute("(get-monthly-ratio 2)", ref environment);

double ratio = result.Number();

// Write the result to the standard output.
Console.WriteLine($"The calculation ratio for February is: {ratio}.");
```

In order for one evaluation to take into consideration and build onto another, a common `IEvaluationEnvironment` is used.

The first call to `Execute` creates the `get-monthly-ratio` procedure.

The second time `Execute` is called the February's ratio is asked `(get-monthly-ratio 2)`. Note that the existing `environment` is used so that
the previous evaluation, the procedure creation, is still in effect.

In this example, the contract is that the RainLisp code implements a procedure called `get-monthly-ratio` which takes a month number
as a parameter and returns a numeric ratio.

> You could get away of using consecutive calls by adopting other techniques like combining the `get-monthly-ratio` definition and call in the
same code and use string interpolation for example to inject the month number. Be careful if you use this technique with strings though, to avoid
a possible malicious code injection.

### Improving Performance

The two `Execute` method flavors we have seen so far, have respective overloads accepting a `RainLisp.AbstractSyntaxTree.Program` instance,
which is an abstract syntax tree, instead of the code as a `string`. When you know that the RainLisp code is unlikely to change, you can
use these calls to speed up the evaluation. For example, you can cache the result of the lexical and grammar syntax analysis and skip these steps
in consecutive calls by calling the aforementioned overloads. If your code is simple, you can even always skip the analysis phases by specifying an
abstract syntax tree directly, effectively treating code as data.

Let's see this in action.

```csharp
using RainLisp;
using RainLisp.AbstractSyntaxTree;
using RainLisp.DotNetIntegration;
using RainLisp.Evaluation;
using RainLisp.Parsing;
using RainLisp.Tokenization;

// Suppose this is retrieved from a database or another suitable storage.
// It creates the RainLisp get-monthly-ratio procedure.
const string RAIN_LISP_CODE = @"
(define (get-monthly-ratio month)
    (cond ((= month 1) 12.42)
          ((= month 2) 31.71)
          (else 9.32)))
";

// Perform lexical analysis.
var tokenizer = new Tokenizer();
var tokens = tokenizer.Tokenize(RAIN_LISP_CODE);

// Perform syntax analysis.
var parser = new Parser();
// program is the abstract syntax tree which can be cached. The tokenization and parsing don't need to be repeated again to create the get-monthly-ratio procedure.
var program = parser.Parse(tokens);

// Evaluate to create get-monthly-ratio and keep it in the given environment.
var interpreter = new Interpreter();
IEvaluationEnvironment? environment = null;
// We evaluate the abstract syntax tree instead of source code.
_ = interpreter.Execute(program, ref environment);

// For the actual call to get-monthly-ratio, we are taking a different approach.
// Since it is just a simple procedure call (application), we build the abstract syntax tree ourselves, effectively treating code as data.
var procedureCallProgram = new RainLisp.AbstractSyntaxTree.Program
{
    DefinitionsAndExpressions = new List<Node>
    {
        // Call to get-monthly-ratio with an argument of 2 which is February.
        new Application(new Identifier("get-monthly-ratio"), new List<Expression> { new NumberLiteral(2) })
    }
};

// We evaluate the manually built abstract syntax tree
var result = interpreter.Execute(procedureCallProgram, ref environment);

double ratio = result.Number();

// Write the result to the standard output.
Console.WriteLine($"The calculation ratio for February is: {ratio}.");
```

## Retrieving Multiple Values

Let's see a more realistic example for businesses. Let's suppose that there is a custom RainLisp code that defines
the payroll calculation for an employee.

The code below defines some helper procedures (setters & getters) and finally the procedure that performs the payroll
calculation and analysis in its constituent parts.

```scheme
; Constructs a data structure with payroll analysis data.
(define (make-payroll-analysis tax insurance net-income married pay-date)
    (list (cons 'tax tax)
          (cons 'insurance insurance)
          (cons 'net-income net-income)
          (cons 'married married)
          (cons 'pay-date pay-date)))

; Getters of individual payroll analysis data.
(define (get-tax payroll-analysis)
    (cdar payroll-analysis))

(define (get-insurance payroll-analysis)
    (cdadr payroll-analysis))

(define (get-net-income payroll-analysis)
    (cdaddr payroll-analysis))

(define (get-marital-status payroll-analysis)
    (cdaddr (cdr payroll-analysis)))

(define (get-paydate payroll-analysis)
    (cdaddr (cddr payroll-analysis)))

; Procedure that calculates and returns the payroll analysis based on the employee's monthly gross income and wether or not they are married.
(define (calculate-payroll monthly-gross-income married)

    ; Set the rates.
    (define tax-rate (if married 18.4 18.6))
    (define insurance-rate 13.9)
    (define net-rate (- 100 tax-rate insurance-rate))

    ; Calculate amounts.
    (define tax (round (* (/ tax-rate 100)
                          monthly-gross-income)
                       2))

    (define insurance (round (* (/ insurance-rate 100)
                                monthly-gross-income)
                             2))

    (define net (round (* (/ net-rate 100)
                          monthly-gross-income)
                       2))

    (define now (utc-now))
    ; Pay on the last day of the current month.
    (define pay-date (add-days
                        (add-months (make-date (year now) (month now) 1) 
                                    1)
                        -1))
    
    (make-payroll-analysis tax insurance net married pay-date))
```

Below, it is demonstrated how the .NET system could use the above code and access the results of the calculation.
Once again, assume the above code is stored in `RAIN_LISP_CODE`.

```csharp
using RainLisp;
using RainLisp.DotNetIntegration;
using RainLisp.Evaluation;

// Install the necessary RainLisp code for payroll calculation.
var interpreter = new Interpreter();
IEvaluationEnvironment? environment = null;
_ = interpreter.Execute(RAIN_LISP_CODE, ref environment);

// Calculate the payroll details for an unmarried employee that gets a 5000 gross income.
// For simplicity, we are using RainLisp code for the calls but we could build our own AST, as we have seen before.
_ = interpreter.Execute("(define payroll (calculate-payroll 5000 false))", ref environment);

// Get the calculated payroll details.
double tax = interpreter.Execute("(get-tax payroll)", ref environment).Number();
double insurance = interpreter.Execute("(get-insurance payroll)", ref environment).Number();
double netIncome = interpreter.Execute("(get-net-income payroll)", ref environment).Number();
bool isMarried = interpreter.Execute("(get-marital-status payroll)", ref environment).Bool();
DateTime payDate = interpreter.Execute("(get-paydate payroll)", ref environment).DateTime();

// Write them to the standard output.
Console.WriteLine($"Bob is {(!isMarried ? "not" : "")} married, pays {tax} tax, {insurance} insurance and is getting paid {netIncome} on {payDate}.");
```

## Implementing a REPL

`IInterpreter` specifies two methods that are useful in case you want to implement a REPL (Read Eval Print Loop) or an editor and evaluation
program for RainLisp. These are `ReadEvalPrintLoop` and `EvaluateAndPrint`, which are able to properly format any output. You can have a look at
how [RainLispConsole](https://github.com/chr1st0scli/RainLispConsole) takes advantage of these methods; of course, you might discover other use cases yourself.