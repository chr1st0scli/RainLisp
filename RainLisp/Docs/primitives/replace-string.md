# replace-string
```scheme
(replace-string str old-value new-value)
```
Returns a new string in which all occurrences of a substring within a given string are replaced by another one.
If the string to be replaced is not found, the original string is returned unchanged.

> *str* is the string to search in.

> *old-value* is the string to be replaced.

> *new-value* is the string to replace all occurrences of the old value.

## Example
```scheme
(replace-string "Scheme LISP" "Scheme" "Rain")
```
-> *"Rain LISP"*