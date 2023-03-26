# reverse
```scheme
(define (reverse sequence)
  (fold-left (lambda (x y) (cons y x)) nil sequence))
```

## Example
```scheme

```