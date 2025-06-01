# at-list
```scheme
(define (at-list items index)
    (if (<= index 0)
        (car items)
        (at-list (cdr items) (- index 1))))
```
Returns the element of a list at a given position.

> *items* is the list to look for the required element in.

> *index* is the zero-based position of the required element in the list. If it is negative, zero is assumed.

## Example
```scheme
(at-list (list 1 2 3 4) 1)
```
-> *2*