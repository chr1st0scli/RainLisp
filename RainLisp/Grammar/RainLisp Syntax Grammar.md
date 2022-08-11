# RainLisp Syntax Grammar

## EBNF-like Grammar
```
program = {definition|expression} EOF

definition = "(" "define" ID expression ")"
			| "(" "define" "(" ID {ID} ")" body ")"
			
body = {definition} expression

expression = NUM | STRING | BOOL | ID 
		| "(" "quote" CHAR {CHAR} ")"
		| "(" "set!" ID expression ")"
		| "(" "if" expression expression [expression] ")"
		| "(" "cond" condition_clause {condition_clause} [condition_else_clause] ")"
		| "(" "begin" expression {expression} ")"
		| "(" "lambda" "(" {ID} ")" body ")"
		| "(" expression {expression} ")"

condition_clause = "(" expression expression {expression} ")"

condition_else_clause = "(" "else" expression {expression} ")"
```

