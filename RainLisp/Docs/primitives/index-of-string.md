# index-of-string
```scheme
(index-of-string str value start-index)
```
Returns the zero-based index of the first occurence of a string withing another string. The search starts at a specified character position.
If the string is not found, -1 is returned. If the string to look for is empty, the return value is the start index.

> *str* is the string to search in.

> *value* is the string to look for.

> *start-index* is the search starting position. Note that only its integral part is considered.

## Example
```scheme
(index-of-string "RainLisp" "Lisp" 0)
```
-> *4*