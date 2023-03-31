# number-to-string
```scheme
(number-to-string num format)
```
Converts a numeric value to its equivalent string representation, using a specified format in [invariant](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture) culture.

> *num* is a numeric value to convert.

> *format* is a numeric format string value. It can be a [standard](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings) or [custom](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings) format.

## Example
```scheme
(number-to-string 23.4678 "000.000")
```
-> *"023.468"*