# RainLisp Lexical Grammar
The lexical grammar expressed in regular expressions and accompanied with explanations follows.

## Booleans
A boolean literal is either true or false.
```
true|false
```

## Numbers
A number literal comprises of an optional number sign, followed by one or more digits which might be followed by a decimal point and zero or more digits.
```
(\+|-)?[0-9]+(\.[0-9]*)?
```
Valid examples are: `12`, `012`, `12.34`, `+12.34`, `-34.`

## Strings
### Unescaped String Literals
An unescaped string literal starts and ends with a double quote and encloses none or more of any characters except a carriage return, a line feed or a backslash character. Therefore, multi-line string literals are not supported.
```
"[^"\r\n\\]*"
```
Valid examples are `""`, `"hello world"`

### Valid Escape Sequences
A valid escape sequence in a string literal starts with a backslash character followed by a double quote, r (carriage return), n (line feed), t (tab) or backslash.
```
(\\")|(\\r)|(\\n)|(\\t)|(\\\\)
```
Examples of strings with valid escape sequences are `"This is a double quote \" and a backslash \\"`, `"hello \n world"`

## Identifiers
A symbol is qualified for an identifier as long as it is none of the above literals (boolean, number or string) and special forms or derived expressions of the language (see below). If it does not fall into the above categories, it can comprise of one or more of any characters except delimiters (see below).
```
[^;"()\s]+
```

## Delimiters
Numbers, boolean literals, identifiers and special keywords can be delimited with a comment start `;`, a string start `"`, an opening or closing parenthesis, or any white space character such as space, tab and new line.
```
[;"()\s]
```

## Comment
A comment starts with a semicolon character, as long as it is not inside a string literal, followed by zero or more characters up to the next line break. Block comment is not supported.
```
;.*
```

## Special Forms/Derived Expressions
Special forms and derived expressions are words with special meaning in the language that cannot be changed by the user.
```
true|false|quote|set!|define|if|cond|else|begin|lambda|and|or|let
```
