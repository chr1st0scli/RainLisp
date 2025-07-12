# Streams
We have seen list operations like `map` and `filter` and their elegance in expressing operations on lists.
But what if we want to operate on a significantly large set? Constructing such a large list is expensive both in
terms of memory and CPU in the first place. Using `map` on it will create an equally large list and `filter` might
also create a large list depending on the condition it is provided with. The amount of operations can be tremendous.

What about infinite sets? What if we want to operate on all natural numbers, or all prime numbers. How do we even
define such a set, let alone operate on it?

Streams come to the rescue. You can imagine streams like delayed lists, ones that are being constructed up to the point
that is absolutely needed and not fully created up front.

Most programming languages have similar constructs dealing with this problem, C# uses iterator methods, Python uses generators
and the list goes on. There are different ways a language can implement this, C# implements it with state machines behind
the scenes. RainLisp, implements it with deferred procedures, i.e. procedures whose execution is postponed until explicitly
asked for.

Let's dive in.

## Deferred Evaluation
The basic building block for streams is the special form `delay`, which delays the evaluation of an expression by putting it
inside a procedure to be called later. Note that based on the language's rules `(foo a)` would first evaluate the argument `a`
and then call the procedure `foo` on it. But `(delay a)`, being a special form, doesn't evaluate `a` before calling
`delay` on it, it is translated to `(lambda() a)`.

So, how do we evaluate `a`? Since, `delay` returns a user procedure, we can simply call it by wrapping it in parentheses
or call `force` on it to force its evaluation.

```scheme
(define a 1)
(define delayed-a (delay a))

(delayed-a)        ; Call it directly.
(force delayed-a)  ; Use force.
```

> Note that the procedure created for the delayed expression is a memoized one. I.e. the expression is evaluated once when needed and subsequent calls will simply return the previously calculated value.
This is done because it is very common to end up with usage patterns that would make too many unnecessary evaluations as we will later see.

## Streams
To create a stream, you can use `cons-stream` which creates a pair of the first expression and a delayed second one.
So, `(cons-stream a b)` is syntactic sugar for `(cons a (delay b))`.

Let's see an example.
```scheme
(define a 1)
(define b 2)

(define stream (cons-stream a b))
stream
```
-> *(1 . [UserProcedure] Parameters: 0)*

By evaluating `stream`, we notice that it is just a pair of 1 and a delayed expression, i.e. a procedure
that will evaluate `b`.

Therefore, calling `car` on it gives 1.
```scheme
(car stream)
```
-> *1*

You should be able to come up with ways for evaluating `b`, but you might be glad to know that there is a procedure
called `cdr-stream` that does it for you, i.e. `(cdr-stream stream)` calls `(force (cdr stream))`.

```scheme
(cdr-stream stream)
```
-> *2*

Next, we are catching our breath with something simpler, [message passing](message-passing.md).