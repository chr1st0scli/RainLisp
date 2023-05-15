# List Operations
There are some operations on sets of data that are very common in all languages with
functional programming characteristics. Let's see some of them and how they are typically used.

## Mapping
Mapping projects elements of a list in a certain way. In other words, you can use it to convert a set
of data, based on a conversion rule.

Let's suppose that we have a list of the first 5 natural numbers and we want to get a new one
with each incremented by 10.

```scheme
(map
  (lambda (x) (+ x 10))
  (list 1 2 3 4 5))
```
-> *(11 12 13 14 15)*

The first argument to map is the conversion procedure. It's a lambda that adds `10` to its parameter `x`.
It is applied to each element at a time. In the end, a new list is returned with the projected elements.

Let's see another example. Suppose that we have a list of strings and we want to convert all of them
to upper case.

```scheme
(map
  to-upper
  (list "hello" "wonderful" "world"))
```
-> *("HELLO" "WONDERFUL" "WORLD")*

Notice that this time, we don't specify a lambda with the details of the conversion, because the
primitive procedure `to-upper` already accepts a single argument which it converts.

## Filtering
Filtering is suitable for selecting a subset of a given data set, based on some
criteria.

Let's suppose that, given a list of natural numbers, we want to keep only those
that are greater than 10.

```scheme
(filter
  (lambda (x) (> x 10))
  (list 1 15 6 3 4 20 9 35 28))
```
-> *(15 20 35 28)*

As you can guess, the first argument passed to `filter` is a procedure that is expected to return a
boolean value. It implements the checking of the condition, that the selected elements need to satisfy.

## Folding
Folding is the operation of aggregating a set of data, in a way that it results to a single outcome.

Supposedly, we want to add the first five natural numbers.

```scheme
(fold-left + 0 (list 1 2 3 4 5))
```
-> *15*

The first parameter of `fold-left` is a procedure that accepts two arguments and returns the result of
an operation on them. Here, we are passing the `+` primitive procedure.

The second parameter is an initial value, a seed if you like, for the aggregation.

The third parameter is the data set to aggregate.

Investigate the source code of [fold-left](../common-libraries/fold-left.md). You will notice that the
iterative process (expressed in infix notation) for the above example, goes something like this:
(((((0 + 1) + 2) + 3) + 4) + 5).

> As you can see, the operation occurs from left to right and this is why this procedure's name has the "left"
characterization. There is also a `fold-right` procedure that processes the elements in the
opposite direction.

The `reduce` procedure is just like `fold-left`, but without the initial seed.
Let's calculate the product of the first five natural numbers with it.

```scheme
(reduce * (list 1 2 3 4 5))
```
-> *120*

## Combining Operations

It is very common to combine the operations we have seen to solve problems.

As an example, we have a set of numbers and we want to find the product of each and 10 and then
find the sum of these products.

We can define a procedure that does exactly that and call it with the first five natural numbers to test it.

```scheme
(define (sum-products-of-10 sequence)
  (reduce +
          (map (lambda (x) (* x 10))
               sequence)))

(sum-products-of-10 (list 1 2 3 4 5))
```
-> *150*

Finally, let's write and call a procedure that sums the even numbers of a given set.

```scheme
(define (sum-of-evens sequence)
  (reduce +
          (filter (lambda (x) (= 0 (% x 2)))
                  sequence)))

(sum-of-evens (list 1 2 3 4 5))
```
-> *6*

A number is even if the remainder of dividing it with `2` is `0`. So, we are filtering the numbers
based on this condition and use `reduce` and `+` to find their sum.

> List operations are library procedures, not primitive ones, which means they are written in the
language itself and you can see and investigate their source code.

The available list operations are:

- [append](../common-libraries/append.md)
- [filter](../common-libraries/filter.md)
- [flatmap](../common-libraries/flatmap.md)
- [fold-left](../common-libraries/fold-left.md)
- [fold-right](../common-libraries/fold-right.md)
- [length](../common-libraries/length.md)
- [map](../common-libraries/map.md)
- [reduce](../common-libraries/reduce.md)
- [reverse](../common-libraries/reverse.md)

Now, let's move on to [user data structures](user-data-structures.md).