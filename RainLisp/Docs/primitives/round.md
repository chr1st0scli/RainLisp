# round
```scheme
(round num decimals)
```
Rounds a numeric value to a specified number of fractional digits, using the away from zero rounding convention.
If the given numeric value to round has fewer fractional digits than the one specified, it is returned unchanged.

> *num* is a numeric value to round.

> *decimals* is the number of decimal places in the return value (0 through 28).

Note that only the integral part of decimal places is considered.

## Examples
```scheme
(round 231.45478 2)
```
-> *231.45*

```scheme
(round 231.45578 2)
```
-> *231.46*