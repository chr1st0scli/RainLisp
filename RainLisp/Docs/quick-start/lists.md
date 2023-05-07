# Lists
A list is a sequence of zero or more values that can be of different types.
In order to create one, we can use pairs which we have seen in the previous section.

A list starts with a pair whose first element is the first value of the list and its second element
is the next pair which repeats this pattern. The second element of the last pair is `nil`, a special
primitive value that terminates a list which is also known as the empty list.

Let's see an example.

```scheme
(cons 1 (cons 2 (cons 3 (cons 4 (cons 5 nil)))))
```
-> *(1 2 3 4 5)*

As you can see, the string representation of a list is its elements one after the other, which are all
surrounded with parentheses.

There is a primitive procedure `list` that makes it easier for use to create lists. Let's see the
`list` expression that is equivalent to the previous one.

```scheme
(list 1 2 3 4 5)
```
-> *(1 2 3 4 5)*

Let's see how this list is represented in memory.

![list](img/list.png)

An interesting aspect of the list's structure is what we get if we apply `cdr` to it. Can you guess?

```scheme
(cdr (list 1 2 3 4 5))
```
-> *(2 3 4 5)*

We get the rest of the list which is still a list! This is because of the repetitive nature of the list's
internal structure that we investigated above.

As mentioned above, `nil` is the empty list.

```scheme
nil
```
-> *()*

It is equivalent to the following expression.

```scheme
(list)
```
-> *()*

Another very common way to declare an empty list is to use the alternative quote notation like this.

```scheme
'()
```
-> *()*

> A very useful primitive procedure is `null?`. It helps us determine if a list is empty or not.
Of course, one could use the `=` and `nil` primitives like so `(= '() nil)` or simply use the
`null?` procedure like so `(null? '())`.

```scheme
(null? '())
```
-> *true*

Let's put all this together to see a typical way that someone iterates over a list and performs some
actions on its elements. Let's write a `for-each` procedure that processes a list's elements
one by one.

```scheme
(define (for-each proc sequence)
  (if (not (null? sequence))
      (begin 
        (proc (car sequence))
        (for-each proc (cdr sequence)))))

(for-each display (list 1 2 3 4 5))
```
-> *12345*

The procedure above accepts a procedure `proc` to apply to every element of the list `sequence`.
While we have not reached the end of the list `(not (null? sequence))`, we apply `proc` to the
first element and then call `for-each` again passing `proc` again but the rest of the list this time
`(for-each proc (cdr sequence))`. The procedure that we pass to `for-each` is the primitive
`display` procedure that prints its argument to the standard output.

Try to follow along the code on every step to understand how combining `null?`, `car`, `cdr` and
recursion, effectively allow us to iterate over the list's elements.

Next, let's see some library procedures that implement some common [list operations](list-operations.md)
in the functional languages domain.
