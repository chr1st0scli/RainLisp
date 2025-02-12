# force
```scheme
(define (force proc)
  (proc))
```
Executes a procedure and returns its result. Typically the procedure is a promise, i.e. a delayed expression
that results from [delay](../special-forms-derived-expressions/delay.md).

> *proc* is the procedure to execute, typically a promise to evaluate an expression.

## Example
```scheme
(force (delay 4))
```
-> *4*