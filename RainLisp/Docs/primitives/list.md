# list
```scheme
(list . values)
```
Returns a new list made of the values provided, or nil if none is given.

## Examples
```scheme
(list "RainLisp" 2023 true)
```
-> *("RainLisp" 2023 true)*

```scheme
; Returns nil, i.e. the empty list.
(list)
```
-> *()*