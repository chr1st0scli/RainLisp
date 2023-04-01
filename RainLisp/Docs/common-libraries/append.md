# append
```scheme
(define (append list1 list2)
  (fold-right cons list2 list1))
```
Returns a new list by appending *list2* to *list1*.

## Example
```scheme
(append (list 1 2 3) (list 4 5 6))
```
-> *(1 2 3 4 5 6)*