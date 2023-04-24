# set!
Assignment is a special form for changing a variable's value.
`ID` is the name of the identifier to change and expression gives the new value.
```
"(" "set!" ID expression ")"
```
The identifier needs to be visible in the current scope; otherwise, an error occurs.
The evaluation result of the assignment itself is unspecified.

## Examples
```scheme
(define my-var 1) ; Define a variable.
(set! my-var 20) ; Change its value.

my-var ; Get its value.
```
-> *20*

```scheme
; Define a function with one parameter.
(define (foo x)
  ; Increment the parameter's value.
  (set! x (+ x 1))
  ; Return its value.
  x)

(foo 3) ; Call the function.
```
-> *4*

```scheme
; Set a value to a variable that is not defined in the current scope.
(set! a 5)
```
-> Gives the following error.
```
Unknown identifier a.
Call Stack
[Assignment a] Line 2, position 2.
```