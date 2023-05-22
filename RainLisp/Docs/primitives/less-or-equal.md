# <=
```scheme
(<= num1 num2)
```
Determines if the first numeric value is less than or equal to the second one.

```scheme
(<= datetime1 datetime2)
```
Determines if the first datetime is the same as or earlier than the second one, ignoring time zones.

## Examples
```scheme
(<= 7 4)
```
-> *false*

```scheme
(<= (make-date 2022 12 31) (make-date 2023 12 31))
```
-> *true*