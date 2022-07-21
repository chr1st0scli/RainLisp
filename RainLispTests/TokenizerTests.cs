using RainLisp;

namespace RainLispTests
{
    public class TokenizerTests
    {
        public static IEnumerable<object[]> GetTokens()
        {
            yield return new object[] { "1", (TokenType.Number, "1") };
            yield return new object[] { "+1", (TokenType.Number, "+1") };
            yield return new object[] { "-1", (TokenType.Number, "-1") };
            yield return new object[] { "12.3456", (TokenType.Number, "12.3456") };
            yield return new object[] { "\"helloworld\"", (TokenType.String, "\"helloworld\"") };
            yield return new object[] { "\"hello world\"", (TokenType.String, "\"hello world\"") };
            yield return new object[] { "\"hello  world\"", (TokenType.String, "\"hello  world\"") };
            yield return new object[] { @"""hello \""wonderful\"" world""", (TokenType.String, @"""hello \""wonderful\"" world""") };
            yield return new object[] { @"""hello \\ wonderful \\ world""", (TokenType.String, @"""hello \\ wonderful \\ world""") };
            //yield return new object[] { //[InlineData(@"""hello \\"" wonderful \\ world""", 1)] //no
            yield return new object[] { "true", (TokenType.Boolean, "true") };
            yield return new object[] { "false", (TokenType.Boolean, "false") };
            yield return new object[] { "a", (TokenType.Identifier, "a") };
            yield return new object[] { "abc", (TokenType.Identifier, "abc") };
            yield return new object[] { "+", (TokenType.Identifier, "+") };
            yield return new object[] { "(quote abcd)", (TokenType.LParen, "("), (TokenType.Quote, "quote"), (TokenType.Identifier, "abcd"), (TokenType.RParen, ")") };
            yield return new object[] { "(set! ab 15)", (TokenType.LParen, "("), (TokenType.Assignment, "set!"), (TokenType.Identifier, "ab"), (TokenType.Number, "15"), (TokenType.RParen, ")") };
            yield return new object[] { "(set! ab 15.4)", (TokenType.LParen, "("), (TokenType.Assignment, "set!"), (TokenType.Identifier, "ab"), (TokenType.Number, "15.4"), (TokenType.RParen, ")") };
            yield return new object[] { "(define ab 15)", (TokenType.LParen, "("), (TokenType.Definition, "define"), (TokenType.Identifier, "ab"), (TokenType.Number, "15"), (TokenType.RParen, ")") };
            yield return new object[] { "(define ab 15.32)", (TokenType.LParen, "("), (TokenType.Definition, "define"), (TokenType.Identifier, "ab"), (TokenType.Number, "15.32"), (TokenType.RParen, ")") };
            yield return new object[] { "(define (foo x y) (+ x y))", (TokenType.LParen, "("), (TokenType.Definition, "define"), (TokenType.LParen, "("), (TokenType.Identifier, "foo"), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.RParen, ")") };
            yield return new object[] { "(define(foo x y)(+ x y))", (TokenType.LParen, "("), (TokenType.Definition, "define"), (TokenType.LParen, "("), (TokenType.Identifier, "foo"), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.RParen, ")") };
            yield return new object[] { "(if (> 1 0) 1 0)", (TokenType.LParen, "("), (TokenType.If, "if"), (TokenType.LParen, "("), (TokenType.Identifier, ">"), (TokenType.Number, "1"), (TokenType.Number, "0"), (TokenType.RParen, ")"), (TokenType.Number, "1"), (TokenType.Number, "0"), (TokenType.RParen, ")") };
            yield return new object[] { "(if(> 1 0) 1 0)", (TokenType.LParen, "("), (TokenType.If, "if"), (TokenType.LParen, "("), (TokenType.Identifier, ">"), (TokenType.Number, "1"), (TokenType.Number, "0"), (TokenType.RParen, ")"), (TokenType.Number, "1"), (TokenType.Number, "0"), (TokenType.RParen, ")") };
            yield return new object[] { "(begin 1 2 3 4)", (TokenType.LParen, "("), (TokenType.Begin, "begin"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.Number, "3"), (TokenType.Number, "4"), (TokenType.RParen, ")") };
            yield return new object[] { "(lambda (x y) (+ x y))", (TokenType.LParen, "("), (TokenType.Lambda, "lambda"), (TokenType.LParen, "("), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.RParen, ")") };
            yield return new object[] { "(lambda(x y)(+ x y))", (TokenType.LParen, "("), (TokenType.Lambda, "lambda"), (TokenType.LParen, "("), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Identifier, "x"), (TokenType.Identifier, "y"), (TokenType.RParen, ")"), (TokenType.RParen, ")") };
            yield return new object[] { "(+ 1 2)", (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.RParen, ")") };
            yield return new object[] { "(+ +1 -2)", (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Number, "+1"), (TokenType.Number, "-2"), (TokenType.RParen, ")") };
            yield return new object[] { "(* 1 2)", (TokenType.LParen, "("), (TokenType.Identifier, "*"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.RParen, ")") };
            yield return new object[] { "(* -1 +2)", (TokenType.LParen, "("), (TokenType.Identifier, "*"), (TokenType.Number, "-1"), (TokenType.Number, "+2"), (TokenType.RParen, ")") };
            yield return new object[] { "(/ 1 2)", (TokenType.LParen, "("), (TokenType.Identifier, "/"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.RParen, ")") };
            yield return new object[] { "(/ -1 +2)", (TokenType.LParen, "("), (TokenType.Identifier, "/"), (TokenType.Number, "-1"), (TokenType.Number, "+2"), (TokenType.RParen, ")") };
            yield return new object[] { "(% 1 2)", (TokenType.LParen, "("), (TokenType.Identifier, "%"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.RParen, ")") };
            yield return new object[] { "(% +1 -2)", (TokenType.LParen, "("), (TokenType.Identifier, "%"), (TokenType.Number, "+1"), (TokenType.Number, "-2"), (TokenType.RParen, ")") };
            yield return new object[] { "(+ 0 1 2)", (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Number, "0"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.RParen, ")") };
            yield return new object[] { "(+ 0 -1 2)", (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Number, "0"), (TokenType.Number, "-1"), (TokenType.Number, "2"), (TokenType.RParen, ")") };
            yield return new object[] { "()", (TokenType.LParen, "("), (TokenType.RParen, ")") };
            yield return new object[] { "(+ 1\n2\r3\r\n4\t5)", (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.Number, "3"), (TokenType.Number, "4"), (TokenType.Number, "5"), (TokenType.RParen, ")") };
            yield return new object[] { @"(+
1
2)", (TokenType.LParen, "("), (TokenType.Identifier, "+"), (TokenType.Number, "1"), (TokenType.Number, "2"), (TokenType.RParen, ")") };
        }

        [Theory]
        [MemberData(nameof(GetTokens))]
        public void Tokenize_Expression_ReturnsCorrectTokens(string expression, params (TokenType, string)[] expectedTokens)
        {
            // Arrange
            // Act
            var tokens = Tokenizer.TokenizeExt(expression);

            // Assert
            Assert.Equal(expectedTokens.Length, tokens.Count);
            for (int i = 0; i < expectedTokens.Length; i++)
            {
                var expectedType = expectedTokens[i];
                Assert.Equal(expectedType.Item1, tokens[i].Type);
                Assert.Equal(expectedType.Item2, tokens[i].Value);
            }
        }
    }
}