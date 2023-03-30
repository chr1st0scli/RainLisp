# add-milliseconds
```scheme
(add-milliseconds datetime num)
```
Returns a new datetime that adds the specified number of milliseconds to the specified datetime.

> *datetime* to add milliseconds to.

> *num* is the whole and fractional number of milliseconds to add, which can be positive or negative. Note that it is rounded to the nearest integer.

## Example
```scheme
(add-milliseconds (make-date 2023 12 31) 400.5)
```
-> *2023-12-31 00:00:00.401*