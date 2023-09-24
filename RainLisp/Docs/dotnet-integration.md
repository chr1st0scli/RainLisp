# Integration with .NET

Even though RainLisp is closer to a general purpose language (in principle and not in terms of feature richness),
the core goal of its initial design was to use it like a DSL (Domain Specific Language) and integrate it with existing
.NET systems.

Imagine cases where parts of your computations change frequently in an ad hoc fashion and largely vary between clients.
Your .NET system can specify an overall computational infrastructure and call RainLisp code for the small configurable parts.

> Currenly .NET code can call RainLisp but not the other way round which is a future plan.

## One-off Call

Let's suppose there is a custom RainLisp code that specifies the format of a log file's name for a .NET system.

```scheme
(define dt (utc-now))
(display "Generating log file name at ")
(display dt)
(newline)
(+ "system-" (datetime-to-string dt "yyyy-MM-dd_HH-mm-ss") ".log")
```

## Consecutive Calls

Now let's suppose there is a RainLisp procedure that gives a ratio varying between calendar months.
That custom ratio is then consumed by a .NET system as part of a more general calculation algorithm.

```scheme
(define (get-monthly-ratio month)
    (cond ((= month 1) 12.42)
          ((= month 2) 31.71)
          (else 9.32)))
```