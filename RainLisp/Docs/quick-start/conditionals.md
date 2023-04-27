# Conditionals
Conditionals allow you to specify different execution paths based on different conditions.
In simple words, you can specify different things to be done in different circumstances.

## If
[If](../special-forms-derived-expressions/if.md) is the most common conditional special form. You can specify a consequent expression to be
evaluated if a condition is met and optionally an alternative expression, in case it is not.
In other words, it is a common `if...` or `if... else...` construct.

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

According to `if`'s specification, the consequent and alternative parts are single expressions.
So, what happens if you want to execute multiple expressions when a condition is met?
That's right, we talked about `begin` in the previous section.

```scheme
(define x 10)

(if (> x 0)
    (begin
      (display x)
      (newline)
      "x is greater than zero.")
    (begin
      (display x)
      (newline)
      "x is less than or equal to zero."))
```
->
```
10
"x is greater than zero."
```

Notice that in the above example, both the consequent and alternative are `begin` expressions.
The `if`'s condition is met, so the value of `x` and then a new line are written to the standard output.
Finally, a descriptive string is returned just like in the previous example.

## Cond
[Cond](../special-forms-derived-expressions/cond.md) is more appropriate to use when you want to test many alternatives.
It consists of a collection of conditions and their respective expressions to be evaluated in case they are met.
It optionally ends with an `else`, which is evaluated in case all previous conditions are not met.
In other words, it is a common `if...` or `if... else if...` or `if... else if... else...` construct with possibly multiple `else if`s.

Let's play around with an example.

```scheme
(define x 4)

(cond ((< x 0) "x is negative.")
      ((= x 0) "x is zero.")
      ((< x 10) "x is less than 10.")
      (else "x is greater than or equal to 10."))
```
-> *"x is less than 10."*

Go ahead and change `x` to `-4`, `0` and `14` and re-execute each time to see what happens.

> Once again, if the `else` is to be evaluated and it is absent, *unspecified* is returned.

The syntax of `cond` makes it possible to have many expressions evaluated when a condition is met.
Therefore, unlike `if`, you don't need to use a `begin` expression.

```scheme
(define x 4)

(cond ((< x 0)
       (display x)
       (newline)
       "x is negative.")
      ((= x 0) "x is zero.")
      ((< x 10)
       (display x)
       (newline)
       "x is less than 10.")
      (else "x is greater than or equal to 10."))
```
->
```
4
"x is less than 10."
```

Notice that we have specified many expressions to be evaluated in only two cases,
when `x` is negative or less than 10. Experiment with different values of `x` and
see what happens.

The conditions that are checked for both `if` and `cond` can become more complex
by using the `and` and `or` derived expressions presented below.

## And
This is the typical logical `and`. It accepts at least one expression and
checks if all evaluate to something equivalent to `true`.
If one evaluates to `false`, it doesn't need to evaluate the rest of them.

Let's see an example.

```scheme
(and 
  (display 1)
  (newline)
  (display 2)
  (newline)
  false
  (display 3))
```
->
```
1
2
false
```

The primitive procedures `display` and `newline` write to the standard output but return the
*unspecified* result, which is different than `false`. Recall that anything other than `false` is considered
to be `true` in RainLisp. So, the evaluation continues until `false` is reached. Therefore, `3`
is never written to the standard output.

Let's combine an `and` with an `if` to see a more realistic example.

```scheme
(define dt (make-date 2023 12 31))

(if (and
      (= (month dt) 12)
      (= (day dt) 31))
    "It's new year's eve!"
    "It's not new year's eve yet :-(")
```
-> *"It's new year's eve!"*

In the above example, it's the 31st of December so it's new year's eve.
Go ahead and experiment with different values for `dt`.

You can see more details about [and](../special-forms-derived-expressions/and.md) if you want.

## Or
This is the typical logical `or`. It accepts at least one expression and
checks if any of them evaluates to something equivalent to `true`.
If one evaluates to `true`, it doesn't need to evaluate the rest of them.

Let's see an example.

```scheme
(define dt (make-date 2023 12 1))

(if (or
      (= (day dt) 1)
      (= (day dt) 31))
    "It's either the 1st or 31st of the month. Pay day!"
    "It's neither the 1st nor the 31st of the month.")
```
-> *"It's either the 1st or 31st of the month. Pay day!"*

In the example above, `(= (day dt) 31)` is not evaluated because
`(= (day dt) 1)` is the first to evaluate to `true`.
Go ahead and experiment with different values for `dt`.

You can see more details about [or](../special-forms-derived-expressions/or.md) if you want.

Now, let's get more serious and learn how to define [procedures](procedures.md).