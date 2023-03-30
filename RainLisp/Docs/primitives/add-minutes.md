# add-minutes
```scheme
(add-minutes datetime num)
```
Returns a new datetime that adds the specified number of minutes to the specified datetime.

> *datetime* to add minutes to.

> *num* the whole and fractional number of minutes to add, which can be positive or negative.

## Example
```scheme
(add-minutes (make-date 2023 12 31) 25.5)
```
-> *2023-12-31 00:25:30.000*