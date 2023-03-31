# eval
```scheme
(eval quote-symbol)
```
Returns a result by evaluating a quote symbol as user code.

```scheme
(eval quote-symbols-list)
```
Returns a result by evaluating a non-empty list of quote symbols as user code.

## Examples
```scheme
(define a 12)
(eval 'a)
```
-> *12*

```scheme
(define a 1)
(eval '(begin (set! a (+ a 1)) a))
```
-> *2*