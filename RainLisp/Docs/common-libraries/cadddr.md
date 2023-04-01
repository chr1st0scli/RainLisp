# cadddr
```scheme
(define (cadddr sequence)
  (car (cdddr sequence)))
```
Helper for accessing a particular element based on [car](../primitives/car.md) and [cdr](../primitives/cdr.md) primitives.

## Example
```scheme
(define sequence (cons (cons (cons (cons 1 2) (cons 3 4)) (cons (cons 5 6) (cons 7 8))) (cons (cons (cons 9 10) (cons 11 12)) (cons (cons 13 14) (cons 15 16)))))
(cadddr sequence)
```
-> *15*