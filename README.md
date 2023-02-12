# RainLisp

![Cloudy RainLisp Logo](Artwork/RainLisp.svg)

## Work in progress.

This is in working state but still incomplete. There are a few pending alterations, missing XML comments, further tests and documentation that will be present in the next phases of the project.

## Introduction
RainLisp is a LISP dialect with many similarities to Scheme. It is implemented entirely in C# and therefore brought to the .NET ecosystem.

It is not intended to replace your everyday programming language at work. Though, you can integrate it to your existing systems to allow for their configuration in terms of code. For example, one can build a system where parts of its computations or workflow logic is implemented in RainLisp.

Additionally, you can easily extend it to implement your own LISP dialect or replace some of its components, like the tokenizer and parser, and reuse the evaluator to easily build an entirely different but compatible programming language.

- [Lexical Grammar](<RainLisp/Grammar/Lexical Grammar.md>)
- [Syntax Grammar](<RainLisp/Grammar/Syntax Grammar.md>)
