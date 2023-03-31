# length
```scheme
(define (length sequence)
  (define (length-iter seq count)
    (if (null? seq)
        count
        (length-iter (cdr seq) (+ 1 count))))
  (length-iter sequence 0))
```

## Example
```scheme

```