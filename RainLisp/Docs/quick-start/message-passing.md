# Message Passing
Suppose that you want to write a procedure that does different things based on a criterion. That criterion is an argument passed
to the procedure and can be seen as a message, hence the message passing term.

Assume that we want to define a procedure that squares the result of a numeric operation on two numbers.

```scheme
(define (op-squared op x y)

  (define (square n) 
    (* n n))

  (define proc (cond ((= op '+) +)
                     ((= op '-) -)
                     ((= op '*) *)
                     ((= op '/) /)
                     (else (error "Unsupported operation."))))

  (square (proc x y)))

(op-squared '+ 8 4)
(op-squared '- 8 4)
(op-squared '* 8 4)
(op-squared '/ 8 4)
```
->
```
144
16
1024
4
```

Above, we define the local `square` procedure that squares a number. We also define a local variable `proc` which we assign a numeric primitive procedure to,
based on the message `op` we receive from the caller. Then, we apply the correct primitive procedure to `x` and `y` and square the result to get the final one.

> Quote symbols are a perfect match for message passing, because they are descriptive and fast to compare. Recall that when quote
symbols are tested for equality, only their memory addresses are compared.

We will later take this technique further and see how it can be used in other disciplines, like encapsulation and data directed programming.

Let's move on to [encapsulation](encapsulation.md) for now.