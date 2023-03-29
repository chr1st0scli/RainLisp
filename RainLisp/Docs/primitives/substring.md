# substring
```scheme
(substring str start-index length)
```
Returns a substring value of a given string. The substring starts at a specified character position and has a specified length.
An empty string is returned if the start index is equal to the length of the string and the desired length is zero.

> *str* is the string to get a substring from.

> *start-index* is the zero-based start index.

> *length* is the length of the substring.

Note that only the integral part of the numeric arguments is considered.

> Note that in traditional LISP, the last parameter is not the length but a stop index. RainLisp differentiates in this regard.

## Example
```scheme
(substring "RainLisp" 0 4)
```
-> *"Rain"*