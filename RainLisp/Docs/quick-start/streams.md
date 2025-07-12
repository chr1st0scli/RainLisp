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

## Creating & Accessing Streams Basics
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
that will evaluate `b`. Therefore, calling `car` on it gives 1.

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

Let's try to create a larger stream to get a grasp of what they look like.

```scheme
(define stream (cons-stream 1 (cons-stream 2 (cons-stream 3 (cons-stream 4 nil)))))
stream
```
-> *(1 . [UserProcedure] Parameters: 0)*

The stream in the above example is a pair made of `1` and a promise to give the rest of the stream.
Note that the rest of the stream is not yet created. `2`, `3` and `4` are not evaluated yet.
When we ask for the first promise to be fulfilled, the result will be `2` and a promise for the rest.

```scheme
(cdr-stream stream)
```
-> *(2 . [UserProcedure] Parameters: 0)*

When we ask for the second promise to be fulfilled, the result will be `3` along with a promise for the rest and so on.

```scheme
(cdr-stream (cdr-stream stream))
```
-> *(3 . [UserProcedure] Parameters: 0)*

As said before, when we just want `3` we can use `car` since streams are implemented with regular pairs.

```scheme
(car (cdr-stream (cdr-stream stream)))
```
-> *3*

Of course, it is preferable to use the procedure `at-stream` with the zero-based index of the element we wish for.

```scheme
(at-stream stream 2)
```
-> *3*

Also, instead of defining the stream as we did above like `(define stream (cons-stream 1 (cons-stream 2 (cons-stream 3 (cons-stream 4 nil)))))`,
we can simply use the procedure `make-range-stream` to easily create a numerical stream.

```scheme
(define stream (make-range-stream 1 4))
(at-stream stream 3)
```
-> *4*

## Custom & Infinite Streams
Now we can move to more interesting things. It's good to know we have `make-range-stream` to create a stream of consecutive numbers,
but how would we create a custom infinite stream to operate on? The key is to create a procedure that calls `cons-stream` recursively.

Let's create the Fibonacci sequence.

```scheme
(define (fib-gen a b)
    (cons-stream a (fib-gen b (+ a b))))

(define fibonacci (fib-gen 0 1))

(at-stream fibonacci 20)
```
-> *6765*

Notice that `fib-gen` is a procedure that creates the Fibonacci stream by creating a pair of `a` and a promise to call
itself recursively, so that our familiar `(cons-stream x (cons-stream y (cons-stream z ...)))` construct is created up to the
point dictated by the `at-stream` call.

Needless to say, every next `a` is the previous `b` and every next `b` is the sum of previous `a` and `b`, which is the Fibonacci sequence definition.
So, we start with 0 and 1 and we effectively create as much as we require, i.e. we force 20 promises to be fulfilled to get the 21st number
in the Fibonacci sequence which is 6765.

Next, we are catching our breath with something simpler, [message passing](message-passing.md).