# nil
```scheme
nil
```
Returns the empty list. The empty list is a reference type and unique in a system wide scope.
It also serves as list termination.

## Examples
```scheme
nil
```
-> *()*

```scheme
(= nil (list))
```
-> *true*

```scheme
 (= nil '())
```
-> *true*

```scheme
(null? nil)
```
-> *true*

```scheme
(= nil 1)
```
-> *false*

```scheme
(cons 1 (cons 2 (cons 3 nil)))
```
-> *(1 2 3)*
