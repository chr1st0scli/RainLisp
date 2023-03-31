# cdr
```scheme
(cdr pair)
```
Returns the second element of a pair.

## Examples
```scheme
(cdr (cons "RainLisp" 2023))
```
-> *2023*

```scheme
(cdr (cons (cons 'Name "RainLisp") (cons 'Copyright 2023)))
```
-> *(Copyright . 2023)*