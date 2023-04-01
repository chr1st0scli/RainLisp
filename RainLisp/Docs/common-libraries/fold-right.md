# fold-right
```scheme
(define (fold-right op initial sequence)
  (if (null? sequence)
      initial
      (op (car sequence) (fold-right op initial (cdr sequence)))))
```

Returns a result by accumulating a computation on a list from right to left.

> *op* is a procedure accepting two arguments and returning the result of accumulation on each step.

> *initial* is the seed of the accumulation.

> *sequence* is the list to accumulate.

The first *op* call's arguments is the last element of the list followed by the initial seed.

## Example
```scheme
; 1 - (2 - (3 - 10))
(fold-right - 10 (list 1 2 3))
```
-> *-8*