# Booleans
Booleans is another RainLisp's basic data type. A boolean is either `true` or `false`.

Let's evaluate some boolean literals.

```scheme
true
```
-> *true*

```scheme
false
```
-> *false*

There is a primitive procedure `not` that returns the logical negation of the argument.

```scheme
(not true)
```
-> *false*

```scheme
(not false)
```
-> *true*

> Note that in RainLisp, in conditional expressions like `if`, `cond` and others like `not` that expect booleans,
all values other than false are considered to be true. Therefore, `(not 0)` and `(not 1)` both give `false`.

Apart from [not](../primitives/not.md), there exist all common boolean logical operations like [and](../special-forms-derived-expressions/and.md),
[or](../special-forms-derived-expressions/or.md) and [xor](../primitives/xor.md).

Next, let's learn about [strings](strings.md).