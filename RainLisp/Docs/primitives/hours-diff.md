# hours-diff
```scheme
(hours-diff datetime-from datetime-to)
```
Returns the number of hours, ranging from -23 through 23, between the times of two datetimes.

> *datetime-from* is the datetime to subtract.

> *datetime-to* is the datetime to subtract the other one from.

## Example
```scheme
(hours-diff (make-datetime 2023 12 30 20 0 0 0) (make-datetime 2023 12 31 22 0 0 0))
```
-> *2*