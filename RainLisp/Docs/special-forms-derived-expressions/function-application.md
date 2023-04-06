# Function Application
A special form for applying a function to optional arguments.
This is typically known as a function call.

> In Scheme LISP, a function might be more often referred to as a procedure.

The first expression gives the procedure to call. The following zero or more expressions
are evaluated from left to right and give the arguments passed to the procedure's parameters.

```
"(" expression expression* ")"
```
The body of the procedure is evaluated in a new inner scope where the procedure's parameters are
bound to the values passed as arguments.

## Examples
```scheme
; Call the primitive procedure +.
(+ 1 2 3 4)
```
-> *10*

```scheme
; Define a procedure that prints 1 and 2 to the standard output on different lines.
(define (print-to-2)
  (define x 1)
  (display x)
  (newline)
  (set! x (+ x 1))
  (display x)
  (newline))

; Call the procedure.
(print-to-2)
```
->
```
1
2
```

```scheme
; Define a procedure giving the absolute value of a number.
(define (abs x)
  (if (< x 0) 
      (* -1 x)
      x))

; Call the procedure.
(abs -7)
```
-> *7*

```scheme
; Declare an anonymous user procedure, i.e. a lambda, that returns
; the greater of two numbers and apply it to arguments 4 and 9.
((lambda (x y)
  (if (>= x y)
      x
      y)) 4 9)
```
-> *9*