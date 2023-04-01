# reduce
```scheme
(define (reduce op sequence)
  (cond ((null? sequence) nil)
        ((null? (cdr sequence)) (car sequence))
        (else (fold-left op (car sequence) (cdr sequence)))))
```
Returns a result by accumulating a computation on a list from left to right.

> *op* is a procedure accepting two arguments and returning the result of accumulation on each step.

> *sequence* is the list to accumulate.

## Example
```scheme
(reduce + (list 1 2 3 4))
```
-> *10*