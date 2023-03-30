# millisecond
```scheme
(millisecond datetime)
```
Returns the millisecond of a given datetime value, expressed as a value between 0 and 999.

## Example
```scheme
(millisecond (make-datetime 2023 12 31 23 59 30 123))
```
-> *123*