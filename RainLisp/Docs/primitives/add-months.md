# add-months
```scheme
(add-months datetime num)
```
Returns a new datetime that adds the specified number of months to the specified datetime.

> *datetime* to add months to.

> *num* is the number of months to add which can be positive or negative.

Note that only the integral part of months is considered.

## Example
```scheme
(add-months (make-date 2023 5 31) 7)
```
-> *2023-12-31 00:00:00.000*