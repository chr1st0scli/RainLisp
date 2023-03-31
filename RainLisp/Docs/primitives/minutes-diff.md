# minutes-diff
```scheme
(minutes-diff datetime-from datetime-to)
```
Returns the number of minutes, ranging from -59 through 59, between the times of two datetimes.

> *datetime-from* is the datetime to subtract.

> *datetime-to* is the datetime to subtract the other one from.

## Example
```scheme
(minutes-diff (make-datetime 2023 12 30 20 35 0 0) (make-datetime 2023 12 31 22 55 0 0))
```
-> *20*