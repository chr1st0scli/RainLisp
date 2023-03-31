# utc?
```scheme
(utc? datetime)
```
Determines if the given datetime is Coordinated Universal Time (UTC).

## Examples
```scheme
(utc? (utc-now))
```
-> *true*

```scheme
(utc? (now))
```
-> *false*