# define
Definition is a special form for defining variables and procedures in the current scope.
The evaluation result of the definition itself is unspecified.

## Variable Definition
A variable is defined by its identifier `id` followed by an expression that gives its value.
```
(define id expression)
```

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

## Procedure Definition
A procedure is defined by its `name` followed by zero or more parameters and a body.
```
(define (name . parameters) body)
```

The body consists of zero or more definitions, followed by at least one expression.
The expressions are evaluated in the order they appear and the evaluation result of the last one
is the final result of the procedure when called.

## Examples
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

## Remarks

> One could argue that a RainLisp procedure could be more appropriately referred to as a function, since there is always a return value.
But in Scheme LISP, it is more often referred to as a procedure.

> Note that the form for defining functions is syntactic sugar for `(define id (lambda (. parameters) body)`.

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

So, when foo is called like so `(foo 1 2)`, what happens is that foo is evaluated first, which gives
the user procedure, then the expressions `1` and `2` are evaluated in turn and finally the
procedure is applied to them. See, [procedure application](function-application.md).