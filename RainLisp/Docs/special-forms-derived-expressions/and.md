# and
A derived expression that implements a logical and. It accepts at least one expression.
```
(and expression . expressions)
```
Each expression is evaluated from left to right, until one evaluates to false or the last expression is reached, in which case it is the result of the evaluation.

## Examples
```scheme
(and true true true)
```
-> *true*

```scheme
(and true false true)
```
-> *false*

```scheme
(and (> 1 0) (> 2 1) (> 3 2))
```
-> *true*

```scheme
(and (> 1 0) (< 2 1) (> 3 2))
```
-> *false*

## Remarks
> `and` is syntactic sugar for a nested `if`.

> Note that in traditional LISP `(and)` with no operands, evaluates to `true`. RainLisp differentiates in this regard.