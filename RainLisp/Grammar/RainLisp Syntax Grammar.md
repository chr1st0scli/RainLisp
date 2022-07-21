# RainLisp Syntax Grammar

## EBNF-like Grammar
```
quote = "(" "quote" CHAR {CHAR} ")"

assignment = "(" "set!" ID expr ")"

definition = "(" "define" ID expr ")"
			| "(" "define" "(" ID {ID} ")" expr_ext ")"

if = "(" "if" cond expr_ext	[expr_ext] ")"

#consider using expr instead of cond
cond = BOOL | ID | if | block | application

block = "(" "begin" expr_ext {expr_ext} ")"

lambda = "(" "lambda" "(" {ID} ")" expr_ext ")"

application = "(" ID {expr} ")"
				| "(" lambda expr {expr} ")"

expr = NUM | STRING | BOOL | ID | quote | if | block | application

expr_ext = expr | assignment | definition

program = expr_ext {expr_ext} EOF
```

