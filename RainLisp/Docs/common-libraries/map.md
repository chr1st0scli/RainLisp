# map
```scheme
(define (map proc sequence)
  (if (null? sequence)
      nil
      (cons (proc (car sequence)) (map proc (cdr sequence)))))
```
Returns a new list with the projections of a given list's elements.

> *proc* is a procedure accepting a single argument (each element of *sequence* at a time) and returning a projection.

> *sequence* is the list whose each element is mapped to another one.

## Example
```scheme
; Multiply each number by 10.
(map
  (lambda (n) (* n 10))
  (list 1 2 3 4 5))
```
-> *(10 20 30 40 50)*