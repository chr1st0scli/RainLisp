# Conditionals
Conditionals allow you to specify different execution paths based on different conditions.
In simple words, you can specify different things to be done in different circumstances.

## If
[If](../special-forms-derived-expressions/if.md) is the most common conditional special form. You can specify a consequent expression to be
evaluated if a condition is met and optionally an alternative expression, in case it is not.
In other words, it is a common `if... else...` construct.

Let's write an if expression to see it in action.

```scheme
(define x 10)

(if (> x 0)
    "x is greater than zero."
    "x is less than or equal to zero.")
```
-> *"x is greater than zero."*

In the above example, we are defining a variable called `x` and set it to `10`.
The condition we are testing is if `x` is greater than `0`. In the above case, it is,
so the consequent is evaluated rather than the alternative.

Go ahead and set `x` to `0` or a negative number and re-execute to see what happens.

> As mentioned above, the alternative (else) expression is optional. If it is the one
to be evaluated and it is absent, *unspecified* is returned.

> *unspecified* is a special value in RainLisp and has no string representation.
This means that the interpreter will not print anything as a result. It will be as if nothing happens.

## Cond
[Cond](../special-forms-derived-expressions/cond.md) is more appropriate to use when you want to test many alternatives.
It consists of a collection of conditions and their respective expressions to be evaluated in case they are met.
It optionally ends with an `else` whose expression is evaluated in case all previous conditions were not met.
In other words, it is a common `if... else if... else...` construct.

Let's play around with an example.

```scheme
(define x 4)

(cond ((< x 0) "x is negative.")
      ((= x 0) "x is zero.")
      ((< x 10) "x is less than 10.")
      (else "x is greater than or equal to 10."))
```
-> *"x is less than 10."*

Go ahead and change `x` to `-4`, `0` and `14` and re-execute each time and see what happens.

## And

## Or