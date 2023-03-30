# make-datetime
```scheme
(make-datetime year month day hour minute second millisecond)
```
Returns a new datetime value in an unspecified time zone, made of a year, month, day of the month, hour, minute, second and millisecond.

> *year* (1 through 9999).

> *month* (1 through 12).

> *day* (1 through the number of days in month).

> *hour* (0 through 23).

> *minute* (0 through 59).

> *second* (0 through 59).

> *millisecond* (0 through 999).

Note that only the integral part of the arguments is considered.

## Example
```scheme
(make-datetime 2023 12 31 23 59 59 999)
```
-> *2023-12-31 23:59:59.999*