# debug
```scheme
(debug primitive-value)
```
Writes a primitive value to the trace listeners in the debug [listeners](https://learn.microsoft.com/en-us/dotnet/framework/debug-trace-profile/trace-listeners) collection. The format of the output is determined by the local culture. Its return value is unspecified.

> *primitive-value* is either a boolean, number, string or datetime.

## Example
```scheme
(debug "RainLisp")
```