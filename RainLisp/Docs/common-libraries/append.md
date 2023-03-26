# append
```lisp
(define (append list1 list2)
  (fold-right cons list2 list1))
```