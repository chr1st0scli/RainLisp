# Variables
It's time to learn how to define and change variables.

## Definition
A variable is defined with the [define](../special-forms-derived-expressions/define.md) keyword.

Let's define one.

```scheme
(define my-var 4)
```

We have defined a variable named `my-var` in the global scope and assigned the value `4` to it.

> Note that in LISP dialects, it is common to use kebab case when naming variables and procedures,
i.e. replacing spaces between words with a dash `-`. This is just a convention which you might want to adopt or not.

Now, let's evaluate the variable and see what is returned.

```scheme
my-var
```
-> *4*

The interpreter looks up the variable name and returns its value.

If you define the same variable again, you simply redefine it; you don't get an error.

```scheme
(define my-var 5)
my-var
```
-> *5*

> You have a lot of freedom in choosing names for your variables, but they can't start with a number or use a reserved keyword.

> A variable is defined in the current scope. In the above examples, we defined the variable `my-var`
in the top-level or global scope.

> When in the top-level scope, you can define variables anywhere. This is not true when defining local
variables, i.e. variables defined inside procedures. But, we will talk about this later when we learn about procedures.

## Assignment
A variable is changed with the [set!](../special-forms-derived-expressions/set!.md) keyword.

> Note that in LISP, it is a convention to end a procedure name with a bang `!` when it changes
some state. Value changing is often called mutation.

Let's change the value of `my-var`.

```scheme
(set! my-var true)
```

We changed `my-var` from `5` to `true`. Let's evaluate the variable and see what we get.

```scheme
my-var
```
-> *true*

If the variable is not visible in the current scope, we get an error.

```scheme
(set! non-existing-var 1)
```
->
```
Unknown identifier non-existing-var.
Call Stack
[Assignment non-existing-var] Line 1, position 2.
```

Before going any deeper, it's a good time to talk about [comment](comment.md).