# filter
```scheme
(define (filter predicate sequence)
  (cond ((null? sequence) nil)
        ((predicate (car sequence))
         (cons (car sequence) (filter predicate (cdr sequence))))
        (else (filter predicate (cdr sequence)))))
```
Returns a new list containing only the elements of a list that satisfy a condition.

> *sequence* is the list to filter.

> *predicate* is a procedure accepting a single argument (each element of *sequence* at a time) and its result is evaluated as a boolean.

## Example
```scheme
; Filter the even numbers of a list.
(filter
  (lambda (n) (= 0 (% n 2)))
  (list 1 2 3 4 5 6 7 8))
```
-> *(2 4 6 8)*