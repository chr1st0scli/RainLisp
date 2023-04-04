# quote
```
"(" "quote" quotable ")"
```

```
"'" quotable
```

```
quotable = NUM | STRING | BOOL | ID
		| "quote" | "set!" | "define" | "if" | "cond" 
		| "else" | "begin" | "lambda" | "let" | "and" | "or"
		| "'" quotable | "(" quotable* ")"
```

## Example
```scheme

```