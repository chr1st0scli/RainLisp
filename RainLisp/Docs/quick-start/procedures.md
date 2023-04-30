# Procedures
Procedures are very important in RainLisp. It's the means of code reuse and abstraction.
With them, you can also apply techniques that resemble principles that you might be familiar
with in object-oriented programming languages, like classes, encapsulation and polymorphism,
which we will talk about later in the advanced section.

> Other programming languages might use the term functions or methods. It's pretty much the same thing.

## Procedure Definition & Usage
We have seen the `define` keyword when defining variables. Guess what; you can also define a procedure
with the same keyword.

Let's define a procedure that takes zero arguments and returns a string.

```scheme
(define (foo) "foo")
```

> Arguments are the actual values passed to a procedure's parameters when the latter is called.
But some people might use the two terms interchangeably to refer to the same thing.

Notice that instead of a variable name, we specified the procedure name inside parentheses.
This is what makes `foo` a procedure. The one and only thing `foo` does, is that it returns
its name as a string.

> To be technically accurate, the procedure's name is really a variable. See the
[specification](../special-forms-derived-expressions/define.md) if you want to know more.

Let's evaluate the procedure name to see what we get.

```scheme
foo
```
-> *[UserProcedure] Parameters: 0*

Indeed, `foo` is a procedure. Now, let's call it by simply enclosing its name in parentheses.

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
-> *7*

Notice that after the procedure name `max`, we are specifying the parameter names `num1` and `num2`.
The body of the procedure contains an `if` expression that we have already covered.

Generally, the body of a procedure starts with zero or more definitions, followed by at least one
expression. The expressions are executed in sequence and the return value of the procedure is the
last expression's result.

Let's build on the previous example to see this in practice.

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

This time, when `max` is called, three local variables are defined.
We are using the primitive procedure [number-to-string](../primitives/number-to-string.md) to create
the string representations of the arguments and assign them to `num1-str` and `num2-str`. 
We are then creating a meaningful message and assign it to `log-message`.

After the definitions, the expressions are executed in turn. The log message and a new line are written to the standard
output. The final expression is the `if` whose result is returned.

> As you would expect, both parameters `num1`, `num2` and the local variables are only visible
within the procedure. Their scope is said to be local in `max` and they are not accessible outside of it,
e.g. in the global scope.

> As we talked about, local variables can only be defined in the beginning of the procedure's body.
This is not common in other LISP dialects, where you can define local variables anywhere within the body.

We can also define local procedures, also known as inner procedures, i.e. procedures within
other ones. As expected, such a procedure is only visible to the procedure it is declared in.
So, a local procedure's purpose is code reuse in the context of its enclosing procedure.

Let's use the previous example to illustrate this idea.

```scheme
(define (max num1 num2)

  (define (display-log)
    (display "Finding the max between ")
    (display num1)
    (display " and ")
    (display num2)
    (display ".")
    (newline))

  (display-log)

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

Above, we have defined the local procedure `display-log` which we call within `max`.

> Notice that `display-log` has access to the parameters of `max`. Similarly, it can access
other variables defined in the enclosing procedure. Even though, it is not mathematically accurate,
this is commonly referred to as a closure.

## Lambdas
Lambdas are anonymous procedures, i.e. common procedures that haven't been given a name.
You can declare one with the lambda keyword and enclose its zero or more parameters in parentheses.

Let's evaluate a lambda with no parameters that simply returns `1`.

```scheme
(lambda () 1)
```
-> *[UserProcedure] Parameters: 0*

As you can see, the result is a user procedure. You can assign it to a variable, call
it immediately, or pass it around as we will see shortly after.

Now, let's evaluate a lambda that is functionally equivalent to `max`.

```scheme
(lambda (num1 num2)
  (if (>= num1 num2)
      num1
      num2))
```
-> *[UserProcedure] Parameters: num1, num2*

Recall that in order to call a procedure, we simply wrap it and the arguments it might expect with parentheses.

```scheme
((lambda (num1 num2)
  (if (>= num1 num2)
      num1
      num2)) 4 7)
```
-> *7*

## Passing Around Procedures
Procedures can be assigned to variables, passed in as arguments or returned from a procedure.

> When procedures have such traits in a language, it is often said that they are first class citizens.

> Procedures that accept as arguments or return procedures, are also known as higher order functions.

Let's see an example.

```scheme
(define (pick-num-with predicate)
  (lambda (num1 num2)
    (if (predicate num1 num2)
        num1
        num2)))

(define max (pick-num-with >=))
(define min (pick-num-with <=))

(max 4 7)
(min 4 7)
```
->
```
7
4
```

Above, we have defined a procedure `pick-num-with` that accepts a predicate procedure as argument.
When called, it returns a lambda, i.e. a procedure which simply picks a number based on the given predicate.

We then call `pick-num-with` with the `>=` primitive procedure and assign the returned procedure to variable `max`.
We do the same with `<=` and `min` respectively and finally call the procedures.

> A procedure that is expected to return a boolean, is often called a predicate.

> Note how `pick-num-with`'s code is reused.

You have learned a lot! When you are ready, we can talk about [loops](loops.md).