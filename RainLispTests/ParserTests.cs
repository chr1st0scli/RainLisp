using RainLisp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [InlineData("(quote a)")]
        [InlineData("(set! a 1)")]
        [InlineData("(define a 1)")]
        [InlineData("(if true 1)")]
        [InlineData("(if true 1 0)")]
        [InlineData("(begin 1)")]
        [InlineData("(begin 1 2 3)")]
        //[InlineData("(lambda 1")]
        [InlineData("(foo)")]
        [InlineData("(foo 1)")]
        [InlineData("(foo 1 2 3)")]
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
        [InlineData("(define a)")]
        [InlineData("(define a 1")]
        [InlineData("(if)")]
        [InlineData("(if true)")]
        [InlineData("(if true 1")]
        [InlineData("(if true 1 0")]
        [InlineData("(begin)")]
        [InlineData("(begin 1")]
        //[InlineData("(lambda 1")]
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
