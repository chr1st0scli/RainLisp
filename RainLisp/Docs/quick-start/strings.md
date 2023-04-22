# Strings
An integral part of every programming language is strings. A string is a sequence of characters enclosed in double quotes.

Let's evaluate a string literal.

```scheme
"Hello world!"
```
-> *"Hello world!"*

> Note that RainLisp does not support multiline string literals. This means that the
enclosing double quotes need to be on the same line.

## Escape Sequences
You can specify well known escape sequences within a string. An escape sequence specifies
a special character within the string. You denote one by starting with a backslash `\`,
followed by a reserved character with a special meaning.

The valid escape sequences in RainLisp are:
- `\n` is the line feed character.
- `\r` is the carriage return character.
- `\t` is the tab character.
- `\"` escapes the double quote character. It specifies a double quote by preventing the string termination.
- `\\` escapes the backslash character. It specifies a backslash by preventing the need to specify any of the above escape sequences.

Let's evaluate some strings with escape sequences.

```scheme
"Hello\nworld!"
```
-> *"Hello\nworld!"*

> Note that the interpreter always prints the string representation of a value.

The output seems to be the same with the input. But the value is different in reality and this
can be demonstrated if we use the [display](../primitives/display.md) primitive procedure,
which prints a given argument to the standard output.

```scheme
(display "Hello\nworld!")
```
->
```
Hello
world!
```

Note that `\n` is really evaluated to a new line character.

Now, let's escape some double quote characters.

```scheme
(display "Simon says \"Clap your hands\".")
```
-> *Simon says "Clap your hands".*

## String Operations
It is very common to concatenate strings together. This is achieved with the [+](../primitives/plus.md) primitive procedure.

Let's try it.

```scheme
(+ "Hello" " " "World!")
```
-> *"Hello World!"*

There are other common string-related operations such as [string-length](../primitives/string-length.md),
[substring](../primitives/substring.md), [index-of-string](../primitives/index-of-string.md),
[replace-string](../primitives/replace-string.md), [to-lower](../primitives/to-lower.md) and
[to-upper](../primitives/to-upper.md)

Next, let's learn about [datetimes](datetimes.md).
