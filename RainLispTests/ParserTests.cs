﻿using Newtonsoft.Json;
using RainLisp;

namespace RainLispTests
{
    public class ParserTests
    {
        [Fact]
        public void Foo()
        {
            // Arrange
            string expression = "(+ 1 2)";
            var parser = new Parser();
            var tokens = Tokenizer.TokenizeExt(expression);

            // Act
            parser.Parse(tokens);

            // Assert
        }

        [Theory]
        [InlineData(0, "1")]
        [InlineData(1, "1.5")]
        [InlineData(2, "true")]
        [InlineData(3, "false")]
        [InlineData(4, "foo")]
        [InlineData(5, "(quote a)")]
        [InlineData(6, "(set! a 1)")]
        [InlineData(7, "(define a 1)")]
        [InlineData(8, "(define (a) 1)")]
        [InlineData(9, "(define (a b) 1)")]
        [InlineData(10, "(define (a b c) (+ b c))")]
        [InlineData(11, "(define (foo x) (define (innerfoo y) (+ x y)) (innerfoo 1))")]
        [InlineData(12, "(if true 1)")]
        [InlineData(13, "(if true 1 0)")]
        [InlineData(14, "(begin 1)")]
        [InlineData(15, "(begin 1 2 3)")]
        [InlineData(16, "(lambda () 1)")]
        [InlineData(17, "(lambda (x) x)")]
        [InlineData(18, "(lambda (x y) 1)")]
        [InlineData(19, "(lambda (x y) (+ x y))")]
        [InlineData(20, "(foo)")]
        [InlineData(21, "(foo 1)")]
        [InlineData(22, "(foo 1 2 3)")]
        [InlineData(23, "((lambda () 1))")]
        [InlineData(24, "((lambda (x) x) 1)")]
        public void Parse_ValidExpression_GivesExpectedAST(int astIndex, string expression)
        {
            // Arrange
            var parser = new Parser();
            var tokens = Tokenizer.TokenizeExt(expression);

            // Act
            var program = parser.Parse(tokens);
            string ast = JsonConvert.SerializeObject(program, Formatting.Indented);
            string expectedAst = File.ReadAllText($"AbstractSyntaxTrees\\{astIndex:00}.json");

            // Assert
            Assert.Equal(expectedAst, ast);
        }

        [Theory]
        [InlineData("(quote)")]
        [InlineData("(quote a")]
        [InlineData("(set!)")]
        [InlineData("(set! a)")]
        [InlineData("(set! a 1")]
        [InlineData("(define)")]
        [InlineData("(define a)")] // error says expecting ( but more accurate would be expecting expression
        [InlineData("(define a 1")]
        [InlineData("(define (1) 1)")]
        [InlineData("(define (foo 1)")] // error says expecting ) but could say expecting identifier, not a number
        [InlineData("(define (foo a))")]
        [InlineData("(define (foo) [ define (boo) 1) (boo))")]
        [InlineData("(define (foo) (define 1 1) 1)")]
        [InlineData("(define (foo) (define a 1))")] // error says expecting ( but more accurate would be expecting expression in the body
        [InlineData("(if)")]
        [InlineData("(if true)")]
        [InlineData("(if true 1")]
        [InlineData("(if true 1 0")]
        [InlineData("(begin)")]
        [InlineData("(begin 1")]
        [InlineData("(lambda 1")]
        [InlineData("(lambda (1) 1)")] // error says expecting ) but could say expecting identifier, not a number
        [InlineData("(lambda ())")] // error says expecting ( but more accurate would be expecting expression in the body
        [InlineData("(lambda () 1")]
        [InlineData("(foo")]
        public void Parse_InvalidExpression_Throws(string expression)
        {
            // Arrange
            var parser = new Parser();
            var tokens = Tokenizer.TokenizeExt(expression);

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => parser.Parse(tokens));
        }
    }
}
