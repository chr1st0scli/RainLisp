# or
A derived expression that implements a logical or. It accepts at least one expression.
```
"(" "or" expression+ ")"
```
Each expression is evaluated until one evaluates to true or the last expression is reached, in which case it is the result of the evaluation.

> `or` is syntactic sugar for a nested `if`.

> Note that in traditional LISP `(or)` with no operands, evaluates to `false`. RainLisp differentiates in this regard.

## Examples
```scheme
(or false true false)
```
-> *true*

```scheme
(or false false false)
```
-> *false*

```scheme
(or (< 1 0) (> 2 1) (< 3 2))
```
-> *true*

```scheme
(or (< 1 0) (< 2 1) (< 3 2))
```
-> *false*