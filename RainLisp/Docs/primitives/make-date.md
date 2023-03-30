# make-date
```scheme
(make-date year month day)
```
Returns a new datetime value in an unspecified time zone, made of a year, month and day of the month.

> *year* (1 through 9999).

> *month* (1 through 12).

> *day* (1 through the number of days in month).

Note that only the integral part of the arguments is considered.

## Example
```scheme
(make-date 2023 12 31)
```
-> *2023-12-31 00:00:00.000*