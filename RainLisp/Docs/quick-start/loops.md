# Loops
Loops allow you to keep executing some code over and over again as long as a condition holds true.
It is an essential 

RainLisp, being a functional programming language, promotes recursion. It does not provide the typical
keywords that allow you to create loops such as `while`, `for` etcetera. You can achieve the same effect
with a recursive procedure, i.e. a procedure that calls itself!

> In fact, one could implement a `while` keyword as syntactic sugar for a recursive procedure.

## Recursion
Let's see a common example when someone is introduced to recursion. We will write a procedure that
finds the factorial of a number. For example the factorial of 5 is 5 * 4 * 3 * 2 * 1 = 120.

```scheme
(define (factorial num)
  (if (= num 1)
      1
      (* num (factorial (- num 1)))))

(factorial 5)
```
-> *120*

As you can see, `factorial` builds a chain of deferred operations, more particularly a chain of
multiplications. Try to follow along the code above, step by step and note the *exit* condition,
i.e. when `num` is `1`, we stop calling `factorial` and return `1`. You will notice that the process
goes something like that `(* 5 (* 4 (* 3 (* 2 (1)))))`. The interpreter builds this chain and starts
multiplying once it is complete. It multiplies `2` and `1` first, the result is multiplied with
`3` and so on, until we get to `5` and get the final result of `120`.

> This chain is actually a stack and the interpreter needs to store information to keep track of it.
Therefore, in circumstances that we know that the chain will grow excessively, if we choose this
approach we are looking for trouble.

> A computational process that builds a chain of deferred operations is often called recursive.

## Tail Recursion
Every procedure like the one above can be written differently, in a way that avoids having the stack
growing at each step. To achieve this, the procedure should not create a deferred operation, the last
thing it should do is call itself, hence the *tail* term. But if it doesn't create a deferred operation,
it means that it should somehow perform one and carry the result to the next step.
The carrier of an operation is a procedure parameter often called an `accumulator` in that it gradually
accumulates the final result.

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
Notice that the last thing `factorial` does is to call itself, it doesn't create a deferred operation
like so `(* num (factorial...`.

> A computational process that doesn't build a chain of deferred operations is often called iterative.

One design drawback is that `factorial` callers need to provide the initial value for `acc`.
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

> True tail recursion requires a low-level support. RainLisp is interpreted in the .NET
framework where a stack is always being built no matter which style you use. Though, it's
always good to know!

## Tree Recursion

Well done! You have added a lot to your arsenal! We can step off the gas a bit and talk about the [let](let.md) keyword.