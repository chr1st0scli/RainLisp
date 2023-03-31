# flatmap
```scheme
(define (flatmap proc sequence)
  (fold-right append nil (map proc sequence)))
```

## Example
```scheme

```