# parse-number
```scheme
(parse-number str)
```
Converts a string representation of a numeric value in [invariant](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture) culture to its number equivalent.

## Example
```scheme
(parse-number "2345.6789")
```
-> *2345.6789*