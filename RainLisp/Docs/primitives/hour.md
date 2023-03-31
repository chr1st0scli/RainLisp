# hour
```scheme
(hour datetime)
```
Returns the hour of a given datetime value, expressed as a value between 0 and 23.

## Example
```scheme
(hour (make-datetime 2023 12 31 23 59 59 999))
```
-> *23*