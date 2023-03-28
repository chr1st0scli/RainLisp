# not
```scheme
(not value)
```
Returns the logical negation of a value. Every value is true except explicit false.

## Examples
```scheme
(not false)
```
-> *true*

```scheme
(not 7)
```
-> *false*

```scheme
(not "rain")
```
-> *false*

```scheme
(not (lambda () 0))
```
-> *false*