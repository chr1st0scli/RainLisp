# begin
A special form for defining a block of expressions to evaluate in the order they appear.
```
"(" "begin" expression+ ")"
```
The evaluation result of the `begin` itself, is the last expression's result.

## Example
```scheme
; Define a variable.
(define a 1)
; Evaluate a code block.
(begin
  (set! a (+ a 1))
  (set! a (+ a 1))
  (set! a (+ a 1))
  a)
```
-> *4*