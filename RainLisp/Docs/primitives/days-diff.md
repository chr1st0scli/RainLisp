# days-diff
```scheme
(days-diff datetime-from datetime-to)
```
Returns the number of days, which can be positive or negative, between two datetimes.

> *datetime-from* is the datetime to subtract.

> *datetime-to* is the datetime to subtract the other one from.

## Example
```scheme
(days-diff (make-date 2023 12 1) (make-date 2023 12 31))
```
-> *30*