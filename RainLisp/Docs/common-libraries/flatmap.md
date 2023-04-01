# flatmap
```scheme
(define (flatmap proc sequence)
  (fold-right append nil (map proc sequence)))
```
Projects each element of a list to a list of many and flattens the results in a single list.

> *proc* is a procedure accepting a single argument (each element of *sequence* at a time) and returning a list.

> *sequence* is the list whose each element is mapped to many.

## Example
```scheme
; Get a list where each number is followed by a boolean specifying if it is even.
(flatmap
  (lambda (n) (list n (= 0 (% n 2))))
  (list 1 2 3 4 5 6))
```
-> *(1 false 2 true 3 false 4 true 5 false 6 true)*