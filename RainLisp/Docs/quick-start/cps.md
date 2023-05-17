# More Looping (Still in progress)
Remember when we saw various ways of looping? We have seen how we can create a recursive process
that seems mathematically elegant and increases the stack.

We have also seen iterative processes (tail recursion) that do not increase the stack.

## Create Deferred Procedure
We can create a procedure that creates another procedure. The goal is that we want to make some processing
for defining an algorithm but not execute it immediately.

Let's use the factorial example once again. This time, we will successively create a procedure within another
procedure multiple times. The resulting procedure will be like a babushka, containing other smaller ones
within it. When the outer procedure is called, our babushka will start opening up until we get the final result.

```scheme
(define (factorial n)

  (define (iter num proc)
    (if (= num 1)
        proc
        (iter (- num 1) (lambda () (* num (proc))))))

  (iter n (lambda () 1)))

(define factorial5 (factorial 5))
(factorial5)
```
-> *120*

This time, we created the `factorial` procedure which creates and returns another procedure that calculates
the factorial of `5`. We assign that procedure to the `factorial5` variable.

When we call it, the enclosed lambdas start being called.

Try to follow along the code. You will see that the computation process is something like the following code.

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

```
5*1
4*5
3*20
2*60
```

> By doing this, we effectively created a procedure `factorial5` which encapsulates the algorithm for computing the factorial of
5. The algorithm has been analyzed and encapsulated in `factorial5`. So, this technique is very useful if you need to perform
some kind of an algorithm analysis but actually call it later. Theoretically, we could perform a required analysis step just once
and possibly use (call) it multiple times without the need to perform the analysis over and over again.

## Continuation Passing Style
Another well known technique is CPS which stands for continuation passing style.
The idea is that each next step in an algorithm is performed by another procedure, called continuation.
It's the one that continues with the processing of the previous step's result.

In order to implement this, every procedure should accept an extra argument, a continuation.
When a procedure calculates a result, it shouldn't return it to the caller, but instead pass it to the continuation.

Let's start by laying down the factorial example we defined for the recursive process.

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

We define `=c` that checks if two numbers are equal and pass the result to the continuation `proc` and its result is returned.
Similarly, `-c` subtracts two numbers and pass the difference to `proc` and `*c` multiplies two numbers and passes the product to `proc`.

Now with these helper procedures at hand and the original factorial procedure as a reference, we can try to define factorial in CPS style.
Let's see the complete program.

```scheme
(define (=c n1 n2 proc)
  (proc (= n1 n2)))

(define (-c n1 n2 proc)
  (proc (- n1 n2)))

(define (*c n1 n2 proc)
  (proc (* n1 n2)))

(define (factorial num)
  (define (iter n proc)
    (=c n 1
        (lambda (exit)
          (if exit
              (proc 1)
              (-c n 1
                  (lambda (prev-num)
                    (iter prev-num
                          (lambda (prev-num-fact)
                            (*c n prev-num-fact proc)))))))))
  (iter num (lambda (x) x)))

(factorial 5)
```
-> *120*

This is read like this:
First we check if `n` is equal to `1` and the result is saved in the `exit` parameter of the lambda which is the next step.
If we are meant to exit, we call the `proc` continuation with `1` in an analogous way to the original factorial procedure.
If we shouldn't exit, we decrement `n` by `1` and the result is saved in the `prev-num` parameter of the next step.

> One thing to notice about CPS is that it is more explicit in the order things happen. In other words, the order in which code
is written reflects the actual evaluation order in the original factorial example.

The tricky part is the final continuation which iteratively calls the inner procedure `iter` but the argument that is passed
to `proc` is a new lambda that performs the multiplication and passes the result to the previous `proc`'s value.

This will effectivey build a nested lambda structure that will unfold when we call `(proc 1)` when `exit` is true.

> Notice the first continuation we pass `(iter num (lambda (x) x))`. This is a procedure that simply returns the passed argument.
It is commonly known as the identity function. It will be the last one called, when we arrive at the final result.

It might seem intimidating, but if you try to follow along the code, you will see that what happens goes something like that.

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

```
2 * 1
3 * 2
4 * 6
5 * 24
```