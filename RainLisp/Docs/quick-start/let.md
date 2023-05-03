# Let Code Block
The `let` keyword defines a block of code in a scope or context if you will, where some variables are only visible
within it.

Let's see an example.

```scheme
(let ((a 1) (b 2))
  (+ a b))
```
-> *3*

We have easily defined two variables `a` and `b` in one line and some code that uses them. The two variables
are only visible within the let expression.

Let's see another one.

```scheme
(define (get-length) 10)
(define (get-height) 22)
(define (get-breadth) 45)

(let ((x (get-length))
      (y (get-height))
      (z (get-breadth)))
  (define product (* x y z))
  (debug "The volume of the cuboid is: ")
  (debug product)
  product)
```
-> *9900*

We define three procedures that supposedly give the dimensions of a cuboid.
We then use a `let` expression where we call the procedures and assign the results to three variables
`x`, `y` and `z`, representing the cuboid's dimensions. Then, in the `let` body, we define yet another variable
that is the product of the former three.

The expressions in the body are executed in turn. Firstly, we are using the [debug](../primitives/debug.md)
primitive procedure to write a log message. The last expression gives the result of the `let` itself.

> Like in a procedure, you can only use the `define` keyword at the start of the `let`'s body.
This is because `let` is syntactic sugar for a procedure call. If you want to see more
details about this, see the technical [specification](../special-forms-derived-expressions/let.md).

Now, let's end the language basics with [quotes](quotes.md).