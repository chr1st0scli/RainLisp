# add-seconds
```scheme
(add-seconds datetime num)
```
Returns a new datetime that adds the specified number of seconds to the specified datetime.

> *datetime* to add seconds to.

> *num* is the whole and fractional number of seconds to add, which can be positive or negative.

## Example
```scheme
(add-seconds (make-date 2023 12 31) 25.5)
```
-> *2023-12-31 00:00:25.500*