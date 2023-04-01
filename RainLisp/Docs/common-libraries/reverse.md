# reverse
```scheme
(define (reverse sequence)
  (fold-left (lambda (x y) (cons y x)) nil sequence))
```
Returns a new list by reversing the elements of a given one.

## Example
```scheme
(reverse (list 1 2 3 4))
```
-> *(4 3 2 1)*