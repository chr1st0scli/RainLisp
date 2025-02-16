# make-range-stream
```scheme
(define (make-range-stream start end)
  (if (> start end)
      nil
      (cons-stream start (make-range-stream (+ start 1) end))))
```
Returns a new stream of numeric values ranging from *start* until *end*. The first numeric value is evaluated and the evaluation of the rest is deferred,
i.e. delayed until explicitly requested. If *start* is greater than *end*, [nil](../primitives/nil.md) is returned.

> *start* is the first value of the stream which is immediately evaluated.

> *end* is the last value of the stream.

## Example
```scheme
; Returns a stream made of the first element and a promise to fulfill the rest of it.
(make-range-stream 7 5000000)
```
-> *(7 . [UserProcedure] Parameters: 0)*