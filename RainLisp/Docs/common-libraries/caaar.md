# caaar
```scheme
(define (caaar sequence)
  (car (caar sequence)))
```
Helper for accessing a particular element based on [car](../primitives/car.md) and [cdr](../primitives/cdr.md) primitives.

## Example
```scheme
(define sequence (cons (cons (cons 1 2) (cons 3 4)) (cons (cons 5 6) (cons 7 8))))
(caaar sequence)
```
-> *1*