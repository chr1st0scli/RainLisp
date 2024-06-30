# parse-number-culture
```scheme
(parse-number-culture str culture)
```
Converts a string representation of a numeric value in a culture-specific format to its number equivalent.

> *str* is the string containing the numeric info.

> *culture* is a string specifying the culture name. If the culture name is an empty string, the [invariant](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture) culture is used.
For a list of valid names, look at the "*Language tag*" column of the [language table](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c).

## Examples
```scheme
(parse-number-culture "2345.6789" "en-EN")
```
-> *2345.6789*

```scheme
(parse-number-culture "2345,6789" "el-GR")
```
-> *2345.6789*