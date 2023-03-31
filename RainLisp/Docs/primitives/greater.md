# >
```scheme
(> num1 num2)
```
Determines if the first numeric value is greater than the second one.

```scheme
(> datetime1 datetime2)
```
Determines if the first datetime is later than the second one.

## Examples
```scheme
(> 7 4)
```
-> *true*

```scheme
(> (make-date 2022 12 31) (make-date 2023 12 31))
```
-> *false*