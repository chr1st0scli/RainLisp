# cons
```scheme
(cons first second)
```
Returns a pair made of the two given values.

## Examples
```scheme
(cons "RainLisp" 2023)
```
-> *("RainLisp" . 2023)*

```scheme
(cons (cons 'Name "RainLisp") (cons 'Copyright 2023))
```
-> *((Name . "RainLisp") Copyright . 2023)*