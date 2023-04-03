# define
Definition is a special form for defining variables and functions. A variable is defined by its identifier `ID` followed by an expression.
```
"(" "define" ID expression ")"
```
A function is defined by its name `ID` followed by zero or more parameters `ID*` and a body.
```
"(" "define" "(" ID ID* ")" body ")"
```
The evaluation result of the definition itself is unspecified.

## Examples
```scheme
; define a variable called my-var with the value 10.
(define my-var 10)
```

```scheme
; Define a function with no parameters that returns a string.
(define (foo)
  "foo applied.")
```

```scheme
; Define a function with two parameters that returns a string.
(define (foo x y)
  (+ "foo applied on "
     x
     " and "
     y
     "."))
```

> Note that the form for defining functions is syntactic sugar for `(define ID (lambda (ID*) body)`.

For example,
```scheme
(define (foo x y)
  (+ x y))
```
is really a user procedure, i.e. a `lambda`, assigned to variable `foo`.
```scheme
(define foo
  (lambda (x y)
    (+ x y)))
```