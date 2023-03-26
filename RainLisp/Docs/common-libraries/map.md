# map
```scheme
(define (map proc sequence)
  (if (null? sequence)
      nil
      (cons (proc (car sequence)) (map proc (cdr sequence)))))
```