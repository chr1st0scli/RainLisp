# error
```scheme
(error primitive-value)
```
Causes a user exception with a string representation of a primitive value to be thrown.
A numeric primitive value is formatted using the invariant culture but all other primitives use the local culture.

> *primitive-value* is either a boolean, number, string or datetime.

## Example
```scheme
(error "An error occured.")
```