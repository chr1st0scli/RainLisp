# =
```scheme
(= value1 value2)
```
Determines if two values are equal. Primitive values like numbers, strings, booleans and datetimes are compared by value. All others are compared by reference.

> In traditional Scheme LISP, there are two related primitives. *equal?* compares values and *eq?* compares references. RainLisp differentiates in this regard and encapsulates both with *=*.

## Examples
```scheme
(= 12.5 12.5)
```
-> *true*

```scheme
(= true true)
```
-> *true*

```scheme
(= "rain" "rain")
```
-> *true*

```scheme
(= (make-date 2023 12 31) (make-date 2023 12 31))
```
-> *true*

```scheme
; A lambda is a reference type and evaluates to a new user procedure.
(= (lambda () 0) (lambda () 0))
```
-> *false*

```scheme
; nil is a reference type but unique globally.
(= nil nil)
```
-> *true*

```scheme
; Quote symbols are reference types but unique in an evaluation session.
(= 'rain 'rain)
```
-> *true*