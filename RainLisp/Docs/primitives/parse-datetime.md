# parse-datetime
```scheme
(parse-datetime str format)
```
Converts a string representation of a datetime in [invariant](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture) culture to its datetime equivalent, using a specified format.

> *str* is the string containing the datetime info.

> *format* is a string specifying the exact format of the datetime info. It can be a [standard](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) or [custom](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) format.

## Example
```scheme
(parse-datetime "2023-12-31 22:30:45.123" "yyyy-MM-dd HH:mm:ss.fff")
```
-> *2023-12-31 22:30:45.123*