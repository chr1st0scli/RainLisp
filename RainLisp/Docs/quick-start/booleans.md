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

> Note that in RainLisp, in conditional expressions like `if`, `cond` and others like `not`, `and`, `or` that expect booleans,
all values other than false are considered to be true. For example, `(not 0)` and `(not 1)` both give `false`.

The boolean logical operations are:
- [and](../special-forms-derived-expressions/and.md)
- [not](../primitives/not.md)
- [or](../special-forms-derived-expressions/or.md)
- [xor](../primitives/xor.md)

Next, let's learn about [strings](strings.md).