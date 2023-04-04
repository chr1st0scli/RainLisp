# define
Definition is a special form for defining variables and functions.
A variable is defined by its identifier `ID` followed by an expression that gives its value.
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

my-var ; Get value of my-var.
```
-> *10*

```scheme
(define my-var 10) ; Define my-var.
(define my-var 11) ; Redefine my-var.

my-var ; Get value of my-var.
```
-> *11*

```scheme
; Define a function with no parameters that returns a string.
(define (foo)
  "foo applied.")

foo ; Get value of foo.
```
-> *[UserProcedure] Parameters: 0*

```scheme
; Define a function with two parameters that returns a string.
(define (foo x y)
  (+ "foo applied on "
     x
     " and "
     y
     "."))

foo ; Get value of foo.
```
-> *[UserProcedure] Parameters: x, y*

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