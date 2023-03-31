# reduce
```scheme
(define (reduce op sequence)
  (cond ((null? sequence) nil)
        ((null? (cdr sequence)) (car sequence))
        (else (fold-left op (car sequence) (cdr sequence)))))
```

## Example
```scheme

```