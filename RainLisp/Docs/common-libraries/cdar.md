# cdar
```scheme
(define (cdar sequence)
  (cdr (car sequence)))
```
Helper for accessing a particular element based on [car](..\primitives\car.md) and [cdr](..\primitives\cdr.md) primitives.

## Example
```scheme
(define sequence (cons (cons 1 2) (cons 3 4)))
(cdar sequence)
```
-> *2*