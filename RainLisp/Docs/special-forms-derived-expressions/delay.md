# delay
A special form that delays the evaluation of the given expression.
The evaluation result is a procedure which evaluates the expression when called.
```
(delay expression)
```

## Examples
```scheme
; Delay displaying "Hello World!".
(delay (display "Hello World!"))
```
-> *[UserProcedure] Parameters: 0*

```scheme
; Assign the procedure to a variable and call it later.
(define delayed (delay (display "Hello World!")))
(delayed)
```
-> *Hello World!*

```scheme
; Use the force library procedure to force the evaluation.
(force (delay (display "Hello World!")))
```
-> *Hello World!*

## Remarks

> `(delay a)` is equivalent to `(lambda() a)`. So, it needs to be a special form; otherwise `(delay a)`
would cause `a` to be evaluated before calling `delay` on it, due to the language's applicative order of evaluation.

> Note that the procedure created for the delayed expression is a memoized one. I.e. the expression is evaluated once
when needed and subsequent calls will simply return the previously calculated value.