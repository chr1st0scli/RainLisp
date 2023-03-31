# add-hours
```scheme
(add-hours datetime num)
```
Returns a new datetime that adds the specified number of hours to the specified datetime.

> *datetime* to add hours to.

> *num* is the whole and fractional number of hours to add, which can be positive or negative.

## Example
```scheme
(add-hours (make-date 2023 12 31) 5.5)
```
-> *2023-12-31 05:30:00.000*