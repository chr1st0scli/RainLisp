# to-local
```scheme
(to-local datetime)
```
Converts a Universal Coordinated Time (UTC) datetime value to local. It must be a UTC or unspecified datetime, in which case it is treated as UTC.

## Example
```scheme
(to-local (make-datetime 2023 3 30 20 07 20 123))
```
-> *2023-03-30 23:07:20.123*