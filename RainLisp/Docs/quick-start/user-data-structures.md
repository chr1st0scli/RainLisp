# User Data Structures
At some point, you will inevitably need to define your own structures that make sense
in the programs you write.

You can organize data using pairs in a variety of ways. You can use single pairs, group them
or nest them depending on how you see fit.

Usually, when you define your own data structures, you define one or more constructors that
create them and a set of accessors (getters) that extract the individual data pieces your
structure is made of.

Let's define a point in a two-dimensional space. Such a point is made of two numeric co-ordinates,
x and y.

```scheme
(define (make-point-2d x y)
  (cons x y))

(define pointA (make-point-2d 3 4))
pointA
```
-> *(3 . 4)*

Above, we defined a constructor called `make-point-2d` and called it to create `pointA` as a simple
pair.

Now, let's define some getters that know how to extract a point's constituent data.

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

The design idea behind this, is that the code that uses the constructor and accessors, does not
need to know the details of how a point is implemented. It should not care if a point is just a
simple pair or something else. All it needs, is a couple of procedures with these particular names.
That way, a data abstraction layer is created. We will take this idea further, later when we talk
about encapsulation.

> Recall that we mentioned procedural abstraction when we talked about loops. We can build more
abstraction layers on top of the data abstraction we built for two-dimensional points. For example, one can
build an abstraction layer (a group of procedures) that performs calculations on points, like finding the
distance between two. In a system, each abstraction layer can be replaced by another one, as long as it complies
to the same contracts. This improves the modularity and therefore the extendability and maintainability of the code
whose importance becomes even more apparent in larger systems.

We can also implement data validation in our constructors and accessors if we choose to do so.
Let's sieze the opportunity to introduce the `error` primitive procedure.

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

If the provided argument is not a pair, an exception is thrown with an appropriate message,
using the [error](../primitives/error.md) primitive procedure.

If your data structure becomes complex, you might want to use quote symbols to make sense out of them
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

This time we choose that a person is made of a list of pairs. Each pair is made of a person's
property (as a quote symbol) and its respective value. Notice that when the identifier `bob` is
evaluated, the interpreter prints a list that can be easily understood because of the quote symbols.

Finally, a call is made to get Bob's last name.

That's it! Well done for your progress. You are encouraged to practice everything you learned.
After that, it's really worth the effort to see a few more advanced topics, starting with [advanced data structures](advanced-data-structures.md),
that will take you RainLisp skills to the next level.