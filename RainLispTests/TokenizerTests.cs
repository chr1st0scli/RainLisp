using RainLisp;

namespace RainLispTests
{
    public class TokenizerTests
    {
        public static IEnumerable<object[]> GetTokens()
        {
            yield return new object[] { "1", (TokenType.Number, "1"), (TokenType.EOF, "") };
            yield return new object[] { "+1", (TokenType.Number, "+1"), (TokenType.EOF, "") };
            yield return new object[] { "-1", (TokenType.Number, "-1"), (TokenType.EOF, "") };
            yield return new object[] { "12.3456", (TokenType.Number, "12.3456"), (TokenType.EOF, "") };
            yield return new object[] { "\"helloworld\"", (TokenType.String, "\"helloworld\""), (TokenType.EOF, "") };
            yield return new object[] { "\"hello world\"", (TokenType.String, "\"hello world\""), (TokenType.EOF, "") };
            yield return new object[] { "\"hello  world\"", (TokenType.String, "\"hello  world\""), (TokenType.EOF, "") };
            yield return new object[] { @"""hello \""wonderful\"" world""", (TokenType.String, @"""hello \""wonderful\"" world"""), (TokenType.EOF, "") };
            yield return new object[] { @"""hello \\ wonderful \\ world""", (TokenType.String, @"""hello \\ wonderful \\ world"""), (TokenType.EOF, "") };
            //yield return new object[] { //[InlineData(@"""hello \\"" wonderful \\ world""", 1)] //no
            yield return new object[] { "true", (TokenType.Boolean, "true"), (TokenType.EOF, "") };
            yield return new object[] { "false", (TokenType.Boolean, "false"), (TokenType.EOF, "") };
            yield return new object[] { "a", (TokenType.Identifier, "a"), (TokenType.EOF, "") };
            yield return new object[] { "abc", (TokenType.Identifier, "abc"), (TokenType.EOF, "") };
            yield return new object[] { "+", (TokenType.Identifier, "+"), (TokenType.EOF, "") };

            yield return new object[]
            {
                "(quote abcd)",
                (TokenType.LParen, "("),
                (TokenType.Quote, "quote"),
                (TokenType.Identifier, "abcd"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(set! ab 15)",
                (TokenType.LParen, "("),
                (TokenType.Assignment, "set!"),
                (TokenType.Identifier, "ab"),
                (TokenType.Number, "15"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(set! ab 15.4)",
                (TokenType.LParen, "("),
                (TokenType.Assignment, "set!"),
                (TokenType.Identifier, "ab"),
                (TokenType.Number, "15.4"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(define ab 15)",
                (TokenType.LParen, "("),
                (TokenType.Definition, "define"),
                (TokenType.Identifier, "ab"),
                (TokenType.Number, "15"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(define ab 15.32)",
                (TokenType.LParen, "("),
                (TokenType.Definition, "define"),
                (TokenType.Identifier, "ab"),
                (TokenType.Number, "15.32"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(define (foo x y) (+ x y))",
                (TokenType.LParen, "("),
                (TokenType.Definition, "define"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "foo"),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(define(foo x y)(+ x y))",
                (TokenType.LParen, "("),
                (TokenType.Definition, "define"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "foo"),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(if (> 1 0) 1 0)",
                (TokenType.LParen, "("),
                (TokenType.If, "if"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, ">"),
                (TokenType.Number, "1"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.Number, "1"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(if(> 1 0) 1 0)",
                (TokenType.LParen, "("),
                (TokenType.If, "if"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, ">"),
                (TokenType.Number, "1"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.Number, "1"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "( cond ( ( >= 1 0) 0) ( ( <= 2 1) 1) ( else 3) )",
                (TokenType.LParen, "("),
                (TokenType.Cond, "cond"),
                (TokenType.LParen, "("),
                (TokenType.LParen, "("),
                (TokenType.Identifier, ">="),
                (TokenType.Number, "1"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "<="),
                (TokenType.Number, "2"),
                (TokenType.Number, "1"),
                (TokenType.RParen, ")"),
                (TokenType.Number, "1"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.Else, "else"),
                (TokenType.Number, "3"),
                (TokenType.RParen, ")"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(cond((>= 1 0) 0)((<= 2 1) 1)(else 3))",
                (TokenType.LParen, "("),
                (TokenType.Cond, "cond"),
                (TokenType.LParen, "("),
                (TokenType.LParen, "("),
                (TokenType.Identifier, ">="),
                (TokenType.Number, "1"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.Number, "0"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "<="),
                (TokenType.Number, "2"),
                (TokenType.Number, "1"),
                (TokenType.RParen, ")"),
                (TokenType.Number, "1"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.Else, "else"),
                (TokenType.Number, "3"),
                (TokenType.RParen, ")"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(begin 1 2 3 4)",
                (TokenType.LParen, "("),
                (TokenType.Begin, "begin"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.Number, "3"),
                (TokenType.Number, "4"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(lambda (x y) (+ x y))",
                (TokenType.LParen, "("),
                (TokenType.Lambda, "lambda"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(lambda(x y)(+ x y))",
                (TokenType.LParen, "("),
                (TokenType.Lambda, "lambda"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Identifier, "x"),
                (TokenType.Identifier, "y"),
                (TokenType.RParen, ")"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(+ 1 2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(+ +1 -2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Number, "+1"),
                (TokenType.Number, "-2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(* 1 2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "*"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(* -1 +2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "*"),
                (TokenType.Number, "-1"),
                (TokenType.Number, "+2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(/ 1 2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "/"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(/ -1 +2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "/"),
                (TokenType.Number, "-1"),
                (TokenType.Number, "+2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(% 1 2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "%"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(% +1 -2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "%"),
                (TokenType.Number, "+1"),
                (TokenType.Number, "-2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(+ 0 1 2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Number, "0"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(+ 0 -1 2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Number, "0"),
                (TokenType.Number, "-1"),
                (TokenType.Number, "2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };
            yield return new object[] { "()", (TokenType.LParen, "("), (TokenType.RParen, ")"), (TokenType.EOF, "") };

            yield return new object[]
            {
                "(+ 1\n2\r3\r\n4\t5)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.Number, "3"),
                (TokenType.Number, "4"),
                (TokenType.Number, "5"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(and true false)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "and"),
                (TokenType.Boolean, "true"),
                (TokenType.Boolean, "false"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(or false true)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "or"),
                (TokenType.Boolean, "false"),
                (TokenType.Boolean, "true"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                "(xor false true)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "xor"),
                (TokenType.Boolean, "false"),
                (TokenType.Boolean, "true"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };

            yield return new object[]
            {
                @"(+
1
2)",
                (TokenType.LParen, "("),
                (TokenType.Identifier, "+"),
                (TokenType.Number, "1"),
                (TokenType.Number, "2"),
                (TokenType.RParen, ")"),
                (TokenType.EOF, "")
            };
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