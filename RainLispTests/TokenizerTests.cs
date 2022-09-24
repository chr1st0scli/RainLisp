using RainLisp;
using RainLisp.Tokenization;

namespace RainLispTests
{
    public class TokenizerTests
    {
        private readonly Tokenizer _tokenizer;

        public TokenizerTests()
        {
            _tokenizer = new Tokenizer();
        }

        public static IEnumerable<object[]> GetTokens()
        {
            yield return new object[] { "1", new ExpectedToken(TokenType.Number, "1", 1), new ExpectedToken(TokenType.EOF, "", 2) };
            yield return new object[] { "+1", new ExpectedToken(TokenType.Number, "+1", 1), new ExpectedToken(TokenType.EOF, "", 3) };
            yield return new object[] { "-1", new ExpectedToken(TokenType.Number, "-1", 1), new ExpectedToken(TokenType.EOF, "", 3) };
            yield return new object[] { "12.3456", new ExpectedToken(TokenType.Number, "12.3456", 1), new ExpectedToken(TokenType.EOF, "", 8) };

            //yield return new object[] { "\"helloworld\"", new ExpectedToken(TokenType.String, "helloworld", 2), new ExpectedToken(TokenType.EOF, "", 13) };
            //yield return new object[] { "\"hello world\"", new ExpectedToken(TokenType.String, "hello world", 2), new ExpectedToken(TokenType.EOF, "", 14) };
            //yield return new object[] { "\"hello  world\"", new ExpectedToken(TokenType.String, "hello  world", 2), new ExpectedToken(TokenType.EOF, "", 15) };
            //yield return new object[] { @"""hello \""wonderful\"" world""", new ExpectedToken(TokenType.String, "hello \"wonderful\" world", 2), new ExpectedToken(TokenType.EOF, "", 28) };
            //yield return new object[] { @"""hello \\ wonderful \\ world""", new ExpectedToken(TokenType.String, "hello \\ wonderful \\ world"), new ExpectedToken(TokenType.EOF, "") };

            //yield return new object[] { //[InlineData(@"""hello \\"" wonderful \\ world""", 1)] //no

            yield return new object[] { "true", new ExpectedToken(TokenType.Boolean, "true", 1), new ExpectedToken(TokenType.EOF, "", 5) };
            yield return new object[] { "false", new ExpectedToken(TokenType.Boolean, "false", 1), new ExpectedToken(TokenType.EOF, "", 6) };
            yield return new object[] { "a", new ExpectedToken(TokenType.Identifier, "a", 1), new ExpectedToken(TokenType.EOF, "", 2) };
            yield return new object[] { "abc", new ExpectedToken(TokenType.Identifier, "abc", 1), new ExpectedToken(TokenType.EOF, "", 4) };
            yield return new object[] { "+", new ExpectedToken(TokenType.Identifier, "+", 1), new ExpectedToken(TokenType.EOF, "", 2) };

            yield return new object[]
            {
                "(quote abcd)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Quote, "quote", 2),
                new ExpectedToken(TokenType.Identifier, "abcd", 8),
                new ExpectedToken(TokenType.RParen, ")", 12),
                new ExpectedToken(TokenType.EOF, "", 13)
            };

            yield return new object[]
            {
                "(set! ab 15)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Assignment, "set!", 2),
                new ExpectedToken(TokenType.Identifier, "ab", 7),
                new ExpectedToken(TokenType.Number, "15", 10),
                new ExpectedToken(TokenType.RParen, ")", 12),
                new ExpectedToken(TokenType.EOF, "", 13)
            };

            yield return new object[]
            {
                "(set! ab 15.4)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Assignment, "set!", 2),
                new ExpectedToken(TokenType.Identifier, "ab", 7),
                new ExpectedToken(TokenType.Number, "15.4", 10),
                new ExpectedToken(TokenType.RParen, ")", 14),
                new ExpectedToken(TokenType.EOF, "", 15)
            };

            yield return new object[]
            {
                "(define ab 15)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Definition, "define", 2),
                new ExpectedToken(TokenType.Identifier, "ab", 9),
                new ExpectedToken(TokenType.Number, "15", 12),
                new ExpectedToken(TokenType.RParen, ")", 14),
                new ExpectedToken(TokenType.EOF, "", 15)
            };

            yield return new object[]
            {
                "(define ab 15.32)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Definition, "define", 2),
                new ExpectedToken(TokenType.Identifier, "ab", 9),
                new ExpectedToken(TokenType.Number, "15.32", 12),
                new ExpectedToken(TokenType.RParen, ")", 17),
                new ExpectedToken(TokenType.EOF, "", 18)
            };

            var defineExpectedTokens = new ExpectedToken[]
            {
                new(TokenType.LParen, "(", 1),
                new(TokenType.Definition, "define", 3),
                new(TokenType.LParen, "(", 10),
                new(TokenType.Identifier, "foo", 11),
                new(TokenType.Identifier, "x", 15),
                new(TokenType.Identifier, "y", 17),
                new(TokenType.RParen, ")", 18),
                new(TokenType.LParen, "(", 20),
                new(TokenType.Identifier, "+", 21),
                new(TokenType.Identifier, "x", 23),
                new(TokenType.Identifier, "y", 25),
                new(TokenType.RParen, ")", 26),
                new(TokenType.RParen, ")", 28),
                new(TokenType.EOF, "", 29)
            };

            yield return new object[]
            {
                "( define (foo x y) (+ x y) )"
            }.Concat(defineExpectedTokens).ToArray();

            var defineExpectedTokens2 = defineExpectedTokens
                .Zip(new[] { 1, 2, 8, 9, 13, 15, 16, 17, 18, 20, 22, 23, 24, 25 })
                .Select(tokenPositionPair => new ExpectedToken(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(define(foo x y)(+ x y))"
            }.Concat(defineExpectedTokens2).ToArray();

            var ifExpectedTokens = new ExpectedToken[]
            {
                new(TokenType.LParen, "(", 1),
                new(TokenType.If, "if", 3),
                new(TokenType.LParen, "(", 6),
                new(TokenType.Identifier, ">", 7),
                new(TokenType.Number, "1", 9),
                new(TokenType.Number, "0", 11),
                new(TokenType.RParen, ")", 12),
                new(TokenType.Number, "1", 14),
                new(TokenType.Number, "0", 16),
                new(TokenType.RParen, ")", 18),
                new(TokenType.EOF, "", 19)
            };

            yield return new object[]
            {
                "( if (> 1 0) 1 0 )"
            }.Concat(ifExpectedTokens).ToArray();

            var ifExpectedTokens2 = ifExpectedTokens
                .Zip(new[] { 1, 2, 4, 5, 7, 9, 10, 12, 14, 15, 16 })
                .Select(tokenPositionPair => new ExpectedToken(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(if(> 1 0) 1 0)",
            }.Concat(ifExpectedTokens2).ToArray();

            var condExpectedTokens = new ExpectedToken[]
            {
                new(TokenType.LParen, "(", 1),
                new(TokenType.Cond, "cond", 3),
                new(TokenType.LParen, "(", 8),
                new(TokenType.LParen, "(", 10),
                new(TokenType.Identifier, ">=", 12),
                new(TokenType.Number, "1", 15),
                new(TokenType.Number, "0", 17),
                new(TokenType.RParen, ")", 18),
                new(TokenType.Number, "0", 20),
                new(TokenType.RParen, ")", 21),
                new(TokenType.LParen, "(", 23),
                new(TokenType.LParen, "(", 25),
                new(TokenType.Identifier, "<=", 27),
                new(TokenType.Number, "2", 30),
                new(TokenType.Number, "1", 32),
                new(TokenType.RParen, ")", 33),
                new(TokenType.Number, "1", 35),
                new(TokenType.RParen, ")", 36),
                new(TokenType.LParen, "(", 38),
                new(TokenType.Else, "else", 40),
                new(TokenType.Number, "3", 45),
                new(TokenType.RParen, ")", 46),
                new(TokenType.RParen, ")", 48),
                new(TokenType.EOF, "", 49)
            };

            yield return new object[]
            {
                "( cond ( ( >= 1 0) 0) ( ( <= 2 1) 1) ( else 3) )"
            }.Concat(condExpectedTokens).ToArray();

            var condExpectedTokens2 = condExpectedTokens
                .Zip(new[] { 1, 2, 6, 7, 8, 11, 13, 14, 16, 17, 18, 19, 20, 23, 25, 26, 28, 29, 30, 31, 36, 37, 38, 39 })
                .Select(tokenPositionPair => new ExpectedToken(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(cond((>= 1 0) 0)((<= 2 1) 1)(else 3))"
            }.Concat(condExpectedTokens2).ToArray();

            yield return new object[]
            {
                "(begin 1 2 3 4)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Begin, "begin", 2),
                new ExpectedToken(TokenType.Number, "1", 8),
                new ExpectedToken(TokenType.Number, "2", 10),
                new ExpectedToken(TokenType.Number, "3", 12),
                new ExpectedToken(TokenType.Number, "4", 14),
                new ExpectedToken(TokenType.RParen, ")", 15),
                new ExpectedToken(TokenType.EOF, "", 16)
            };

            var lambdaExpectedTokens = new ExpectedToken[]
            {
                new(TokenType.LParen, "(", 1),
                new(TokenType.Lambda, "lambda", 3),
                new(TokenType.LParen, "(", 10),
                new(TokenType.Identifier, "x", 11),
                new(TokenType.Identifier, "y", 13),
                new(TokenType.RParen, ")", 14),
                new(TokenType.LParen, "(", 16),
                new(TokenType.Identifier, "+", 17),
                new(TokenType.Identifier, "x", 19),
                new(TokenType.Identifier, "y", 21),
                new(TokenType.RParen, ")", 22),
                new(TokenType.RParen, ")", 24),
                new(TokenType.EOF, "", 25)
            };

            yield return new object[]
            {
                "( lambda (x y) (+ x y) )"
            }.Concat(lambdaExpectedTokens).ToArray();

            var lambdaExpectedTokens2 = lambdaExpectedTokens
                .Zip(new[] { 1, 2, 8, 9, 11, 12, 13, 14, 16, 18, 19, 20, 21 })
                .Select(tokenPositionPair => new ExpectedToken(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(lambda(x y)(+ x y))"
            }.Concat(lambdaExpectedTokens2).ToArray();

            var letExpectedTokens = new ExpectedToken[]
            {
                new(TokenType.LParen, "(", 1),
                new(TokenType.Let, "let", 3),
                new(TokenType.LParen, "(", 7),
                new(TokenType.LParen, "(", 8),
                new(TokenType.Identifier, "a", 9),
                new(TokenType.Number, "1", 11),
                new(TokenType.RParen, ")", 12),
                new(TokenType.LParen, "(", 14),
                new(TokenType.Identifier, "b", 15),
                new(TokenType.Number, "2", 17),
                new(TokenType.RParen, ")", 18),
                new(TokenType.LParen, "(", 20),
                new(TokenType.Identifier, "c", 21),
                new(TokenType.Number, "3", 23),
                new(TokenType.RParen, ")", 24),
                new(TokenType.RParen, ")", 25),
                new(TokenType.LParen, "(", 27),
                new(TokenType.Identifier, "+", 28),
                new(TokenType.Identifier, "a", 30),
                new(TokenType.Identifier, "b", 32),
                new(TokenType.Identifier, "c", 34),
                new(TokenType.RParen, ")", 35),
                new(TokenType.RParen, ")", 37),
                new(TokenType.EOF, "", 38)
            };

            yield return new object[]
            {
                "( let ((a 1) (b 2) (c 3)) (+ a b c) )"
            }.Concat(letExpectedTokens).ToArray();

            var letExpectedTokens2 = letExpectedTokens
                .Zip(new[] { 1, 2, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 17, 19, 20, 21, 22, 23, 25, 27, 29, 30, 31, 32 })
                .Select(tokenPositionPair => new ExpectedToken(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(let((a 1)(b 2)(c 3))(+ a b c))"
            }.Concat(letExpectedTokens2).ToArray();

            yield return new object[]
            {
                "(+ 1 2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "+", 2),
                new ExpectedToken(TokenType.Number, "1", 4),
                new ExpectedToken(TokenType.Number, "2", 6),
                new ExpectedToken(TokenType.RParen, ")", 7),
                new ExpectedToken(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(+ +1 -2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "+", 2),
                new ExpectedToken(TokenType.Number, "+1", 4),
                new ExpectedToken(TokenType.Number, "-2", 7),
                new ExpectedToken(TokenType.RParen, ")", 9),
                new ExpectedToken(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(* 1 2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "*", 2),
                new ExpectedToken(TokenType.Number, "1", 4),
                new ExpectedToken(TokenType.Number, "2", 6),
                new ExpectedToken(TokenType.RParen, ")", 7),
                new ExpectedToken(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(* -1 +2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "*", 2),
                new ExpectedToken(TokenType.Number, "-1", 4),
                new ExpectedToken(TokenType.Number, "+2", 7),
                new ExpectedToken(TokenType.RParen, ")", 9),
                new ExpectedToken(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(/ 1 2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "/", 2),
                new ExpectedToken(TokenType.Number, "1", 4),
                new ExpectedToken(TokenType.Number, "2", 6),
                new ExpectedToken(TokenType.RParen, ")", 7),
                new ExpectedToken(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(/ -1 +2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "/", 2),
                new ExpectedToken(TokenType.Number, "-1", 4),
                new ExpectedToken(TokenType.Number, "+2", 7),
                new ExpectedToken(TokenType.RParen, ")", 9),
                new ExpectedToken(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(% 1 2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "%", 2),
                new ExpectedToken(TokenType.Number, "1", 4),
                new ExpectedToken(TokenType.Number, "2", 6),
                new ExpectedToken(TokenType.RParen, ")", 7),
                new ExpectedToken(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(% +1 -2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "%", 2),
                new ExpectedToken(TokenType.Number, "+1", 4),
                new ExpectedToken(TokenType.Number, "-2", 7),
                new ExpectedToken(TokenType.RParen, ")", 9),
                new ExpectedToken(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(+ 0 1 2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "+", 2),
                new ExpectedToken(TokenType.Number, "0", 4),
                new ExpectedToken(TokenType.Number, "1", 6),
                new ExpectedToken(TokenType.Number, "2", 8),
                new ExpectedToken(TokenType.RParen, ")", 9),
                new ExpectedToken(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(+ 0 -1 2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "+", 2),
                new ExpectedToken(TokenType.Number, "0", 4),
                new ExpectedToken(TokenType.Number, "-1", 6),
                new ExpectedToken(TokenType.Number, "2", 9),
                new ExpectedToken(TokenType.RParen, ")", 10),
                new ExpectedToken(TokenType.EOF, "", 11)
            };

            yield return new object[] { "()", new ExpectedToken(TokenType.LParen, "(", 1), new ExpectedToken(TokenType.RParen, ")", 2), new ExpectedToken(TokenType.EOF, "", 3) };

            yield return new object[]
            {
                "(+ 1\n2\r3\r\n4\t5)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "+", 2),
                new ExpectedToken(TokenType.Number, "1", 4),
                new ExpectedToken(TokenType.Number, "2", 1, 2),
                new ExpectedToken(TokenType.Number, "3", 1, 3),
                new ExpectedToken(TokenType.Number, "4", 1, 5), // line 5 maybe should become 4 because of consecutive \r\n
                new ExpectedToken(TokenType.Number, "5", 3, 5),
                new ExpectedToken(TokenType.RParen, ")", 4, 5),
                new ExpectedToken(TokenType.EOF, "", 5, 5)
            };

            yield return new object[]
            {
                "(and true false)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "and", 2),
                new ExpectedToken(TokenType.Boolean, "true", 6),
                new ExpectedToken(TokenType.Boolean, "false", 11),
                new ExpectedToken(TokenType.RParen, ")", 16),
                new ExpectedToken(TokenType.EOF, "", 17)
            };

            yield return new object[]
            {
                "(or false true)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "or", 2),
                new ExpectedToken(TokenType.Boolean, "false", 5),
                new ExpectedToken(TokenType.Boolean, "true", 11),
                new ExpectedToken(TokenType.RParen, ")", 15),
                new ExpectedToken(TokenType.EOF, "", 16)
            };

            yield return new object[]
            {
                "(xor false true)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "xor", 2),
                new ExpectedToken(TokenType.Boolean, "false", 6),
                new ExpectedToken(TokenType.Boolean, "true", 12),
                new ExpectedToken(TokenType.RParen, ")", 16),
                new ExpectedToken(TokenType.EOF, "", 17)
            };

            yield return new object[]
            {
                @"(+
1
2)",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "+", 2),
                new ExpectedToken(TokenType.Number, "1", 1, 3), // maybe should be 2 and next all 4 because of consecutive \r\n
                new ExpectedToken(TokenType.Number, "2", 1, 5),
                new ExpectedToken(TokenType.RParen, ")", 2, 5),
                new ExpectedToken(TokenType.EOF, "", 3, 5)
            };

            yield return new object[]
            {
                "(+ 10 15);This is a comment, () \t\";",
                new ExpectedToken(TokenType.LParen, "(", 1),
                new ExpectedToken(TokenType.Identifier, "+", 2),
                new ExpectedToken(TokenType.Number, "10", 4),
                new ExpectedToken(TokenType.Number, "15", 7),
                new ExpectedToken(TokenType.RParen, ")", 9),
                new ExpectedToken(TokenType.EOF, "", 36),
            };

            yield return new object[]
            {
                @"; A function for returning the max of two numbers.
(define (max num1 num2) ; num1 and num2 are numbers
    ; If num1 is greater than num2, return num1. Otherwise, return num2.
    (if (> num1 num2) num1
        num2))

(max 55 21); Expecting 55.",
                new ExpectedToken(TokenType.LParen, "(", 1, 3),
                new ExpectedToken(TokenType.Definition, "define", 2, 3),
                new ExpectedToken(TokenType.LParen, "(", 9, 3),
                new ExpectedToken(TokenType.Identifier, "max", 10, 3),
                new ExpectedToken(TokenType.Identifier, "num1", 14, 3),
                new ExpectedToken(TokenType.Identifier, "num2", 19, 3),
                new ExpectedToken(TokenType.RParen, ")", 23, 3),
                new ExpectedToken(TokenType.LParen, "(", 5, 7),
                new ExpectedToken(TokenType.If, "if", 6, 7),
                new ExpectedToken(TokenType.LParen, "(", 9, 7),
                new ExpectedToken(TokenType.Identifier, ">", 10, 7),
                new ExpectedToken(TokenType.Identifier, "num1", 12, 7),
                new ExpectedToken(TokenType.Identifier, "num2", 17, 7),
                new ExpectedToken(TokenType.RParen, ")", 21, 7),
                new ExpectedToken(TokenType.Identifier, "num1", 23, 7),
                new ExpectedToken(TokenType.Identifier, "num2", 9, 9),
                new ExpectedToken(TokenType.RParen, ")", 13, 9),
                new ExpectedToken(TokenType.RParen, ")", 14, 9),
                new ExpectedToken(TokenType.LParen, "(", 1, 13),
                new ExpectedToken(TokenType.Identifier, "max", 2, 13),
                new ExpectedToken(TokenType.Number, "55", 6, 13),
                new ExpectedToken(TokenType.Number, "21", 9, 13),
                new ExpectedToken(TokenType.RParen, ")", 11, 13),
                new ExpectedToken(TokenType.EOF, "", 27, 13),
            };
        }

        public record ExpectedToken(TokenType TokenType, string Value, uint Position = 0, uint Line = 1);

        [Theory]
        [MemberData(nameof(GetTokens))]
        public void Tokenize_Expression_ReturnsCorrectTokens(string expression, params ExpectedToken[] expectedTokens)
        {
            // Arrange
            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(expectedTokens.Length, tokens.Count);
            for (int i = 0; i < expectedTokens.Length; i++)
            {
                var expectedToken = expectedTokens[i];
                Assert.Equal(expectedToken.TokenType, tokens[i].Type);
                Assert.Equal(expectedToken.Value, tokens[i].Value);
                Assert.Equal(expectedToken.Line, tokens[i].Line);
                Assert.Equal(expectedToken.Position, tokens[i].Position);
            }
        }
    }
}