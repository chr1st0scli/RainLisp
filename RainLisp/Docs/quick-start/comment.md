# Comment
It's often useful to complement your code with additional explanatory information in natural language.
In RainLisp, you can start a comment with the semicolon character `;`.
Once started, a comment stops at the end of the line.

Let's write some comment.

```scheme
; This is a comment occupying the whole line on its own.
(+ 1 1) ; Add two numbers. This is a comment next to some code.
```
-> *2*

> Note that in RainLisp, there is no alternative way to write comment.
For example, there is no block comment construct.
If you want to comment a group of lines, each should start with a semicolon.

Next, let's see how to define a block of code with [begin](begin.md).
