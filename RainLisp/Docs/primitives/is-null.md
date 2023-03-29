# null?
```scheme
(null? value)
```
Determines if the given value is nil, i.e. an empty list.

## Examples
```scheme
(null? nil)
```
-> *true*

```scheme
(null? (list))
```
-> *true*

```scheme
(null? '())
```
-> *true*

```scheme
(null? (list 1 2 3))
```
-> *false*

```scheme
(null? 1)
```
-> *false*