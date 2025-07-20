# map-stream
```scheme
(define (map-stream proc stream)
  (if (null? stream)
      nil
      (cons-stream (proc (car stream)) (map-stream proc (cdr-stream stream)))))
```
Returns a new stream with the projection of the first element and a promise for the projection of the rest of them.

> *proc* is a procedure accepting a single argument (each element of *stream* at a time) and returning a projection.

> *stream* is the delayed list whose each element is mapped to another one on demand.

## Examples
```scheme
; Multiply each number in a stream by 10.
; It returns the projected first element and a promise to project the rest of them.
(map-stream 
  (lambda (n) (* n 10))
  (make-range-stream 1 5))
```
-> *(10 . [MemoizedUserProcedure] Parameters: 0)*

```scheme
; Create a stream of multiples of 10.
(define projected-stream
  (map-stream 
    (lambda (n) (* n 10))
    (make-range-stream 1 5)))

; Project the second element.
(car (cdr-stream projected-stream))
```
-> *20*