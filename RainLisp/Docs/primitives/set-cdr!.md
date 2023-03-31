# set-cdr!
```scheme
(set-cdr! pair value)
```
Sets the second part of a pair to the value provided. Its return value is unspecified.

## Example
```scheme
(define a-pair (cons 1 1))
(set-cdr! a-pair 2)
a-pair
```
-> *(1 . 2)*