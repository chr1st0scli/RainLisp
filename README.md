# RainLisp
![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/chr1st0scli/3ad6a6f6575320603cc8edf6171b42e8/raw/code-coverage.json)

![Cloudy RainLisp Logo](Artwork/RainLisp-Colored.svg)

## Work in progress.

This is in working state but still incomplete. There are a few pending alterations, further tests and documentation that will be present in the next phases of the project.

## Introduction
RainLisp is a LISP dialect with many similarities to Scheme. It is implemented entirely in C# and therefore brought to the .NET ecosystem.

It is not intended to replace your everyday programming language at work. Though, you can integrate it with your existing systems to allow for their configuration in terms of code.

For example, one can build a system where parts of its computations or workflow logic is implemented in RainLisp. Its simplicity and capabilites makes it ideal for using it like a DSL (Domain Specific Language) that integrates with your .NET system.

Additionally, you can easily extend it to implement your own LISP dialect or replace some of its components, like the tokenizer and parser, and reuse the evaluator to easily build an entirely different but compatible programming language.

You can also use it independently, using its code editor to learn LISP, play around with it and have fun!

- [Lexical Grammar](<RainLisp/Grammar/Lexical Grammar.md>)
- [Syntax Grammar](<RainLisp/Grammar/Syntax Grammar.md>)
- [Documentation](RainLisp/Docs/contents.md)