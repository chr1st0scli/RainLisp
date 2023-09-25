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

## Retrieving Multiple Values

### Pair

### More Complex Structures

## Improving Performance

The two `Evaluate` method flavors we have seen so far, have respective overloads accepting a `RainLisp.AbstractSyntaxTree.Program` instance
which is an abstract syntax tree instead of the code as a `string`. When you know that the RainLisp code is unlikely to change, you can
use these calls to speed up the evaluation.

For example, you can cache the result of the lexical and grammar syntax analysis and skip these steps in consecutive calls by calling the
aforementioned overloads.

## Implementing a REPL