# Integration with .NET

Even though RainLisp is closer to a general purpose language (in principle and not in terms of feature richness),
the core goal of its initial design was to use it like a DSL (Domain Specific Language) and integrate it with existing
.NET systems.

Imagine cases where parts of your computations change frequently in an ad hoc fashion and vary largely between clients.
Your .NET system can specify an overall computational infrastructure and call RainLisp code for smaller configurable parts.

> Currenly .NET code can call RainLisp but not the other way round which is a future plan.

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

The above code can be stored in a database or some other configuration medium. In the code below, assume it is loaded in `RAIN_LISP_CODE`.
Then the .NET system can evaluate the above code as follows.

```csharp
using RainLisp;
using RainLisp.Evaluation.Results;

var interpreter = new Interpreter();

var result = interpreter.Evaluate(RAIN_LISP_CODE).Last();
string logFileName = ((StringDatum)result).Value;

Console.WriteLine($"Calculated log file name: {logFileName}.");
```

Notice that the last evaluation result is taken into consideration which is expected to be a `StringDatum`, which effectively reflects
the programming contract between the two systems.

> Note that exception handling is omitted for brevity.

## Consecutive Calls

Now let's suppose there is a RainLisp procedure that gives a ratio varying between calendar months.
It specifies a `12.42` ratio for January, `31.71` for February and `9.32` for every other month.

```scheme
(define (get-monthly-ratio month)
    (cond ((= month 1) 12.42)
          ((= month 2) 31.71)
          (else 9.32)))
```

That custom ratio is then consumed by a .NET system as part of a more general calculation algorithm.
Below, it is demonstrated how one can additively make calls to RainLisp, each building on the previous one in an
additive fashion. In simple words, the first call will "install" or create if you will the `get-monthly-ratio` procedure
and the second one will call it with an appropriate value.

Once again, assume that the above code is loaded into `RAIN_LISP_CODE`.

```csharp
using RainLisp;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

var interpreter = new Interpreter();

IEvaluationEnvironment? environment = null;
_ = interpreter.Evaluate(RAIN_LISP_CODE, ref environment)
    .Last();

var result = interpreter.Evaluate("(get-monthly-ratio 2)", ref environment)
    .Last();

double ratio = ((NumberDatum)result).Value;

Console.WriteLine($"The calculation ratio for February is: {ratio}.");
```

In order for one evaluation to take into consideration and
build onto another, a common `IEvaluationEnvironment` is used.

> `Evaluate` returns an `IEnumerable<EvaluationResult>`. Therefore, notice that the Linq `Last` method is called to force the enumeration
and therefore actually evaluate the code. If this is not done, `get-monthly-ratio` will never actually be created.

The second time `Evaluate` is called, the existing `environment` is used so that the procedure `get-monthly-ratio` exists and we don't
start from scratch. The ratio for February is asked `(get-monthly-ratio 2)`. 

> Once again, the `Last` method is used to force the evaluation, even though other Linq techniques can apply. For this particular example,
`First` could also work since `(get-monthly-ratio 2)` is the only call that is made.

In this example, the contract is that the RainLisp code implements a procedure called `get-monthly-ratio` which takes a month number
as a parameter and returns a numeric ratio.

> You could get away of using consecutive calls by adopting other techniques like combining the `get-monthly-ratio` definition and call in the
same code and use string interpolation for example to inject the month number. Be careful if you use this technique with strings though, to avoid
a possible malicious code injection.

## Improving Performance

The two `Evaluate` method flavors we have seen so far, have respective overloads accepting a `RainLisp.AbstractSyntaxTree.Program` instance
which is an abstract syntax tree instead of the code as a `string`. When you know that the RainLisp code is unlikely to change, you can
use these calls to speed up the evaluation.

For example, you can cache the result of the lexical and grammar syntax analysis and skip these steps in consecutive calls by calling the
aforementioned overloads. If your code is simple, you can even always skip the analysis phases by specifying an abstract syntax tree directly,
effectively treating code as data.

Let's see this in action.

```csharp
using RainLisp;
using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
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
_ = interpreter.Evaluate(program, ref environment)
    .Last();

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

var result = interpreter.Evaluate(procedureCallProgram, ref environment)
    .Last();

double ratio = ((NumberDatum)result).Value;

Console.WriteLine($"The calculation ratio for February is: {ratio}.");
```

## Retrieving Multiple Values

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

(define payroll (calculate-payroll 3000 false))
```


## Implementing a REPL

`IInterpreter` specifies two methods that are useful in case you want to implement a REPL (Readl Eval Print Loop) or an editor and evalution
program around RainLisp. These are `ReadEvalPrintLoop` and `EvaluateAndPrint`. You can have a look at how [RainLispConsole](https://github.com/chr1st0scli/RainLispConsole)
takes advantage of these methods. Of course, you might discover other use cases yourself.