# quote
A special form for creating quote symbols. A quote symbol can be viewed as a unique tag.
It is created based on something that can be quoted, i.e. a quotable.
```
(quote quotable)
```

There is an alternative form for creating a quote symbol that is equivalent to the above.
It starts with an apostrophe `'` and is followed by a quotable. This form is often preferred for brevity.
```
'quotable
```

A quotable is anything that can be quoted. It can be a number, string or boolean literal, an identifier (e.g. variable name),
a language keyword or a list of zero or more quotables in the form `(. quotables)`.

## Examples
```scheme
(quote a) ; A single quote symbol.
```
-> *a*

```scheme
'a ; Same as above.
```
-> *a*

```scheme
(= 'a 'a)
```
-> *true*

```scheme
(= 'a 'A)
```
-> *false*

```scheme
(quote (a b 1 2)) ; A list of quote symbols.
```
-> *(a b 1 2)*

```scheme
'(a b 1 2) ; Same as above.
```
-> *(a b 1 2)*

```scheme
'(a b (cd ef)) ; A nested list of quote symbols.
```
-> *(a b (cd ef))*

```scheme
'(quote (a b c)) ; A nested list of quote symbols, starting with the quote symbol quote.
```
-> *(quote (a b c))*

```scheme
''(a b c) ; Same as above.
```
-> *(quote (a b c))*

```scheme
; Create an empty list. Equivalent to calling (list)
'()
```
-> *()*

## Remarks
> A quote symbol is unique during an evaluation session. So, traditionally quote symbols are
compared for equality in "message passing" and "data directed" style programming and when creating and extracting data
from user data structures.

> Quote symbols can also be used in other advanced ways, like metaprogramming, i.e.
writing programs that write programs which can later be evaluated with the aid of the `eval` primitive procedure.

> Note that traditionally in LISP, one can treat some quote symbols as their underlying value.
For example the number 1 as a quote symbol can be added to another number like so: `(+ '1 5)`.
RainLisp differentiates in this regard, i.e. quote symbols are always text representations
and they cannot be used interchangeably with other semantics. The above expression in RainLisp produces an error.

```scheme
(+ '1 5)
```
->
```
Wrong type of argument, expecting one of NumberDatum, StringDatum, but got QuoteSymbol.
Call Stack
[Application +] Line 1, position 2.
```