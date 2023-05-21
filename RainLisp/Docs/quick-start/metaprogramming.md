# Metaprogramming
Have you ever wondered if it is possible to write a program that can write another one?

The answer is yes. We are going to investigate yet another aspect of quote symbols, that enables us to generate code and use
the [eval](../primitives/eval.md) primitive procedure to evalute it.

We will use an example that might not be realistic or make a lot of sense, just for illustration purposes.
Let's define a procedure that creates code that counts up to a given number.

```scheme
; Procedure that builds code that counts.
(define (build-counting-code count)
  (define (iter quote-list cnt)
    (if (= cnt count)
        (append quote-list '(a)) ; In the end, return a.
        (iter (append quote-list '((set! a (+ a 1)))) (+ cnt 1)))) ; Append successive assignments to a, incremented by one.

  ; Start with a lambda that defines variable a that is set to 0.
  (iter '(lambda () (define a 0)) 0))

(define code (build-counting-code 4))

; A list of quote symbols.
code

; Evaluating code, gives a lambda as defined above.
(define count-proc (eval code))

(count-proc)
```
->
```
(lambda () (define a 0) (set! a (+ a 1)) (set! a (+ a 1)) (set! a (+ a 1)) (set! a (+ a 1)) a)
4
```

We call the procedure `build-counting-code` with `4` as an argument. As a result, the source code of a lambda
that counts up to four, is generated and assigned to `code`. This is in the form of a list of quote symbols.

We then provide this list to the `eval` primitive procedure and the code is executed, thus creating a user procedure
which is assigned to `count-proc`.

Finally, we execute it and enjoy the fruits of our labor.

Let's move on to the final section and see some more [looping techniques](cps.md).
