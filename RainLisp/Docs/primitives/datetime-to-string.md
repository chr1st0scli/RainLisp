# datetime-to-string
```scheme
(datetime-to-string datetime format)
```
Converts a datetime to its equivalent string representation, using a specified format in [invariant](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture) culture.

> *datetime* to convert.

> *format* is a [standard](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) or [custom](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) date and time format string.

## Example
```scheme
(datetime-to-string (make-datetime 2023 12 31 22 30 45 123) "yyyy-MM-dd HH:mm:ss.fff")
```
-> *"2023-12-31 22:30:45.123"*