# lambda
A special form for creating a user procedure with zero or more parameters. The body is executed when the procedure is called.
```
(lambda (. parameters) body)
```

The procedure's body consists of zero or more definitions, followed by at least one expression.
The expressions are evaluated in the order they appear and the evaluation result of the last one
is the final result of the procedure when called.

## Examples
```scheme
; Create a user procedure that returns the greater of two numbers.
(lambda (x y)
  (if (>= x y)
      x
      y))
```
-> *[UserProcedure] Parameters: x, y*

```scheme
; Create an anonymous user procedure, i.e. a lambda, that returns
; the greater of two numbers and apply it to arguments 4 and 9.
((lambda (x y)
  (if (>= x y)
      x
      y)) 4 9)
```
-> *9*

```scheme
; Create a procedure that prints the sum of a number's multiple of 10
; with its multiple of 100 to the standard output.
(lambda (x)
  (define y1 (* x 10))
  (define y2 (* x 100))
  (display (+ y1 y2)))
```
-> *[UserProcedure] Parameters: x*