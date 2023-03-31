# add-days
```scheme
(add-days datetime num)
```
Returns a new datetime that adds the specified number of days to the specified datetime.

> *datetime* to add days to.

> *num* is the whole and fractional number of days to add, which can be positive or negative.

## Example
```scheme
(add-days (make-date 2023 5 1) 30)
```
-> *2023-05-31 00:00:00.000*