# List Operations
There are some operations on sets of data that are very common in all languages with
functional programming characteristics. Let's see some of them and how they are typically used.

## Map
Map projects elements of a list in a certain way. In other words, you can use it to convert a set
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

## Filter
Filter is suitable for selecting a subset of a given data set based on some
criteria.

Let's suppose that, given a list of natural numbers, we want to keep only those
that are greater than 10.

```scheme
(filter
  (lambda (x) (> x 10))
  (list 1 15 6 3 4 20 9 35 28))
```
-> *(15 20 35 28)*

## Folding

## Combining Operations

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