# Loops
Loops allow you to keep executing some code over and over again as long as a condition holds true.
It is an essential asset in every programming language.

RainLisp, being a functional programming language, promotes recursion. It does not provide the typical
keywords that allow you to create loops, such as `while`, `for`, etcetera. You can achieve the same effect
with a recursive procedure, i.e. a procedure that calls itself!

> In fact, a language designer could implement a `while` keyword as syntactic sugar and use a recursive procedure behind the scenes.

## Recursion
Let's see a common example that is used as introduction to recursion. We will write a procedure that
finds the factorial of a number. For example the factorial of 5 is 5 * 4 * 3 * 2 * 1 = 120. Notice that
the factorial of 5 is 5 multiplied by the factorial of 4.

```scheme
(define (factorial num)
  (if (= num 1)
      1
      (* num (factorial (- num 1)))))

(factorial 5)
```
-> *120*

As you can see, `factorial` builds a chain of deferred operations, more particularly a chain of
multiplications. Try to follow along the code step by step. Note the *exit* condition,
i.e. when `num` is `1`, we stop calling `factorial` and return `1`.

You will notice that the process goes something like that `(* 5 (* 4 (* 3 (* 2 (1)))))`.
The interpreter builds this chain and starts multiplying once it is complete. First, it
multiplies `2` and `1`; the result is then multiplied with `3` and so on, until we get to `5`
and conclude with the result of `120`.

> This chain is actually a stack and the interpreter needs to store information to keep track of it.
Therefore, in circumstances that we know that the chain will grow excessively, this approach will
probably mean trouble. We might come face to face with the infamous stack overflow error.

> A computational process that builds a chain of deferred operations is often called recursive.

## Tail Recursion
Every procedure like the one above, can be written in a way that avoids having the stack
growing at each step. To achieve this, the procedure should not create a deferred operation. Instead,
it should call itself as the last operation of every step, hence the *tail* term.

But if it doesn't create a deferred operation, it means that it should somehow perform one and carry the result to the next step.
The carrier of an operation's result is a procedure parameter. It is often called an `accumulator` because it gradually
accumulates, until it reaches the final result.

Let's see this in action.

```scheme
(define (factorial num acc)
  (if (= num 1)
      acc
      (factorial (- num 1) (* num acc))))

(factorial 5 1)
```
-> *120*

Note that this time, the *exit* condition returns the accumulator instead of `1`. Try to follow along
the code above. You will notice that the process goes something like that `(* (* (* (* 5 1) 4) 3) 2)`.
Notice that the last thing `factorial` does on each step, is to call itself. It doesn't create a deferred
operation like so `(* num (factorial...`.

> A computational process that doesn't build a chain of deferred operations is often called iterative.

One design drawback in the code above, is that `factorial`'s callers need to provide the initial value for `acc`.
Let's fix this based on what we have learned in the previous section.

```scheme
(define (factorial number)
  (define (iter num acc)
    (if (= num 1)
        acc
        (iter (- num 1) (* num acc))))

  (iter number 1))

(factorial 5)
```
-> *120*

As you can see, we renamed the previous `factorial` to `iter` and made it a local procedure.
Now, callers only need to provide the number they want to calculate the factorial for.

Notice, that its usage has now become the same with `factorial` from the previous section.

> This is our first glimpse of abstraction in a procedural level. The point is that callers do not
need to know about `iter`, which can be seen as an internal detail of `factorial`. They only need
to know something more abstract, the name of the procedure, i.e. `factorial`, the fact that it
accepts a single numeric argument and what it does but not how. In fact, we could use this version of
`factorial`, or the one from the previous section. The client code could work with either, with no change at all.

> True tail recursion requires low-level support. RainLisp is interpreted in the .NET
framework where a stack is always being built, no matter which style you use. Though, it's
always good to know!

## Tree Recursion
Another common technique is tree recursion. Let's tackle with a common problem that can be solved with it.

We need to find the nth Fibonacci number. Each number in the Fibonacci sequence is the sum of the two
previous numbers. I.e. `0 1 1 2 3 5 8 13 21 34...`

```scheme
(define (fibonacci n)
  (cond ((= n 0) 0)
        ((= n 1) 1)
        (else (+ (fibonacci (- n 1))
                 (fibonacci (- n 2))))))

(fibonacci 9)
```
-> *34*

This is tree recursion because the process splits into two on each step. You can easily
distinguish this because the procedure calls itself twice every time. Also, note that a
stack is being built, just like in the recursive process we have seen at the start of this section.

We can rewrite the procedure as an equivalent iterative process that doesn't increase the stack
and completes in considerably less steps.

```scheme
(define (fibonacci n)
  (define (iter n1 n2 index)
    (if (= index n)
        n1
        (iter n2 (+ n1 n2) (+ index 1))))

  (iter 0 1 0))

(fibonacci 9)
```
-> *34*

If you want a more in-depth analysis of recursion, have a look at the respective
[section](https://mitp-content-server.mit.edu/books/content/sectbyfn/books_pres_0/6515/sicp.zip/full-text/book/book-Z-H-11.html#%_sec_1.2) of SICP.

Well done! You have added a lot to your arsenal! We can step off the gas a bit and talk about the [let](let.md) keyword.