# More Looping Techniques
Remember when we saw various ways of looping? We have seen recursive processes that increase the stack but look
elegant. We have also seen iterative processes (tail recursion) that do not increase the stack.

Let's investigate a couple of other common techniques that may come in handy.

## Deferred Procedures
We can create and call a procedure that creates another procedure to be called later. The goal is that we want to
make some pre-processing for defining an algorithm just once and possibly execute it multiple times later.
In scenarios that such pre-processing is realistic, we can avoid repeating it every time we want to apply an
algorithm.

Even though a factorial calculation might not be a realistic example, we can use it to illustrate the idea.

We will successively create a procedure within another procedure multiple times. The resulting procedure will be
like a babushka, containing other smaller ones within it. When the outer procedure is called, our babushka will start opening up until we get the final result.

First, let's remember the factorial example, defined as an iterative process (tail-recursive).

```scheme
(define (factorial number)
  (define (iter num acc)
    (if (= num 1)
        acc
        (iter (- num 1) (* num acc))))

  (iter number 1))
```

Now let's redefine it in a way that it returns a user procedure.

```scheme
(define (factorial number)
  (define (iter num proc)
    (if (= num 1)
        proc
        (iter (- num 1) (lambda () (* num (proc))))))

  (iter number (lambda () 1)))

(define factorial5 (factorial 5))
(factorial5)
```
-> *120*

If you compare the two versions, you will notice that the accumulator `acc` is not a number any more, but a
procedure named `proc`. Also, notice that instead of immediately executing the multiplication `(* num acc)`,
we siply defer it by enclosing it in a lambda `(lambda () (* num (proc)))`.

In the example above, we create a procedure that knows how to calculate the factorial of `5`. We assign it
to the `factorial5` variable and when we call it, the enclosed lambdas start being called one after the other.

Try to follow along the code. You will see that the computation process goes something like that:

```scheme
(define factorial5
  (lambda () (* 2
    ((lambda () (* 3
      ((lambda () (* 4
        ((lambda () (* 5
          ((lambda () 1))))))))))))))

(factorial5)
```
-> *120*

The multiplications, expressed in infix notation, occur in the following order.

```
5 * 1
4 * 5
3 * 20
2 * 60
```

## Continuation Passing Style
Another well known technique is CPS, which stands for continuation passing style.
The idea is that each step in an algorithm produces a result, which is passed to a continuation procedure that
embodies the next step. That way, program flow is more explicit.

In order to apply CPS, every procedure should accept an extra argument, a continuation.
When a procedure calculates a result, it shouldn't return it to the caller, but instead pass it to the continuation
and return whatever that returns.

First, let's remember the factorial example, defined as a recursive process.

```scheme
(define (factorial num)
  (if (= num 1)
      1
      (* num (factorial (- num 1)))))
```

Notice that there are three main operations. An equality check, a multiplication and a subtraction.
So, let's define these operations in CPS style.

```scheme
(define (=c n1 n2 proc)
  (proc (= n1 n2)))

(define (-c n1 n2 proc)
  (proc (- n1 n2)))

(define (*c n1 n2 proc)
  (proc (* n1 n2)))
```

Now, with these helper procedures at hand and the original recursive example as a reference, we can try to define `factorial` in CPS style.
Once again, a nested lambda is created, but it's called immediately.

```scheme
(define (=c n1 n2 proc)
  (proc (= n1 n2)))

(define (-c n1 n2 proc)
  (proc (- n1 n2)))

(define (*c n1 n2 proc)
  (proc (* n1 n2)))

(define (factorial num)
  (define (iter-cps n proc)
    (=c n 1
        (lambda (exit)
          (if exit
              (proc 1)
              (-c n 1
                  (lambda (prev-num)
                    (iter-cps prev-num
                              (lambda (prev-num-fact)
                                (*c n prev-num-fact proc)))))))))
  (iter-cps num (lambda (x) x)))

(factorial 5)
```
-> *120*

The local (or inner) procedure `iter-cps` is itself written in CPS style; note its continuation parameter `proc`.
We are declaring it as a local procedure to hide this detail from the caller and simplify its usage.

Let's go through it and explain how it can be read.

First, we check if `n` is equal to `1` and the result is saved in the `exit` parameter of the lambda which is the next step.

Then, if we must exit, we call `(proc 1)`. Compare that with the original recursive version where `1` is immediately returned.
Recall the rule that a CPS procedure shouldn't immediately return a result, but rather pass it to a continuation.

Otherwise, if we shouldn't exit, we decrement `n` by `1` and the result is saved in the `prev-num` parameter of the last step.

> One thing to notice about CPS is that the order in which code is written, reflects the actual evaluation order.
Indeed, if you consider the line `(* num (factorial (- num 1)))` in the original recursive version, you will realize that the
subtraction occurs first, then the call to `factorial` and finally the multiplication.

The tricky part is the final step which iteratively calls the inner procedure `iter-cps`. The argument that is passed
to `proc` is a new deferred lambda that performs a multiplication and passes the product to the current `proc`'s value.

This is the point where successive continuations are chained together to form a nested lambda, which will be ultimately
called when `(proc 1)` is executed.

> Notice the first continuation we pass to `iter-cps`, i.e. `(lambda (x) x)`. It will be the last one called, when we arrive
at the final result. It's a procedure that simply returns the passed argument unchanged and that's why it is commonly known
as the identity function.

It might seem intimidating, but if you try to follow along the code, you will see that what happens goes something like that:

```scheme
(define (*c n1 n2 proc)
  (proc (* n1 n2)))

((lambda (prev-num-fact) 
   (*c 2 prev-num-fact
       (lambda (prev-num-fact) 
         (*c 3 prev-num-fact
             (lambda (prev-num-fact) 
               (*c 4 prev-num-fact
                   (lambda (prev-num-fact)
                     (*c 5 prev-num-fact
                         (lambda (x) x))))))))) 1)
```
-> *120*

The multiplications, expressed in infix notation, occur in the following order.

```
2 * 1
3 * 2
4 * 6
5 * 24
```

Congratulations! You have completed the quick start guide, which might not have been that quick after all! :-)
