# cdr-stream
```scheme
(define (cdr-stream stream)
  (force (cdr stream)))
```
Returns the rest of a stream, i.e. its next element by forcing its promise to be fulfilled and a promise for the rest of it.

## Example
```scheme
(cdr-stream (make-range-stream 1 8000000))
```
-> *(2 . [UserProcedure] Parameters: 0)*