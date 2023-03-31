# display
```scheme
(display primitive-value)
```
Writes a primitive value to the standard output. The format of the output is determined by the local culture. Its return value is unspecified.

> *primitive-value* is either a boolean, number, string or datetime.

## Example
```scheme
(display "RainLisp")
```