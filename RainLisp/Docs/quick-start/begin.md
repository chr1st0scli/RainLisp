# Begin Code Block
With `begin`, you can define a group of expressions to be evaluated in sequence.
Its result is the last expression's result.

```scheme
(begin
  (display 1)
  (display 2)
  (display 3))
```
-> *123*

This example writes `1`, `2` and `3` consecutively to the standard output.

Let's see another one.

```scheme
(+ 1
   (begin
     (display "About to return 3 which will be added to 1.")
     (newline)
     3))
```
->
```
About to return 3 which will be added to 1.
4
```

As you can see, `begin` writes a string to the standard output and evaluates to `3`,
which is then added to `1` giving `4`.

You may want to see more examples of [begin](../special-forms-derived-expressions/begin.md).

Next, let's dive deeper with [conditionals](conditionals.md).