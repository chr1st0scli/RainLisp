using RainLisp.Tokenization;
using static RainLisp.Tokenization.TokenType;

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
            static ExpectedToken Expect(TokenType tokenType, string value, uint position, uint line = 1) => new(tokenType, value, position, line);

            yield return new object[] { null!, Expect(EOF, "", 1) };
            yield return new object[] { "", Expect(EOF, "", 1) };
            yield return new object[] { " ", Expect(EOF, "", 2) };
            yield return new object[] { "\n", Expect(EOF, "", 1, 2) };

            yield return new object[] { "1", Expect(Number, "1", 1), Expect(EOF, "", 2) };
            yield return new object[] { "+1", Expect(Number, "+1", 1), Expect(EOF, "", 3) };
            yield return new object[] { "-1", Expect(Number, "-1", 1), Expect(EOF, "", 3) };
            yield return new object[] { "12.3456", Expect(Number, "12.3456", 1), Expect(EOF, "", 8) };

            #region String literals.
            yield return new object[] { "\"\"", Expect(TokenType.String, "", 1), Expect(EOF, "", 3) };
            yield return new object[] { "\" \"", Expect(TokenType.String, " ", 1), Expect(EOF, "", 4) };
            yield return new object[] { "\"helloworld\"", Expect(TokenType.String, "helloworld", 1), Expect(EOF, "", 13) };
            yield return new object[] { "\"hello world\"", Expect(TokenType.String, "hello world", 1), Expect(EOF, "", 14) };
            yield return new object[] { "\"hello  world\"", Expect(TokenType.String, "hello  world", 1), Expect(EOF, "", 15) };
            yield return new object[] { "\"hello\tworld\"", Expect(TokenType.String, "hello\tworld", 1), Expect(EOF, "", 14) };

            // Valid escape sequences.
            yield return new object[] { "\"\\n\\n\"", Expect(TokenType.String, "\n\n", 1), Expect(EOF, "", 7) };
            yield return new object[] { @"""hello \""wonderful\"" world""", Expect(TokenType.String, "hello \"wonderful\" world", 1), Expect(EOF, "", 28) };
            yield return new object[] { @"""hello \\ wonderful \\ world""", Expect(TokenType.String, "hello \\ wonderful \\ world", 1), Expect(EOF, "", 30) };
            yield return new object[] { @"""\\""", Expect(TokenType.String, @"\", 1), Expect(EOF, "", 5) };
            yield return new object[] { @"""\\hi""", Expect(TokenType.String, @"\hi", 1), Expect(EOF, "", 7) };
            yield return new object[] { @"""\\\""""", Expect(TokenType.String, @"\""", 1), Expect(EOF, "", 7) };
            yield return new object[] { @"""hello\""world""", Expect(TokenType.String, @"hello""world", 1), Expect(EOF, "", 15) };
            yield return new object[] { @"""hello\\world""", Expect(TokenType.String, @"hello\world", 1), Expect(EOF, "", 15) };
            yield return new object[] { @"""hello\nworld""", Expect(TokenType.String, "hello\nworld", 1), Expect(EOF, "", 15) };
            yield return new object[] { @"""hello\rworld""", Expect(TokenType.String, "hello\rworld", 1), Expect(EOF, "", 15) };
            yield return new object[] { @"""hello\tworld""", Expect(TokenType.String, "hello\tworld", 1), Expect(EOF, "", 15) };

            // Strings adjacent to other tokens.
            yield return new object[] { "12\"hi\"", Expect(Number, "12", 1), Expect(TokenType.String, "hi", 3), Expect(EOF, "", 7) };
            yield return new object[] { "12 \"hi\"", Expect(Number, "12", 1), Expect(TokenType.String, "hi", 4), Expect(EOF, "", 8) };
            yield return new object[] { "abc\"hi\"", Expect(Identifier, "abc", 1), Expect(TokenType.String, "hi", 4), Expect(EOF, "", 8) };
            yield return new object[] { "abc \"hi\"", Expect(Identifier, "abc", 1), Expect(TokenType.String, "hi", 5), Expect(EOF, "", 9) };
            yield return new object[] { "\"hi\"12", Expect(TokenType.String, "hi", 1), Expect(Number, "12", 5), Expect(EOF, "", 7) };
            yield return new object[] { "\"hi\" 12", Expect(TokenType.String, "hi", 1), Expect(Number, "12", 6), Expect(EOF, "", 8) };
            yield return new object[] { "\"hi\"abc", Expect(TokenType.String, "hi", 1), Expect(Identifier, "abc", 5), Expect(EOF, "", 8) };
            yield return new object[] { "\"hi\" abc", Expect(TokenType.String, "hi", 1), Expect(Identifier, "abc", 6), Expect(EOF, "", 9) };
            yield return new object[] { "\"hello\"\"world\"", Expect(TokenType.String, "hello", 1), Expect(TokenType.String, "world", 8), Expect(EOF, "", 15) };
            yield return new object[] { "\"hello\" \"world\"", Expect(TokenType.String, "hello", 1), Expect(TokenType.String, "world", 9), Expect(EOF, "", 16) }; 
            #endregion

            yield return new object[] { "true", Expect(TokenType.Boolean, "true", 1), Expect(EOF, "", 5) };
            yield return new object[] { "false", Expect(TokenType.Boolean, "false", 1), Expect(EOF, "", 6) };
            yield return new object[] { "a", Expect(Identifier, "a", 1), Expect(EOF, "", 2) };
            yield return new object[] { "abc", Expect(Identifier, "abc", 1), Expect(EOF, "", 4) };
            yield return new object[] { "+", Expect(Identifier, "+", 1), Expect(EOF, "", 2) };

            yield return new object[]
            {
                "(quote abcd)",
                Expect(LParen, "(", 1),
                Expect(Quote, "quote", 2),
                Expect(Identifier, "abcd", 8),
                Expect(RParen, ")", 12),
                Expect(EOF, "", 13)
            };

            yield return new object[]
            {
                "(set! ab 15)",
                Expect(LParen, "(", 1),
                Expect(Assignment, "set!", 2),
                Expect(Identifier, "ab", 7),
                Expect(Number, "15", 10),
                Expect(RParen, ")", 12),
                Expect(EOF, "", 13)
            };

            yield return new object[]
            {
                "(set! ab 15.4)",
                Expect(LParen, "(", 1),
                Expect(Assignment, "set!", 2),
                Expect(Identifier, "ab", 7),
                Expect(Number, "15.4", 10),
                Expect(RParen, ")", 14),
                Expect(EOF, "", 15)
            };

            yield return new object[]
            {
                "(define ab 15)",
                Expect(LParen, "(", 1),
                Expect(Definition, "define", 2),
                Expect(Identifier, "ab", 9),
                Expect(Number, "15", 12),
                Expect(RParen, ")", 14),
                Expect(EOF, "", 15)
            };

            yield return new object[]
            {
                "(define ab 15.32)",
                Expect(LParen, "(", 1),
                Expect(Definition, "define", 2),
                Expect(Identifier, "ab", 9),
                Expect(Number, "15.32", 12),
                Expect(RParen, ")", 17),
                Expect(EOF, "", 18)
            };

            var defineExpectedTokens = new ExpectedToken[]
            {
                new(LParen, "(", 1),
                new(Definition, "define", 3),
                new(LParen, "(", 10),
                new(Identifier, "foo", 11),
                new(Identifier, "x", 15),
                new(Identifier, "y", 17),
                new(RParen, ")", 18),
                new(LParen, "(", 20),
                new(Identifier, "+", 21),
                new(Identifier, "x", 23),
                new(Identifier, "y", 25),
                new(RParen, ")", 26),
                new(RParen, ")", 28),
                new(EOF, "", 29)
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
                new(LParen, "(", 1),
                new(If, "if", 3),
                new(LParen, "(", 6),
                new(Identifier, ">", 7),
                new(Number, "1", 9),
                new(Number, "0", 11),
                new(RParen, ")", 12),
                new(Number, "1", 14),
                new(Number, "0", 16),
                new(RParen, ")", 18),
                new(EOF, "", 19)
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
                new(LParen, "(", 1),
                new(Cond, "cond", 3),
                new(LParen, "(", 8),
                new(LParen, "(", 10),
                new(Identifier, ">=", 12),
                new(Number, "1", 15),
                new(Number, "0", 17),
                new(RParen, ")", 18),
                new(Number, "0", 20),
                new(RParen, ")", 21),
                new(LParen, "(", 23),
                new(LParen, "(", 25),
                new(Identifier, "<=", 27),
                new(Number, "2", 30),
                new(Number, "1", 32),
                new(RParen, ")", 33),
                new(Number, "1", 35),
                new(RParen, ")", 36),
                new(LParen, "(", 38),
                new(Else, "else", 40),
                new(Number, "3", 45),
                new(RParen, ")", 46),
                new(RParen, ")", 48),
                new(EOF, "", 49)
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
                Expect(LParen, "(", 1),
                Expect(Begin, "begin", 2),
                Expect(Number, "1", 8),
                Expect(Number, "2", 10),
                Expect(Number, "3", 12),
                Expect(Number, "4", 14),
                Expect(RParen, ")", 15),
                Expect(EOF, "", 16)
            };

            var lambdaExpectedTokens = new ExpectedToken[]
            {
                new(LParen, "(", 1),
                new(Lambda, "lambda", 3),
                new(LParen, "(", 10),
                new(Identifier, "x", 11),
                new(Identifier, "y", 13),
                new(RParen, ")", 14),
                new(LParen, "(", 16),
                new(Identifier, "+", 17),
                new(Identifier, "x", 19),
                new(Identifier, "y", 21),
                new(RParen, ")", 22),
                new(RParen, ")", 24),
                new(EOF, "", 25)
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
                new(LParen, "(", 1),
                new(Let, "let", 3),
                new(LParen, "(", 7),
                new(LParen, "(", 8),
                new(Identifier, "a", 9),
                new(Number, "1", 11),
                new(RParen, ")", 12),
                new(LParen, "(", 14),
                new(Identifier, "b", 15),
                new(Number, "2", 17),
                new(RParen, ")", 18),
                new(LParen, "(", 20),
                new(Identifier, "c", 21),
                new(Number, "3", 23),
                new(RParen, ")", 24),
                new(RParen, ")", 25),
                new(LParen, "(", 27),
                new(Identifier, "+", 28),
                new(Identifier, "a", 30),
                new(Identifier, "b", 32),
                new(Identifier, "c", 34),
                new(RParen, ")", 35),
                new(RParen, ")", 37),
                new(EOF, "", 38)
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
                Expect(LParen, "(", 1),
                Expect(Identifier, "+", 2),
                Expect(Number, "1", 4),
                Expect(Number, "2", 6),
                Expect(RParen, ")", 7),
                Expect(EOF, "", 8)
            };

            yield return new object[]
            {
                "(+ +1 -2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "+", 2),
                Expect(Number, "+1", 4),
                Expect(Number, "-2", 7),
                Expect(RParen, ")", 9),
                Expect(EOF, "", 10)
            };

            yield return new object[]
            {
                "(* 1 2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "*", 2),
                Expect(Number, "1", 4),
                Expect(Number, "2", 6),
                Expect(RParen, ")", 7),
                Expect(EOF, "", 8)
            };

            yield return new object[]
            {
                "(* -1 +2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "*", 2),
                Expect(Number, "-1", 4),
                Expect(Number, "+2", 7),
                Expect(RParen, ")", 9),
                Expect(EOF, "", 10)
            };

            yield return new object[]
            {
                "(/ 1 2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "/", 2),
                Expect(Number, "1", 4),
                Expect(Number, "2", 6),
                Expect(RParen, ")", 7),
                Expect(EOF, "", 8)
            };

            yield return new object[]
            {
                "(/ -1 +2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "/", 2),
                Expect(Number, "-1", 4),
                Expect(Number, "+2", 7),
                Expect(RParen, ")", 9),
                Expect(EOF, "", 10)
            };

            yield return new object[]
            {
                "(% 1 2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "%", 2),
                Expect(Number, "1", 4),
                Expect(Number, "2", 6),
                Expect(RParen, ")", 7),
                Expect(EOF, "", 8)
            };

            yield return new object[]
            {
                "(% +1 -2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "%", 2),
                Expect(Number, "+1", 4),
                Expect(Number, "-2", 7),
                Expect(RParen, ")", 9),
                Expect(EOF, "", 10)
            };

            yield return new object[]
            {
                "(+ 0 1 2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "+", 2),
                Expect(Number, "0", 4),
                Expect(Number, "1", 6),
                Expect(Number, "2", 8),
                Expect(RParen, ")", 9),
                Expect(EOF, "", 10)
            };

            yield return new object[]
            {
                "(+ 0 -1 2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "+", 2),
                Expect(Number, "0", 4),
                Expect(Number, "-1", 6),
                Expect(Number, "2", 9),
                Expect(RParen, ")", 10),
                Expect(EOF, "", 11)
            };

            yield return new object[] { "()", Expect(LParen, "(", 1), Expect(RParen, ")", 2), Expect(EOF, "", 3) };

            var tokensOnDifferentLines = new ExpectedToken[]
            {
                new(LParen, "(", 1),
                new(Identifier, "+", 2),
                new(Number, "1", 4),
                new(Number, "2", 1, 2),
                new(Number, "3", 1, 3),
                new(Number, "4", 1, PickLine(4, 5)),
                new(Number, "5", 3, PickLine(4, 5)),
                new(RParen, ")", 4, PickLine(4, 5)),
                new(EOF, "", 5, PickLine(4, 5))
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
                Expect(LParen, "(", 1),
                Expect(Identifier, "and", 2),
                Expect(TokenType.Boolean, "true", 6),
                Expect(TokenType.Boolean, "false", 11),
                Expect(RParen, ")", 16),
                Expect(EOF, "", 17)
            };

            yield return new object[]
            {
                "(or false true)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "or", 2),
                Expect(TokenType.Boolean, "false", 5),
                Expect(TokenType.Boolean, "true", 11),
                Expect(RParen, ")", 15),
                Expect(EOF, "", 16)
            };

            yield return new object[]
            {
                "(xor false true)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "xor", 2),
                Expect(TokenType.Boolean, "false", 6),
                Expect(TokenType.Boolean, "true", 12),
                Expect(RParen, ")", 16),
                Expect(EOF, "", 17)
            };

            yield return new object[]
            {
                @"(+
1
2)",
                Expect(LParen, "(", 1),
                Expect(Identifier, "+", 2),
                Expect(Number, "1", 1, PickLine(2, 3)),
                Expect(Number, "2", 1, PickLine(3, 5)),
                Expect(RParen, ")", 2, PickLine(3, 5)),
                Expect(EOF, "", 3, PickLine(3, 5))
            };

            yield return new object[]
            {
                "(+ 10 15);This is a comment, () \t\";",
                Expect(LParen, "(", 1),
                Expect(Identifier, "+", 2),
                Expect(Number, "10", 4),
                Expect(Number, "15", 7),
                Expect(RParen, ")", 9),
                Expect(EOF, "", 36),
            };

            yield return new object[]
            {
                @"; A function for returning the max of two numbers.
(define (max num1 num2) ; num1 and num2 are numbers
    ; If num1 is greater than num2, return num1. Otherwise, return num2.
    (if (> num1 num2) num1
        num2))

(max 55 21); Expecting 55.",
                Expect(LParen, "(", 1, PickLine(2, 3)),
                Expect(Definition, "define", 2, PickLine(2, 3)),
                Expect(LParen, "(", 9, PickLine(2, 3)),
                Expect(Identifier, "max", 10, PickLine(2, 3)),
                Expect(Identifier, "num1", 14, PickLine(2, 3)),
                Expect(Identifier, "num2", 19, PickLine(2, 3)),
                Expect(RParen, ")", 23, PickLine(2, 3)),
                Expect(LParen, "(", 5, PickLine(4, 7)),
                Expect(If, "if", 6, PickLine(4, 7)),
                Expect(LParen, "(", 9, PickLine(4, 7)),
                Expect(Identifier, ">", 10, PickLine(4, 7)),
                Expect(Identifier, "num1", 12, PickLine(4, 7)),
                Expect(Identifier, "num2", 17, PickLine(4, 7)),
                Expect(RParen, ")", 21, PickLine(4, 7)),
                Expect(Identifier, "num1", 23, PickLine(4, 7)),
                Expect(Identifier, "num2", 9, PickLine(5, 9)),
                Expect(RParen, ")", 13, PickLine(5, 9)),
                Expect(RParen, ")", 14, PickLine(5, 9)),
                Expect(LParen, "(", 1, PickLine(7, 13)),
                Expect(Identifier, "max", 2, PickLine(7, 13)),
                Expect(Number, "55", 6, PickLine(7, 13)),
                Expect(Number, "21", 9, PickLine(7, 13)),
                Expect(RParen, ")", 11, PickLine(7, 13)),
                Expect(EOF, "", 27, PickLine(7, 13)),
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
        [InlineData("\"\\\"", 1, 1)]
        [InlineData("\"hello world.\\\"", 1, 1)]
        [InlineData("\"hi", 1, 1)]
        [InlineData("\"hi 12", 1, 1)]
        [InlineData("13\"hi", 1, 3)]
        [InlineData(@"""hello \\"" wonderful \\ world""", 1, 30)]
        [InlineData(@"a b c ""hi", 1, 7)]
        [InlineData(@"""hi a b c", 1, 1)]
        [InlineData(@" ""hi a b c", 1, 2)]
        [InlineData("a b c\n\"hi", 2, 1)]
        [InlineData("a b c\n\"hi\" d \"world", 2, 8)]
        [InlineData("\"hi\" \"world", 1, 6)]
        [InlineData("\"hi\"\na\n\"world", 3, 1)]
        [InlineData("\"hi\"\na\nd \"world", 3, 3)]
        public void Tokenize_NonTerminatedString_Throws(string expression, uint expectedLine, uint expectedPosition)
        {
            Tokenize_InvalidString_Throws<NonTerminatedStringException>(expression, expectedLine, expectedPosition);
        }

        [Theory]
        [InlineData("\"hello\nworld.\\\"", 1, 7, '\n')]
        [InlineData("\"hello\nworld.\"", 1, 7, '\n')]
        [InlineData("\"hello\rworld.\"", 1, 7, '\r')]
        [InlineData("\"hello\r\nworld.\"",1, 7, '\r')]
        [InlineData(@"""hello there
world.""", 1, 13, '\r')] // Note that if this test is ran on a unix platform, the expected character might not be \r.
        public void Tokenize_MultilineString_Throws(string expression, uint expectedLine, uint expectedPosition, char expectedCharacter)
        {
            var exception = Tokenize_InvalidString_Throws<InvalidStringCharacterException>(expression, expectedLine, expectedPosition);
            Assert.Equal(expectedCharacter, exception!.Character);
        }

        [Theory]
        [InlineData(@"""hello \a world""", 1, 9, 'a')]
        [InlineData(@"""hello \b world""", 1, 9, 'b')] // backspace is not supported.
        [InlineData(@"""hello \c world""", 1, 9, 'c')]
        [InlineData(@"""hello \d world""", 1, 9, 'd')]
        [InlineData(@"""hello \9 world""", 1, 9, '9')]
        public void Tokenize_InvalidEscapeSequenceInString_Throws(string expression, uint expectedLine, uint expectedPosition, char expectedCharacter)
        {
            var exception = Tokenize_InvalidString_Throws<InvalidEscapeSequenceException>(expression, expectedLine, expectedPosition);
            Assert.Equal(expectedCharacter, exception!.Character);
        }

        private TException? Tokenize_InvalidString_Throws<TException>(string expression, uint expectedLine, uint expectedPosition) where TException : TokenizationException
        {
            // Arrange
            TException? exception = null;

            // Act
            try
            {
                _tokenizer.Tokenize(expression);
            }
            catch (TException ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<TException>(exception);
            Assert.Equal(expectedLine, exception!.Line);
            Assert.Equal(expectedPosition, exception!.Position);

            return exception;
        }
    }
}