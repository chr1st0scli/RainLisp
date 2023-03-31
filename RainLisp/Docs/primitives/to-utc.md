# to-utc
```scheme
(to-utc datetime)
```
Converts a local datetime value to Universal Coordinated Time (UTC). It must be a local or unspecified datetime, in which case it is treated as local.

## Example
```scheme
(to-utc (make-datetime 2023 3 30 21 07 20 123))
```
-> *2023-03-30 18:07:20.123*