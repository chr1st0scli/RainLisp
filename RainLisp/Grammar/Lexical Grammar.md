# RainLisp Lexical Grammar
The lexical grammar expressed in regular expressions.

## Booleans
A boolean literal is either true or false.
```
true|false
```

## Numbers
A number literal comprises of 1 or more digits which might be followed by `.` and one or more digits. Alternatively, it can simply start with a `.` and followed by one or more digits.
```
([0-9]+(\.[0-9]+)?)|\.[0-9]+
```
Valid examples are: `12` `012` `12.34` `.34`

## Strings
### Unescaped String Literals
An unescaped string literal starts and ends with a `"` and encloses none or more of any characters except a carriage return, a line feed, a tab or a backslash character.
```
"[^"\r\n\t\\]*"
```
### Valid Escape Sequences
The valid escape sequences in a string literal are `\"` `\r` `\n` `\t` and `\\`.
```
(\\")|(\\r)|(\\n)|(\\t)|(\\\\)
```

## Identifiers
An identifier can comprise of one or more of any characters except `;` `"` `(` `)` or any white space character.
```
[^;"()\s]+
```

## Delimiters
Numbers, boolean literals, identifiers and special keywords are delimited with `;` `"` `(` `)` or any white space character.
```
[;"()\s]
```
