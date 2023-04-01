# fold-left
```scheme
(define (fold-left op initial sequence)
  (define (iter result rest)
    (if (null? rest)
        result
        (iter (op result (car rest)) (cdr rest))))
  (iter initial sequence))
```
Returns a result by accumulating a computation on a list from left to right.

> *op* is a procedure accepting two arguments and returning the result of accumulation on each step.

> *initial* is the seed of the accumulation.

> *sequence* is the list to accumulate.

The first *op* call's arguments is the initial seed followed by the first element of the list.

## Example
```scheme
; ((10 - 1) - 2) - 3
(fold-left - 10 (list 1 2 3))
```
-> *4*