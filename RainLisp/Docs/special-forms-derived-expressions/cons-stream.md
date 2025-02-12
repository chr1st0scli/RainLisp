# cons-stream
A derived expression for constructing a stream, i.e. a pair of an immediately evaluated expression and a delayed one.
```
(cons-stream expression expression)
```

The first expression is evaluated immediately and the second one is delayed. The result of the `cons-stream` expression
is a pair made of the value of the first expression and a procedure which evaluates the second one when called, i.e. a promise.

## Examples
```scheme
(cons-stream 1 2)
```
-> *(1 . [UserProcedure] Parameters: 0)*

```scheme
; Assign the procedure to a variable and call it later.
(define delayed (cdr (cons-stream 1 2)))
(delayed)
```
-> *2*

```scheme
; Use the force library procedure to force the evaluation.
(force (cdr (cons-stream 1 2)))
```
-> *2*

```scheme
; Use the cdr-stream library procedure to force the evaluation.
(cdr-stream (cons-stream 1 2))
```
-> *2*

## Remarks
> Note that `(cons-stream a b)` is syntactic sugar for `(cons a (delay b))`.

> Note that the procedure created for the delayed expression is a memoized one. I.e. the expression is evaluated once
when needed and subsequent calls will simply return the previously calculated value.