# set-car!
```scheme
(set-car! pair value)
```
Sets the first part of a pair to the value provided. Its return value is unspecified.

## Example
```scheme
(define a-pair (cons 1 1))
(set-car! a-pair 0)
a-pair
```
-> *(0 . 1)*