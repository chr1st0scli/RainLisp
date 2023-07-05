# if
A special form for declaring alternative expressions to be evaluated based on the result of a predicate.

```
(if predicate consequent alternative)
```

The first expression is the predicate. The second is the consequent, which is evaluated if the predicate evaluates to true.
The optional last one is the alternative, which is evaluated if the predicate evaluates to false.
If the alternative is to be evaluated and there is none, the result is unspecified.

## Examples
```scheme
(if true 1 0)
```
-> *1*

```scheme
(if false 1 0)
```
-> *0*

```scheme
(if (> 1 0) "1 is positive.")
```
-> *"1 is positive."*