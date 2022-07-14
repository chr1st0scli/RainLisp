using RainLisp;

namespace RainLispTests
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData("1", TokenType.Number)]
        [InlineData("+1", TokenType.Number)]
        [InlineData("-1", TokenType.Number)]
        [InlineData("12.3456", TokenType.Number)]
        [InlineData("\"helloworld\"", TokenType.String)]
        [InlineData("\"hello world\"", TokenType.String)]
        [InlineData("\"hello  world\"", TokenType.String)]
        [InlineData(@"""hello \""wonderful\"" world""", TokenType.String)]
        [InlineData(@"""hello \\ wonderful \\ world""", TokenType.String)]
        //[InlineData(@"""hello \\"" wonderful \\ world""", 1)] //no
        [InlineData("true", TokenType.Boolean)]
        [InlineData("false", TokenType.Boolean)]
        [InlineData("a", TokenType.Identifier)]
        [InlineData("abc", TokenType.Identifier)]
        [InlineData("+", TokenType.Identifier)]
        [InlineData("(quote abcd)", TokenType.LParen, TokenType.Quote, TokenType.Identifier, TokenType.RParen)]
        [InlineData("(set! ab 15)", TokenType.LParen, TokenType.Assignment, TokenType.Identifier, TokenType.Number, TokenType.RParen)]
        [InlineData("(set! ab 15.4)", TokenType.LParen, TokenType.Assignment, TokenType.Identifier, TokenType.Number, TokenType.RParen)]
        [InlineData("(define ab 15)", TokenType.LParen, TokenType.Definition, TokenType.Identifier, TokenType.Number, TokenType.RParen)]
        [InlineData("(define ab 15.32)", TokenType.LParen, TokenType.Definition, TokenType.Identifier, TokenType.Number, TokenType.RParen)]
        [InlineData("(define (foo x y) (+ x y))", TokenType.LParen, TokenType.Definition, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.RParen)]
        [InlineData("(define(foo x y)(+ x y))", TokenType.LParen, TokenType.Definition, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.RParen)]
        [InlineData("(if (> 1 0) 1 0)", TokenType.LParen, TokenType.If, TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(if(> 1 0) 1 0)", TokenType.LParen, TokenType.If, TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(begin 1 2 3 4)", TokenType.LParen, TokenType.Begin, TokenType.Number, TokenType.Number, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(lambda (x y) (+ x y))", TokenType.LParen, TokenType.Lambda, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.RParen)]
        [InlineData("(lambda(x y)(+ x y))", TokenType.LParen, TokenType.Lambda, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.LParen, TokenType.Identifier, TokenType.Identifier, TokenType.Identifier, TokenType.RParen, TokenType.RParen)]
        [InlineData("(+ 1 2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(+ +1 -2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(* 1 2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(* -1 +2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(/ 1 2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(/ -1 +2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(% 1 2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(% +1 -2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(+ 0 1 2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("(+ 0 -1 2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData("()", TokenType.LParen, TokenType.RParen)]
        [InlineData("(+ 1\n2\r3\r\n4\t5)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.Number, TokenType.Number, TokenType.Number, TokenType.RParen)]
        [InlineData(@"(+
1
2)", TokenType.LParen, TokenType.Identifier, TokenType.Number, TokenType.Number, TokenType.RParen)]
        public void Tokenize_Expression_ReturnsCorrectTokens(string expression, params TokenType[] expectedTypes)
        {
            // Arrange
            // Act
            var tokens = Tokenizer.TokenizeExt(expression);

            // Assert
            Assert.Equal(expectedTypes.Length, tokens.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                var expectedType = expectedTypes[i];
                Assert.Equal(expectedType, tokens[i].Type);
            }
        }
    }
}