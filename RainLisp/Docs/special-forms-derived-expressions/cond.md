# cond
A derived expression for declaring many alternative expressions to be evaluated based on the result of different predicates.
This is typically known as an `if... else if... else...` expression.
It has one or more conditional clauses, the `(predicate expression . expressions)` part, followed by an optional conditional else clause in the end.
```
(cond (predicate expression . expressions) (else expression . expressions))
```

The first expression of a conditional clause is the predicate. If it evaluates to true, the rest of the expressions
will be evaluated in turn and the last one's result will be the final one. If the predicates of all conditional clauses evaluate to false, the
conditional else clause is evaluated. The expressions are once again evaluated in the order they appear and the last one's result is the final one.
If no conditional else clause is provided, the final result is unspecified.

## Examples
```scheme
; Define a variable.
(define x 0)
; Evaluate a condition.
(cond ((< x 0) "negative")
      ((= x 0) "zero")
      (else "positive"))
```
-> *"zero"*

```scheme
; Define a variable.
(define x 3)

; Print a description and a new line and return 0 for zero, -1 for a negative value and 1 for a positive one.
(cond ((< x 0) 
       (display "negative")
       (newline)
       -1)
      ((= x 0)
       (display "zero")
       (newline)
       0)
      (else 
       (display "positive")
       (newline)
       1))
```
->
```
positive
1
```

## Remarks
> Note that `cond` is syntactic sugar for a nested `if`.
