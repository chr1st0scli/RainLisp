# length
```scheme
(define (length sequence)
  (fold-left (lambda (count n) (+ count 1)) 0 sequence))
```
Returns the length of a list.

## Example
```scheme
(length (list 1 2 3 4))
```
-> *4*