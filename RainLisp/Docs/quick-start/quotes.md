# Quotes
You can imagine that a quote is something like a tag that you may attach to data.
It is a feature that opens the doors to many possibilities. You can use them with
user data structures, in message passing, data driven programming and metaprogramming
that we will see later.

For now, let's just introduce the topic.

```scheme
(quote this-is-a-quote)
```
-> *this-is-a-quote*

The above expression creates the quote symbol `this-is-a-quote`.

Traditionally in LISP dialects, there is an alternative way to create quotes,
using the single quote character `'`. It is often preferred for its convenience.
Generally, `'abc` is equivalent to `(quote abc)`.

```scheme
'this-is-a-quote
```
-> *this-is-a-quote*

A quote symbol is unique during an evaluation session, so it comes in handy for equality checks.
For example, we can check if something is tagged with a particular text.

> The comparisons are also performant, because quotes are reference types, which means that only
memory addresses are compared.

```scheme
(= 'this-is-a-quote 'this-is-a-quote)
```
-> *true*

Also, they are case sensitive.

```scheme
(= 'this-is-a-quote 'This-is-a-quote)
```
-> *false*

You can also create lists of quotes.

```scheme
'(ab cd 10 20 30)
```
-> *(ab cd 10 20 30)*

or equivalently

```scheme
(quote (ab cd 10 20 30))
```
-> *(ab cd 10 20 30)*

> Something notable is that a quote of a primitive literal in RainLisp is just a quote.
For example `'10` is the quote symbol `10` and cannot be handled like a number. 
Whereas in other LISP dialects, it can. You can add it or do anything to it that you can to numbers.
In RainLisp, once a quote, always a quote.

If you want to see more examples, look at the [specification](../special-forms-derived-expressions/quote.md).

Congratulations! You have covered all the basics of RainLisp.
It's time to move on to [data structures](pairs.md).
