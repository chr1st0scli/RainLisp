﻿# Numbers
Numbers is one of the basic data types of RainLisp.
You can specify a number with an optional number sign, some digits and optionally decimal digits.

Let's try evaluating some numbers.

```scheme
12
```
-> *12*

```scheme
+12.34
```
-> *12.34*

```scheme
-12.34
```
-> *-12.34*

> Note that under the hood, all RainLisp numbers are 64-bit floating point, which reminds the way Javascript treats them.

You can do all the typical operations on numbers that you are already familiar with.
Let's add some.

```scheme
(+ 1 2 3 4)
```
-> *10*

> Note that RainLisp, being a LISP dialect, uses prefix notation instead of infix.
I.e. instead of `1 + 2 + 3 + 4`, you specify the + operator once, followed by the operands 
and surround everything with parentheses like so: `(+ 1 2 3 4)`.

Let's subtract some numbers.

```scheme
(- 10 5.5)
```
-> *4.5*

The operations that relate to numbers are:

- [+](../primitives/plus.md)
- [-](../primitives/minus.md)
- [*](../primitives/multiply.md)
- [/](../primitives/divide.md)
- [%](../primitives/modulo.md)
- [<](../primitives/less.md)
- [<=](../primitives/less-or-equal.md)
- [=](../primitives/equal.md)
- [>](../primitives/greater.md)
- [>=](../primitives/greater-or-equal.md)
- [ceiling](../primitives/ceiling.md)
- [floor](../primitives/floor.md)
- [number-to-string](../primitives/number-to-string.md)
- [parse-number](../primitives/parse-number.md)
- [parse-number-culture](../primitives/parse-number-culture.md)
- [round](../primitives/round.md)

Next, let's learn about [booleans](booleans.md).
