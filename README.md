# RainLisp [![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/intent/tweet?text=RainLisp,%20a%20.NET%20LISP%20implementation.&url=https://github.com/chr1st0scli/RainLisp&hashtags=lisp,Dotnet,developers)

![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/chr1st0scli/3ad6a6f6575320603cc8edf6171b42e8/raw/code-coverage.json)
[![nuget](https://img.shields.io/nuget/vpre/RainLisp?color=blue)](https://www.nuget.org/packages/RainLisp/)
[![License](https://img.shields.io/github/license/chr1st0scli/rainlisp)](LICENSE.txt)

![Cloudy RainLisp Logo](Artwork/RainLisp-Colored.svg)

RainLisp is a programming language, belonging to the LISP family of languages, with many similarities to Scheme. It is implemented entirely in C# and therefore brought to the .NET ecosystem.

It is not intended to replace your everyday programming language at work. Though, you can integrate it with your existing systems to allow for their configuration in terms of code.

For example, one can build a system where parts of its computations or workflow logic is implemented in RainLisp. Its simplicity and capabilities make it ideal for using it like a DSL (Domain Specific Language) that integrates with your .NET system.

Additionally, you can easily extend it to implement your own LISP dialect or replace some of its components, like the tokenizer and parser, and reuse the evaluator to easily build an entirely different but compatible programming language.

You can also use it independently, using its code editor to learn LISP, play around with it and have fun!

## Documentation
- [Tutorial](RainLisp/Docs/quick-start.md)
- [Specification](RainLisp/Docs/contents.md)
- [.NET Integration](RainLisp/Docs/dotnet-integration.md)

## Tools
- [RainLisp Console](https://github.com/chr1st0scli/RainLispConsole)
- [RainLisp VSCode](https://marketplace.visualstudio.com/items?itemName=chr1st0scli.rainlisp-vscode)
