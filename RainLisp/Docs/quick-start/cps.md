# Continuation Passing Style
Remember when we saw various ways of looping? We have seen how we can create a recursive process
that seems mathematically elegant and increases the stack.

We have also seen iterative processes (tail recursion) that does not increase the stack.

We can now see another technique which is known as CPS and stands for continuation passing style.
This is based on higher-order procedures, i.e. procedures that accept other procedures as arguments.
The idea is that you pass a procedure as argument which defines what happens next.

Let's use the factorial example once again. This time, we will successively create a procedure within another
procedure multiple times. The resulting procedure will be like a babushka, containing other smaller ones
within it. When the outer procedure is called, our babushka will start opening up.

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

Try to follow along the code. You will see that our babushka factorial procedure is something like
that.

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
