# User Data Structures
At some point, you will inevitably need to organize data in a way that makes sense
in the programs you write.

You can do so, by using pairs in a variety of ways. You can use single pairs, group
or nest them, depending on how you see fit.

In order to create a data structure, you define a set of procedures.
There can be one or more constructor procedures that are responsible for creating a "data record"
and a set of accessor (getter) procedures that give access to the individual data pieces your
record is made of.

Let's define a point in a two-dimensional space. Such a point is made of two numeric co-ordinates,
x and y.

```scheme
(define (make-point-2d x y)
  (cons x y))

(define pointA (make-point-2d 3 4))
pointA
```
-> *(3 . 4)*

Above, we defined a constructor called `make-point-2d` and called it to create `pointA`.
Note that the concrete implementation of a point is a simple pair.

Now, let's continue from the previous code and define some getters that know how to extract
a point's co-ordinates.

```scheme
(define (get-x point)
  (car point))

(define (get-y point)
  (cdr point))

(get-x pointA)
(get-y pointA)
```
->
```
3
4
```

The set of constructor and accessor procedures, effectively create a data abstraction layer. 

The design idea behind this, is that the code that uses the constructor and accessors, does not
need to know the details of how a point is implemented. It should not care if a point is just a
simple pair or anything else. All it needs, is a set of procedures with particular signatures.

> In RainLisp, a procedure's signature, also known as a declaration, is merely the name of the procedure
and the number of its parameters. For what is worth, in a typical statically typed programming language,
this also includes the parameters' and return value's types.

> A procedure's definition is its body. I.e. the code that implements it.

Recall that we mentioned procedural abstraction when we talked about loops. We can build more
abstraction layers on top of the data abstraction we built for two-dimensional points. For example, one can
create a layer made of procedures that perform calculations on points, like finding the distance between two.

> In a system, each abstraction layer can be replaced by another one, as long as it complies to the same contract (usage rules).
This improves the modularity and therefore the extendability and maintainability of the code, whose importance
becomes even more apparent in larger systems.

We can also implement data validation in our constructors and accessors, if we choose to do so.
Let's seize this opportunity to introduce the `error` primitive procedure.

```scheme
(define (get-x point)
  (if (not (pair? point))
      (error "Invalid point.")
      (car point)))

(get-x 0)
```
->
```
User error: Invalid point.
Call Stack
[Application error] Line 3, position 8.
[Application get-x] Line 6, position 2.
Invalid point.
```

When `get-x` is called, if the provided argument is not a pair, an exception is thrown with an appropriate message.
This is done using the [error](../primitives/error.md) primitive procedure.

If your data structure becomes more complex, you might want to use quote symbols to make sense out of them
more easily. Let's create a data structure that represents a person and a single accessor procedure that
retrieves their last name.

```scheme
(define (make-person first-name last-name birthday)
  (list
    (cons 'first-name first-name)
    (cons 'last-name last-name)
    (cons 'birthday birthday)))

(define bob (make-person "Bob" "Fictionados" (make-date 1945 7 31)))

bob

(define (get-last-name person)
  (cdadr person))

(get-last-name bob)
```
->
```
((first-name . "Bob") (last-name . "Fictionados") (birthday . 1945-07-31 00:00:00.000))
"Fictionados"
```

This time, we choose that a person is made of a list of pairs. Each pair is made of a person's
property (as a quote symbol) and its respective value. Notice that when the identifier `bob` is
evaluated, the interpreter prints a list that can be easily understood because of the quote symbols.

Finally, a call to `get-last-name` is made to get Bob's last name.

That's it! Well done for your progress. You are encouraged to practice everything you learned.
After that, it's really worth the effort to see a few more advanced topics, starting with [advanced data structures](advanced-data-structures.md),
that will take you RainLisp skills to the next level!
