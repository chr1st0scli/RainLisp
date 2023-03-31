# milliseconds-diff
```scheme
(milliseconds-diff datetime-from datetime-to)
```
Returns the number of milliseconds, ranging from -999 through 999, between the times of two datetimes.

> *datetime-from* is the datetime to subtract.

> *datetime-to* is the datetime to subtract the other one from.

## Example
```scheme
(milliseconds-diff (make-datetime 2023 12 30 20 35 20 100) (make-datetime 2023 12 31 22 55 59 888))
```
-> *788*