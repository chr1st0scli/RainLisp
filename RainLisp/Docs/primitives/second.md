# second
```scheme
(second datetime)
```
Returns the second of a given datetime value, expressed as a value between 0 and 59.

## Example
```scheme
(second (make-datetime 2023 12 31 23 59 30 123))
```
-> *30*