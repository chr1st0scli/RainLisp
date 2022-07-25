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
        [InlineData("1")]
        [InlineData("1.5")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("foo")]
        [InlineData("(quote a)")]
        [InlineData("(set! a 1)")]
        [InlineData("(define a 1)")]
        [InlineData("(define (a) 1)")]
        [InlineData("(define (a b) 1)")]
        [InlineData("(define (a b c) (+ b c))")]
        [InlineData("(define (foo x) (define (innerfoo y) (+ x y)) (innerfoo 1))")]
        [InlineData("(if true 1)")]
        [InlineData("(if true 1 0)")]
        [InlineData("(begin 1)")]
        [InlineData("(begin 1 2 3)")]
        [InlineData("(lambda () 1)")]
        [InlineData("(lambda (x) x)")]
        [InlineData("(lambda (x y) 1)")]
        [InlineData("(lambda (x y) (+ x y))")]
        [InlineData("(foo)")]
        [InlineData("(foo 1)")]
        [InlineData("(foo 1 2 3)")]
        [InlineData("((lambda () 1))")]
        [InlineData("((lambda (x) x) 1)")]
        public void Parse_ValidExpression_DoesNotThrow(string expression)
        {
            // Arrange
            var parser = new Parser();
            var tokens = Tokenizer.TokenizeExt(expression);

            // Act
            // Assert
            parser.Parse(tokens);
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
