# filter-stream
```scheme
(define (filter-stream predicate stream)
  (cond ((null? stream) nil)
        ((predicate (car stream))
         (cons-stream (car stream) (filter-stream predicate (cdr-stream stream))))
        (else (filter-stream predicate (cdr-stream stream)))))
```
Returns a new stream containing the first element of a stream that satisfies a condition and a promise for the rest of them.

> *predicate* is a procedure accepting a single argument (each element of *stream* at a time) and its result is evaluated as a boolean.

> *stream* is the stream to filter.

## Examples
```scheme
; Filter the even numbers of a stream.
; It returns the first even number and a promise to filter the rest of them.
(filter-stream
  (lambda (n) (= 0 (% n 2)))
  (make-range-stream 1 8))
```
-> *(2 . [MemoizedUserProcedure] Parameters: 0)*

```scheme
; Create a stream of evens.
(define evens
  (filter-stream
    (lambda (n) (= 0 (% n 2)))
    (make-range-stream 1 8)))

; Find the second even.
(car (cdr-stream evens))
```
-> *4*