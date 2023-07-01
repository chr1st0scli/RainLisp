# Encapsulation a la Functional
Even though, immutability is a desired trait in many cases, there are problems in the real world that are better and/or more
easily modeled with mutability. I.e. state that can change. But when it comes to that, we will probably need a controlled way of doing so.

Encapsulation is the bundling of methods and data together and prohibiting the direct and uncontrolled access to that data.

If you are familiar with object oriented programming, you already knwow that classes are the main means of achieving it.

But what happens in a functional programming language and particularly in a LISP dialect such as RainLisp, that doesn't have the
notion of classes?

As mentioned before, the main means of abstraction in RainLisp are procedures. We can take advantage of the fact that they are
"first class citizens" and obscure the boundaries between procedures and data. We can emulate a class and therefore the controlled
access to its internal details.

Let's create a bank account abstraction.

```scheme
(define (make-account amount)

  ; Data.
  (define balance amount)

  ; Local procedures that operate on data.
  (define (deposit amount)
    (set! balance (+ balance amount)))

  (define (withdraw amount)
    (if (< balance amount)
        (error "Insufficient funds.")
        (set! balance (- balance amount))))

  (define (get-balance)
    balance)

  ; A lambda is returned that captures and gives access to the internal procedures.
  (lambda (op)
    (cond ((= op 'deposit) deposit)
          ((= op 'withdraw) withdraw)
          ((= op 'balance) get-balance)
          (else (error "Unknown operation.")))))

; Optional interface procedures that allow us to operate on different accounts and hide the message passing details.
(define (get-balance account)
  ((account 'balance)))

(define (withdraw account amount)
  ((account 'withdraw) amount))

(define (deposit account amount)
  ((account 'deposit) amount))

; Ready to try it! Create two accounts and make transactions.
(define lisa-account (make-account 300))
(define bob-account (make-account 320))

(withdraw lisa-account 100)
(withdraw bob-account 200)
(deposit lisa-account 42.50)

(get-balance lisa-account)
(get-balance bob-account)
```
->
```
242.5
120
```

Above, we define a `make-account` procedure that accepts the initial amount for opening a bank account.
Within it, we define the `balance` local variable and some procedures that implement common bank account operations,
such as `deposit` and `withdraw`. So, `balance` is the data, the internal state that we want to give controlled access to.

The access to data is given via the procedures, but all of them are local ones. How do we make them available to callers?
The procedure `make-account` returns a lambda that captures (remember closures?) the account's internals and uses message passing
to give access to the operations.

> Needless to say, not all procedures must become publicly available. If we wanted to emulate an object oriented class's *private* method
that would only be reusable within `make-account`, we would simply not expose it in the lambda.

Finally, outside of `make-account`, we define some helper procedures that allow us to operate on different accounts and
hide the message passing technique from the callers.

> Pay attention to the mental shift. Accounts can now be seen as objects, when they are really only procedures.

In the end, two accounts are created and we perform some operations on them.

Next, let's extend the ideas in this section and talk about [data directed programming](data-directed-programming.md).
