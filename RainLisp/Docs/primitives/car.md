# car
```scheme
(car pair)
```
Returns the first element of a pair.

## Examples
```scheme
(car (cons "RainLisp" 2023))
```
-> *"RainLisp"*

```scheme
(car (cons (cons 'Name "RainLisp") (cons 'Copyright 2023)))
```
-> *(Name . "RainLisp")*