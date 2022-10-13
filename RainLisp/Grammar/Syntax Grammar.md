# RainLisp Syntax Grammar

## EBNF-like Grammar
Terms inside curly braces `{}` occur zero or more times. Terms inside square brackets `[]` appear zero or one time. `|` separates alternatives. Terms inside double quotes `"` are terminals that appear as is. Terms in upper case are terminals of a certain class, like numbers or strings. Lower case terms are non-terminals and are defined with the equals sign `=`. Spaces that appear in the definitions have no semantics, they are used for readability purposes.
```
program = {definition|expression} EOF
```

```
definition = "(" "define" ID expression ")"
			| "(" "define" "(" ID {ID} ")" body ")"
```

```	
body = {definition} expression {expression}
```

```
expression = NUM | STRING | BOOL | ID 
		| "(" "quote" CHAR {CHAR} ")"
		| "(" "set!" ID expression ")"
		| "(" "if" expression expression [expression] ")"
		| "(" "cond" condition_clause {condition_clause} [condition_else_clause] ")"
		| "(" "begin" expression {expression} ")"
		| "(" "lambda" "(" {ID} ")" body ")"
		| "(" "let" "(" let_clause {let_clause} ")" body ")"
		| "(" "and" expression {expression} ")"
		| "(" "or" expression {expression} ")"
		| "(" expression {expression} ")"
```

```
condition_clause = "(" expression expression {expression} ")"
```

```
condition_else_clause = "(" "else" expression {expression} ")"
```

```
let_clause = "(" ID expression ")"
```
