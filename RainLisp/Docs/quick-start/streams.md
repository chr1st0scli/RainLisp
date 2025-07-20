# Streams
We have seen list operations like `map` and `filter` and their elegance in expressing operations on lists.
But what if we want to operate on significantly large sets? Constructing a large list can be expensive both in
terms of memory and CPU, let alone using `map` on it which will create yet another one with the element projections. 
The amount of operations can be unbearable.

What about infinite sets? What if we want to operate on all natural numbers, or all prime numbers? How do we even
go about defining such a set, let alone operate on it?

Streams come to the rescue. You can imagine streams like delayed lists, ones that are being constructed on demand;
up to the point that is absolutely needed and not fully created up front.

Most programming languages have similar constructs dealing with this problem; C# uses iterator methods, Python uses generators
and the list goes on. There are different ways a language can implement this, C# implements it with state machines behind
the scenes. RainLisp, implements it with deferred procedures, i.e. procedures whose execution is postponed until explicitly
asked for. You might have heard this concept as promises in other languages.

Let's dive in.

## Deferred Evaluation
The basic building block for streams is the special form [delay](../special-forms-derived-expressions/delay.md), which delays the evaluation of an expression by putting it
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
->
```
1
1
```

> Note that the procedure created for the delayed expression is a memoized one. I.e. the expression is evaluated once when needed.
Subsequent calls to the same procedure will simply return the previously calculated value.
This is done because it is very common to end up with usage patterns that would result to too many unnecessary evaluations, as we will later see.

## Creating & Accessing Streams

### Basics
To create a stream, you can use [cons-stream](../special-forms-derived-expressions/cons-stream.md) which creates a pair of the first expression and a delayed second one.
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
that will evaluate `b` when called. Therefore, calling `car` on it gives 1.

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

Let's try to create a larger stream to get a grasp of what they look like. We are chaining `cons-stream` calls just as
we did with `cons` to make lists.

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

As said before, when we just want `3` we can use `car`, since streams are implemented with regular pairs.

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

### Custom & Infinite Streams
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
itself recursively, in a way that our familiar `(cons-stream x (cons-stream y (cons-stream z ...)))` construct is created up to the
point dictated by the `at-stream` call.

Needless to say, every next `a` is the previous `b` and every next `b` is the sum of previous `a` and `b`, which is the Fibonacci sequence definition.
So, we start with 0 and 1 and we effectively create as much as we require, i.e. we force 20 promises to be fulfilled to get the 21st number
in the Fibonacci sequence, which is 6765.

## Mapping & Filtering
As mentioned in the beginning of this section, we can combine the expressive elegance of map and filter operations with infinite sequences
using the respective stream-friendly procedures `map-stream` and `filter-stream`. These map and filter on demand, as requested
and they don't need the whole sequence to be built up front like their list equivalent cousins `map` and `filter`.

Let's see an example of how we can find the 101st integer that is divisable with 7 using filtering.

```scheme
(define (integers-starting-from n)
    (cons-stream n (integers-starting-from (+ n 1))))

(define integers (integers-starting-from 1))

(define (divisable? x y)
    (= 0 (% x y)))

(define integers-divisable-with-7
    (filter-stream (lambda(n) (divisable? n 7))
                   integers))

(at-stream integers-divisable-with-7 100)
```
-> *707*

We start by defining the integers stream as a procedure that builds consecutive delayed `cons-stream` calls. We also define the predicate that
can support the desired condition `divisable?`.

We then define the stream `integers-divisable-with-7` by calling `filter-stream` with the desired condition and the stream of integers.
Note that `filter-stream` is used just like `filter` but with streams.

Finally, we ask for the 101st number that satisfies the condition and get the result.

If you see the code of [filter-stream](../common-libraries/filter-stream.md), you will notice that it returns the first element that satisifes
a given condition and a promise to find the rest. This means that it keeps fulfilling promises, until it reaches the next element that satisfies the predicate.

Similarly, [map-stream](../common-libraries/map-stream.md) returns the projection of the first element of a stream and a promise to project the
rest of them on a need-to-know basis.

### Sieve of Eratosthenes
Let's see a more interesting filtering problem. Let's find the 51st prime number with the sieve of Eratosthenes (276 BC). A prime number is one that is exactly
divided, i.e. with no remainder, by itself and 1 alone.

```scheme
(define (integers-starting-from n)
    (cons-stream n (integers-starting-from (+ n 1))))

(define (divisible? x y)
    (= 0 (% x y)))

; Eratosthenes
(define (sieve stream)
    (cons-stream
        ; A prime number.
        (car stream)
        ; And a promise to sieve the next integers that are not divisable by the current prime number.
        ; Note that the next delayed call to sieve will incorporate the same delayed filtering for consecutive calls.
        ; It will result in a cascading style of excluding numbers that are divisable by the previously found prime numbers.
        (sieve (filter-stream 
                    (lambda (n)
                        (not (divisible? n (car stream))))
                    (cdr-stream stream)))))

(define primes (sieve (integers-starting-from 2)))

(at-stream primes 50)
```
-> *233*

The first prime number is 2, so we need to sieve the stream of integers starting from 2, i.e. 2, 3, 4, 5, 6, 7 and so on.

At each step, to find the next prime, we examine the next number that is not divisible by all previously found prime numbers.

- For the second one, we start from 3 and search for the first number that is not divisible by 2, which is 3.
- For the third one, we start from 4 and search for the first number that is not divisible by 2 and 3, which is 5.
- For the fourth one, we start from 6 and search for the first number that is not divisible by 2, 3 and 5, which is 7.
- For the fifth one, we start from 8 and search for the first number that is not divisible by 2, 3, 5 and 7, which is 11 and so on.

Note that the stream argument that is recursively fed to each sieve call is made in a way that contains consecutive filters that a prime
needs to satisfy up to that point. For example, the stream at the last step above contains:
1. The number 8 and a promise for the rest of the integers
2. that is fed to `filter-stream` to find the next number not divisible by 2
3. which is fed to `filter-stream` to find the next number not divisible by 3
4. which is fed to `filter-stream` to find the next number not divisible by 5
5. which is fed to `filter-stream` to find the next number not divisible by 7

If you change the lambda that is used with `filter-stream` as below to print what is checked on each step, it might help
you debug and understand the call stack described above.

```scheme
(lambda (n)
    (if (>= n 8)
        (begin
            (display (+ "checking " (number-to-string n "0") " with " (number-to-string (car stream) "0")))
            (newline)))
    (not (divisible? n (car stream))))
```

Now, if you run the code below to get the fifth prime number, when we reach 8, the program will start printing information.
8 is discarded because it is divisable by 2, so we move on to 9 that is not divisable by 2, but it is by 3. So, we move on to 10
that is discarded because it is divisable by 2 and move to 11 which is checked that is not divisable by 2, 3, 5 and 7 in turn.

```scheme
(at-stream primes 4)
```
->
```
checking 8 with 2
checking 9 with 2
checking 9 with 3
checking 10 with 2
checking 11 with 2
checking 11 with 3
checking 11 with 5
checking 11 with 7
11
```

## Implicit Streams
We call implicit streams the ones that are defined in terms of their own merit and not with procedures that generate elements
one by one like `fib-gen` and `integers-starting-from` that we have seen above.

Let's see an example, we want to define a stream of powers of two and find the fifth one which is 16.

```scheme
; Powers of two are 2^0, 2^1, 2^2, 2^3, 2^4... I.e. 1, 2, 4, 8, 16...
(define two-powers
    (cons-stream 1
                (map-stream (lambda (x) (* x 2)) 
                            two-powers)))

(at-stream two-powers 4)
```
-> *16*

It's quite difficult to try to debug this in your head by following the evolution of returned pairs and promises at each step.
But let's try to simplify things and give a general overview.

So, we define that `two-powers` is a stream made of 1 and a promise to double the `two-powers`.

```scheme
two-powers
```
-> *(1 . [UserProcedure] Parameters: 0)*

When we force the promise, e.g. by applying the first `cdr-stream` to `two-powers`, the `map-stream` is executed which gives us 1 * 2 = 2
and a promise to double the rest of `two-powers`.

```scheme
(cdr-stream two-powers)
```
-> *(2 . [UserProcedure] Parameters: 0)*

But by looking at its definition above, the rest of `two-powers` is itself a promise to double the `two-powers`. That way, consecutive doublings
are chained.

Effectively, when we ask for the 3rd power of 2, we are asking for the fulfillment of a promise that performs the following multiplications.
- 2 * (2 * ( 2 * 1))
- 2 * (2 * 2)
- 2 * 4
- 8

```scheme
(at-stream two-powers 3)
```
-> *8*

Notice that the memoized user procedures that are related to promises shine with implicit streams. Without memoization, the same multiplications
would be repeated many times. For example `(at-stream two-powers 3)` that gives 8 would repeat 
- 1 * 2, 3 times
- 2 * 2, 2 times
- 4 * 2, 1 time.

With memoization, each multiplication is performed only once because its result is cached. This is because,
`two-streams` is made in a way that chains a promise to double the result of another promise to double another result and so on.
At each step that we consume the stream with `at-stream`, the first part of the resulting pair, which is the result of the multiplication,
is never carried on to contribute to the next one. Instead, the same procedure representing each step is carried on to be executed again
for additional powers of two.

It's like magic, isn't it? Try to rehearse this in your head but don't worry if you find it overwhelming. Often, in functional
programming, people argue that you shouldn't need to explain every step, it's enough to see the overall picture and comprehend
the expression in a higher level.

Next, we are catching our breath with something simpler, [message passing](message-passing.md).