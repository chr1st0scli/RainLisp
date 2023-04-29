# Procedures
Procedures are very important in RainLisp. It's the means of code reuse and abstraction.
With them, you can also apply techniques that resemble principles that you might be familiar
with in object-oriented programming languages, like classes, encapsulation and polymorphism.
We will talk about these later in the advanced section, you don't need to worry about it now.

> Other programming languages might use the term functions or methods. It's pretty much the same thing.

## Procedure Definition & Usage
We have seen the `define` keyword when defining variables. Guess what; you can also define a procedure
with the same keyword.

Let's define a procedure that takes zero arguments and returns a string.

```scheme
(define (foo) "foo")
```

Notice that instead of a variable name, we specified the procedure name inside parentheses.
This is what makes `foo` a procedure. The one and only thing `foo` does is that it returns a
string.

Let's evaluate the procedure name to see what we get.

```scheme
foo
```
-> *[UserProcedure] Parameters: 0*

Indeed, `foo` is a procedure. Now let's call it by simply enclosing its name in parentheses.

```scheme
(foo)
```
-> *"foo"*

As you can see, the string was returned.

Now, let's define a procedure that finds the maximum of two numbers and call it.

```scheme
(define (max num1 num2)
  (if (>= num1 num2)
      num1
      num2))

(max 4 7)
```
-> *7*.

Notice that after the procedure name `max`, we are specifying the parameter names `num1` and `num2`.
The body of the procedure contains an `if` expression that we have already covered.

Generally, the body of a procedure starts with zero or more definitions, followed by at least one
expression. The expressions are executed in sequence and the return value of the procedure is the
last expression's result.

Let's build on the previous example to see all this in practice.

```scheme
(define (max num1 num2)
  (define num1-str (number-to-string num1 ""))
  (define num2-str (number-to-string num2 ""))
  (define log-message (+ "Finding the max between " num1-str " and " num2-str "."))

  (display log-message)
  (newline)

  (if (>= num1 num2)
      num1
      num2))

(max 4 7)
```
->
```
Finding the max between 4 and 7.
7
```

This time, when `max` is called, three local variables are defined, `num1-str`, `num2-str` and `log-message`.
The first two local variables are the string representations of `num1` and `num2` parameter values.
We create these with the primitive procedure [number-to-string](../primitives/number-to-string.md)
and use them to build a descriptive log message.

After the definitions, the expressions are executed in turn. The log message and a new line are written to the standard
output. The final expression is the `if` whose result is returned by the procedure as we have seen before.

> As you would expect, both parameters `num1`, `num2` and the local variables are only visible
within the procedure. Their scope is said to be local in `max` and they are not accessible outside of it,
like in the global scope.

> As we talked about, local variables can only be defined in the beginning of the procedure's body.
This is not common in other LISP dialects, where you can define local variables anywhere within the body.

## Lambdas

## Passing Around Procedures

