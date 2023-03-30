# minute
```scheme
(minute datetime)
```
Returns the minute of a given datetime value, expressed as a value between 0 and 59.

## Example
```scheme
(minute (make-datetime 2023 12 31 23 59 30 123))
```
-> *59*