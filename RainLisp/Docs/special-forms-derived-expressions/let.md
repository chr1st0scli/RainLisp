# let
A derived expression for declaring variables and expressions using them, in a scope limited
within the `let` expression itself. It has one or more let clauses which define the variables,
followed by a body.
```
"(" "let" "(" let_clause+ ")" body ")"
```

A let clause starts with an `ID` being the name of the identifier, followed by an expression that gives its value.
```
let_clause = "(" ID expression ")"
```

The body consists of zero or more definitions, followed by at least one expression.
The expressions are evaluated in the order they appear and the evaluation result of the last one
is the final result of the `let` expression.
```
body = definition* expression+
```

## Examples
```scheme
; Declare a and b in a local scope and add them.
(let ((a 1) (b 2))
  (+ a b))
```
-> *3*

```scheme
; Define a procedure that calculates some formula based on parameter x.
(define (foo x)
  (let ((a (+ x 1))
        (b (+ x 2)))
    (define c (* a b))
    (+ x a b c)))

(foo 4) ; Call the procedure.
```
-> *45*

## Remarks

> Note that `let` is syntactic sugar for [lambda application](function-application.md).

I.e.

```scheme
(let ((a 1) (b 2))
  (+ a b))
```

is really a user procedure with two parameters `a` and `b`, being called with arguments `1` and `2`.

```scheme
((lambda (a b)
  (+ a b)) 1 2)
```