# at-stream
```scheme
(define (at-stream stream index)
    (if (<= index 0)
        (car stream)
        (at-stream (cdr-stream stream) (- index 1))))
```
Returns the element of a stream at a given position. It fulfills the promises in the stream until the required position is reached.

> *stream* is the stream to look for the required element in.

> *index* is the zero-based position of the required element in the stream. If it is negative, zero is assumed.

## Example
```scheme
(at-stream (make-range-stream 1 5000) 1)
```
-> *2*