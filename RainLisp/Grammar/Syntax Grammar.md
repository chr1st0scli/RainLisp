# RainLisp Syntax Grammar

Terms followed by `*` occur zero or more times.
Terms followed by `+` occur one or more times.
Terms inside square brackets `[]` are optional, i.e. they appear zero or one time.
`|` separates alternatives.
Terms inside double quotes `"` are terminals that appear as is.
Terms in upper case are terminals of a certain class, like identifiers, numbers or strings.
Lower case terms are non-terminals and are defined with the equals sign `=`.
Spaces that appear in the definitions have no semantics, they are used for readability purposes.

## Program
A RainLisp program consists of zero or more definitions and/or expressions in no specific order.
```
program = form* EOF
form = definition|expression
```

## Definition
Definition is a special form for defining variables and functions. A variable is defined by its identifier followed by an expression.
A function is defined by its name followed by zero or more parameters and a body.
The latter form (function) is syntactic sugar for `(define ID (lambda (ID*) body)`.
```
definition = "(" "define" ID expression ")"
           | "(" "define" "(" ID ID* ")" body ")"
```

## Body
A body consists of zero or more definitions followed by at least one expression.
```	
body = definition* expression+
```

## Expression
An expression can be a number, string or boolean literal, an identifier (e.g. variable name)
or one of the special forms `'`, `quote`, `set!`, `if`, `begin`, `lambda`, or one of the derived expressions `cond`, `let`, `and`, `or`.
If it is none of the above, then it is a function application/call.
The first expression of the function application gives the function to be applied
and the zero or more expressions that follow, give the arguments the function is applied to.
```
expression = NUM | STRING | BOOL | ID 
		| "'" quotable
		| "(" "quote" quotable ")"
		| "(" "set!" ID expression ")"
		| "(" "if" expression expression [expression] ")"
		| "(" "cond" condition_clause+ [condition_else_clause] ")"
		| "(" "begin" expression+ ")"
		| "(" "lambda" "(" ID* ")" body ")"
		| "(" "let" "(" let_clause+ ")" body ")"
		| "(" "and" expression+ ")"
		| "(" "or" expression+ ")"
		| "(" expression expression* ")"
```

```
condition_clause = "(" expression expression+ ")"
```

```
condition_else_clause = "(" "else" expression+ ")"
```

```
let_clause = "(" ID expression ")"
```

## Quotable
A quotable can be a number, string or boolean literal, an identifier (e.g. variable name), a keyword (quote, set!, define, if, cond, else, begin, lambda, let, and, or) or a list of zero or more quotables.
```
quotable = NUM | STRING | BOOL | ID
		| "quote" | "set!" | "define" | "if" | "cond" | "else" | "begin" | "lambda" | "let" | "and" | "or"
		| "'" quotable | "(" quotable* ")"
```