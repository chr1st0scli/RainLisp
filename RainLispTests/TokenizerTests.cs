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

        public record ExpectedToken(TokenType TokenType, string Value, uint Position, uint Line = 1);

        public static IEnumerable<object[]> GetTokens()
        {
            static uint PickLine(uint winOS, uint otherOS) => Environment.NewLine == "\r\n" ? winOS : otherOS;
            static ExpectedToken Expect(TokenType tokenType, string value, uint position, uint line = 1) => new ExpectedToken(tokenType, value, position, line);

            yield return new object[] { "1", Expect(TokenType.Number, "1", 1), Expect(TokenType.EOF, "", 2) };
            yield return new object[] { "+1", Expect(TokenType.Number, "+1", 1), Expect(TokenType.EOF, "", 3) };
            yield return new object[] { "-1", Expect(TokenType.Number, "-1", 1), Expect(TokenType.EOF, "", 3) };
            yield return new object[] { "12.3456", Expect(TokenType.Number, "12.3456", 1), Expect(TokenType.EOF, "", 8) };

            yield return new object[] { "\"helloworld\"", Expect(TokenType.String, "helloworld", 1), Expect(TokenType.EOF, "", 13) };
            yield return new object[] { "\"hello world\"", Expect(TokenType.String, "hello world", 1), Expect(TokenType.EOF, "", 14) };
            yield return new object[] { "\"hello  world\"", Expect(TokenType.String, "hello  world", 1), Expect(TokenType.EOF, "", 15) };
            yield return new object[] { @"""hello \""wonderful\"" world""", Expect(TokenType.String, "hello \"wonderful\" world", 1), Expect(TokenType.EOF, "", 28) };
            yield return new object[] { @"""hello \\ wonderful \\ world""", Expect(TokenType.String, "hello \\ wonderful \\ world", 1), Expect(TokenType.EOF, "", 30) };

            yield return new object[] { "12\"hi\"", Expect(TokenType.Number, "12", 1), Expect(TokenType.String, "hi", 3), Expect(TokenType.EOF, "", 7) };
            yield return new object[] { "12 \"hi\"", Expect(TokenType.Number, "12", 1), Expect(TokenType.String, "hi", 4), Expect(TokenType.EOF, "", 8) };

            yield return new object[] { "abc\"hi\"", Expect(TokenType.Identifier, "abc", 1), Expect(TokenType.String, "hi", 4), Expect(TokenType.EOF, "", 8) };
            yield return new object[] { "abc \"hi\"", Expect(TokenType.Identifier, "abc", 1), Expect(TokenType.String, "hi", 5), Expect(TokenType.EOF, "", 9) };

            yield return new object[] { "\"hi\"12", Expect(TokenType.String, "hi", 1), Expect(TokenType.Number, "12", 5), Expect(TokenType.EOF, "", 7) };
            yield return new object[] { "\"hi\" 12", Expect(TokenType.String, "hi", 1), Expect(TokenType.Number, "12", 6), Expect(TokenType.EOF, "", 8) };

            yield return new object[] { "\"hi\"abc", Expect(TokenType.String, "hi", 1), Expect(TokenType.Identifier, "abc", 5), Expect(TokenType.EOF, "", 8) };
            yield return new object[] { "\"hi\" abc", Expect(TokenType.String, "hi", 1), Expect(TokenType.Identifier, "abc", 6), Expect(TokenType.EOF, "", 9) };

            yield return new object[] { "true", Expect(TokenType.Boolean, "true", 1), Expect(TokenType.EOF, "", 5) };
            yield return new object[] { "false", Expect(TokenType.Boolean, "false", 1), Expect(TokenType.EOF, "", 6) };
            yield return new object[] { "a", Expect(TokenType.Identifier, "a", 1), Expect(TokenType.EOF, "", 2) };
            yield return new object[] { "abc", Expect(TokenType.Identifier, "abc", 1), Expect(TokenType.EOF, "", 4) };
            yield return new object[] { "+", Expect(TokenType.Identifier, "+", 1), Expect(TokenType.EOF, "", 2) };

            yield return new object[]
            {
                "(quote abcd)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Quote, "quote", 2),
                Expect(TokenType.Identifier, "abcd", 8),
                Expect(TokenType.RParen, ")", 12),
                Expect(TokenType.EOF, "", 13)
            };

            yield return new object[]
            {
                "(set! ab 15)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Assignment, "set!", 2),
                Expect(TokenType.Identifier, "ab", 7),
                Expect(TokenType.Number, "15", 10),
                Expect(TokenType.RParen, ")", 12),
                Expect(TokenType.EOF, "", 13)
            };

            yield return new object[]
            {
                "(set! ab 15.4)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Assignment, "set!", 2),
                Expect(TokenType.Identifier, "ab", 7),
                Expect(TokenType.Number, "15.4", 10),
                Expect(TokenType.RParen, ")", 14),
                Expect(TokenType.EOF, "", 15)
            };

            yield return new object[]
            {
                "(define ab 15)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Definition, "define", 2),
                Expect(TokenType.Identifier, "ab", 9),
                Expect(TokenType.Number, "15", 12),
                Expect(TokenType.RParen, ")", 14),
                Expect(TokenType.EOF, "", 15)
            };

            yield return new object[]
            {
                "(define ab 15.32)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Definition, "define", 2),
                Expect(TokenType.Identifier, "ab", 9),
                Expect(TokenType.Number, "15.32", 12),
                Expect(TokenType.RParen, ")", 17),
                Expect(TokenType.EOF, "", 18)
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

            // Set different positions for the same expression.
            var defineExpectedTokens2 = defineExpectedTokens
                .Zip(new[] { 1, 2, 8, 9, 13, 15, 16, 17, 18, 20, 22, 23, 24, 25 })
                .Select(tokenPositionPair => Expect(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
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

            // Set different positions for the same expression.
            var ifExpectedTokens2 = ifExpectedTokens
                .Zip(new[] { 1, 2, 4, 5, 7, 9, 10, 12, 14, 15, 16 })
                .Select(tokenPositionPair => Expect(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
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

            // Set different positions for the same expression.
            var condExpectedTokens2 = condExpectedTokens
                .Zip(new[] { 1, 2, 6, 7, 8, 11, 13, 14, 16, 17, 18, 19, 20, 23, 25, 26, 28, 29, 30, 31, 36, 37, 38, 39 })
                .Select(tokenPositionPair => Expect(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(cond((>= 1 0) 0)((<= 2 1) 1)(else 3))"
            }.Concat(condExpectedTokens2).ToArray();

            yield return new object[]
            {
                "(begin 1 2 3 4)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Begin, "begin", 2),
                Expect(TokenType.Number, "1", 8),
                Expect(TokenType.Number, "2", 10),
                Expect(TokenType.Number, "3", 12),
                Expect(TokenType.Number, "4", 14),
                Expect(TokenType.RParen, ")", 15),
                Expect(TokenType.EOF, "", 16)
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

            // Set different positions for the same expression.
            var lambdaExpectedTokens2 = lambdaExpectedTokens
                .Zip(new[] { 1, 2, 8, 9, 11, 12, 13, 14, 16, 18, 19, 20, 21 })
                .Select(tokenPositionPair => Expect(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
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

            // Set different positions for the same expression.
            var letExpectedTokens2 = letExpectedTokens
                .Zip(new[] { 1, 2, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 17, 19, 20, 21, 22, 23, 25, 27, 29, 30, 31, 32 })
                .Select(tokenPositionPair => Expect(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(let((a 1)(b 2)(c 3))(+ a b c))"
            }.Concat(letExpectedTokens2).ToArray();

            yield return new object[]
            {
                "(+ 1 2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "+", 2),
                Expect(TokenType.Number, "1", 4),
                Expect(TokenType.Number, "2", 6),
                Expect(TokenType.RParen, ")", 7),
                Expect(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(+ +1 -2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "+", 2),
                Expect(TokenType.Number, "+1", 4),
                Expect(TokenType.Number, "-2", 7),
                Expect(TokenType.RParen, ")", 9),
                Expect(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(* 1 2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "*", 2),
                Expect(TokenType.Number, "1", 4),
                Expect(TokenType.Number, "2", 6),
                Expect(TokenType.RParen, ")", 7),
                Expect(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(* -1 +2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "*", 2),
                Expect(TokenType.Number, "-1", 4),
                Expect(TokenType.Number, "+2", 7),
                Expect(TokenType.RParen, ")", 9),
                Expect(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(/ 1 2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "/", 2),
                Expect(TokenType.Number, "1", 4),
                Expect(TokenType.Number, "2", 6),
                Expect(TokenType.RParen, ")", 7),
                Expect(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(/ -1 +2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "/", 2),
                Expect(TokenType.Number, "-1", 4),
                Expect(TokenType.Number, "+2", 7),
                Expect(TokenType.RParen, ")", 9),
                Expect(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(% 1 2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "%", 2),
                Expect(TokenType.Number, "1", 4),
                Expect(TokenType.Number, "2", 6),
                Expect(TokenType.RParen, ")", 7),
                Expect(TokenType.EOF, "", 8)
            };

            yield return new object[]
            {
                "(% +1 -2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "%", 2),
                Expect(TokenType.Number, "+1", 4),
                Expect(TokenType.Number, "-2", 7),
                Expect(TokenType.RParen, ")", 9),
                Expect(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(+ 0 1 2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "+", 2),
                Expect(TokenType.Number, "0", 4),
                Expect(TokenType.Number, "1", 6),
                Expect(TokenType.Number, "2", 8),
                Expect(TokenType.RParen, ")", 9),
                Expect(TokenType.EOF, "", 10)
            };

            yield return new object[]
            {
                "(+ 0 -1 2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "+", 2),
                Expect(TokenType.Number, "0", 4),
                Expect(TokenType.Number, "-1", 6),
                Expect(TokenType.Number, "2", 9),
                Expect(TokenType.RParen, ")", 10),
                Expect(TokenType.EOF, "", 11)
            };

            yield return new object[] { "()", Expect(TokenType.LParen, "(", 1), Expect(TokenType.RParen, ")", 2), Expect(TokenType.EOF, "", 3) };

            var tokensOnDifferentLines = new ExpectedToken[]
            {
                new(TokenType.LParen, "(", 1),
                new(TokenType.Identifier, "+", 2),
                new(TokenType.Number, "1", 4),
                new(TokenType.Number, "2", 1, 2),
                new(TokenType.Number, "3", 1, 3),
                new(TokenType.Number, "4", 1, PickLine(4, 5)),
                new(TokenType.Number, "5", 3, PickLine(4, 5)),
                new(TokenType.RParen, ")", 4, PickLine(4, 5)),
                new(TokenType.EOF, "", 5, PickLine(4, 5))
            };

            yield return new object[]
            {
                "(+ 1\n2\r3\r\n4\t5)",
            }.Concat(tokensOnDifferentLines).ToArray();

            // Set different lines for the same expression.
            var tokensOnDifferentLines2 = tokensOnDifferentLines
                .Zip(new[] { 1, 1, 1, 2, 3, 5, 5, 5, 5 })
                .Select(tokenPositionPair => Expect(tokenPositionPair.First.TokenType, tokenPositionPair.First.Value, tokenPositionPair.First.Position, (uint)tokenPositionPair.Second))
                .ToArray();

            yield return new object[]
            {
                "(+ 1\n2\r3\n\r4\t5)",
            }.Concat(tokensOnDifferentLines2).ToArray();

            yield return new object[]
            {
                "(+ 1\n2\r3\r\r4\t5)",
            }.Concat(tokensOnDifferentLines2).ToArray();

            yield return new object[]
            {
                "(+ 1\n2\r3\n\n4\t5)",
            }.Concat(tokensOnDifferentLines2).ToArray();

            yield return new object[]
            {
                "(and true false)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "and", 2),
                Expect(TokenType.Boolean, "true", 6),
                Expect(TokenType.Boolean, "false", 11),
                Expect(TokenType.RParen, ")", 16),
                Expect(TokenType.EOF, "", 17)
            };

            yield return new object[]
            {
                "(or false true)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "or", 2),
                Expect(TokenType.Boolean, "false", 5),
                Expect(TokenType.Boolean, "true", 11),
                Expect(TokenType.RParen, ")", 15),
                Expect(TokenType.EOF, "", 16)
            };

            yield return new object[]
            {
                "(xor false true)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "xor", 2),
                Expect(TokenType.Boolean, "false", 6),
                Expect(TokenType.Boolean, "true", 12),
                Expect(TokenType.RParen, ")", 16),
                Expect(TokenType.EOF, "", 17)
            };

            yield return new object[]
            {
                @"(+
1
2)",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "+", 2),
                Expect(TokenType.Number, "1", 1, PickLine(2, 3)),
                Expect(TokenType.Number, "2", 1, PickLine(3, 5)),
                Expect(TokenType.RParen, ")", 2, PickLine(3, 5)),
                Expect(TokenType.EOF, "", 3, PickLine(3, 5))
            };

            yield return new object[]
            {
                "(+ 10 15);This is a comment, () \t\";",
                Expect(TokenType.LParen, "(", 1),
                Expect(TokenType.Identifier, "+", 2),
                Expect(TokenType.Number, "10", 4),
                Expect(TokenType.Number, "15", 7),
                Expect(TokenType.RParen, ")", 9),
                Expect(TokenType.EOF, "", 36),
            };

            yield return new object[]
            {
                @"; A function for returning the max of two numbers.
(define (max num1 num2) ; num1 and num2 are numbers
    ; If num1 is greater than num2, return num1. Otherwise, return num2.
    (if (> num1 num2) num1
        num2))

(max 55 21); Expecting 55.",
                Expect(TokenType.LParen, "(", 1, PickLine(2, 3)),
                Expect(TokenType.Definition, "define", 2, PickLine(2, 3)),
                Expect(TokenType.LParen, "(", 9, PickLine(2, 3)),
                Expect(TokenType.Identifier, "max", 10, PickLine(2, 3)),
                Expect(TokenType.Identifier, "num1", 14, PickLine(2, 3)),
                Expect(TokenType.Identifier, "num2", 19, PickLine(2, 3)),
                Expect(TokenType.RParen, ")", 23, PickLine(2, 3)),
                Expect(TokenType.LParen, "(", 5, PickLine(4, 7)),
                Expect(TokenType.If, "if", 6, PickLine(4, 7)),
                Expect(TokenType.LParen, "(", 9, PickLine(4, 7)),
                Expect(TokenType.Identifier, ">", 10, PickLine(4, 7)),
                Expect(TokenType.Identifier, "num1", 12, PickLine(4, 7)),
                Expect(TokenType.Identifier, "num2", 17, PickLine(4, 7)),
                Expect(TokenType.RParen, ")", 21, PickLine(4, 7)),
                Expect(TokenType.Identifier, "num1", 23, PickLine(4, 7)),
                Expect(TokenType.Identifier, "num2", 9, PickLine(5, 9)),
                Expect(TokenType.RParen, ")", 13, PickLine(5, 9)),
                Expect(TokenType.RParen, ")", 14, PickLine(5, 9)),
                Expect(TokenType.LParen, "(", 1, PickLine(7, 13)),
                Expect(TokenType.Identifier, "max", 2, PickLine(7, 13)),
                Expect(TokenType.Number, "55", 6, PickLine(7, 13)),
                Expect(TokenType.Number, "21", 9, PickLine(7, 13)),
                Expect(TokenType.RParen, ")", 11, PickLine(7, 13)),
                Expect(TokenType.EOF, "", 27, PickLine(7, 13)),
            };
        }

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

        [Theory]
        [InlineData("\"hi")]
        [InlineData("\"hi 12")]
        [InlineData("13\"hi")]
        [InlineData(@"""hello \\"" wonderful \\ world""")]
        public void Tokenize_UnclosedString_Throws(string expression)
        {
            // Arrange
            Action action = () => _tokenizer.Tokenize(expression);

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }
    }
}