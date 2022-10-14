using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.Keywords;
using System.Text;

namespace RainLisp.Tokenization
{
    public class Tokenizer : ITokenizer
    {
        public IList<Token> Tokenize(string? expression)
        {
            var tokens = new List<Token>();
            uint line = 1, charPosition = 1;

            if (string.IsNullOrEmpty(expression))
            {
                RegisterEOF();
                return tokens;
            }

            bool charInstring = false, charInComment = false;
            var lexemeStringBuilder = new StringBuilder();
            StringTokenizer? stringTokenizer = null;

            #region Local Helper Methods.
            void RegisterToken(string value, TokenType tokenType, uint position)
            {
                var token = new Token { Value = value, Type = tokenType, Line = line, Position = position };
                tokens.Add(token);
            }

            void RegisterEOF()
                => RegisterToken(string.Empty, TokenType.EOF, charPosition);

            void RegisterStringLiteralToken()
            {
                charInstring = false;
                RegisterToken(stringTokenizer!.GetString(), TokenType.String, GetLastStringStartPosition());
                stringTokenizer = null;
            }

            void RegisterUnknownToken()
            {
                if (lexemeStringBuilder.Length == 0)
                    return;

                string value = lexemeStringBuilder.ToString();
                lexemeStringBuilder.Clear();

                if (string.IsNullOrWhiteSpace(value))
                    return;

                RegisterToken(value, GetTokenType(value), charPosition - (uint)value.Length);
            }

            void ChangeLine()
            {
                line++;
                charPosition = 1;
            }

            void PlatformBounceNextNewLine(ref int i)
            {
                // Skip the next \n character, so that \r\n is treated as a single new line if the platform dictates it.
                if (i < expression.Length - 1 && expression[i + 1] == NEW_LINE && System.Environment.NewLine == $"{CARRIAGE_RETURN}{NEW_LINE}")
                    i++;
            }

            // The string token's position relates to the actual number of characters typed in the string and not the resulting string value's length.
            uint GetLastStringStartPosition()
                => charPosition - stringTokenizer!.CharactersProcessed - 1; 
            #endregion

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (charInstring)
                    stringTokenizer!.AddToString(c, line, charPosition);

                // Disregard a character in a comment except a new line that ends it.
                else if (charInComment)
                {
                    if (c == CARRIAGE_RETURN)
                    {
                        charInComment = false;
                        ChangeLine();
                        PlatformBounceNextNewLine(ref i);
                        continue; // Skip advancing character position.
                    }
                    else if (c == NEW_LINE)
                    {
                        charInComment = false;
                        ChangeLine();
                        continue; // Skip advancing character position.
                    }
                }
                // Start of a comment.
                else if (c == COMMENT)
                {
                    RegisterUnknownToken();
                    charInComment = true;
                }
                // Start of a string.
                else if (c == DOUBLE_QUOTE)
                {
                    RegisterUnknownToken();
                    charInstring = true;
                    stringTokenizer = new StringTokenizer(RegisterStringLiteralToken);
                }
                else if (c == LPAREN)
                {
                    RegisterUnknownToken();
                    RegisterToken(c.ToString(), TokenType.LParen, charPosition);
                }
                else if (c == RPAREN)
                {
                    RegisterUnknownToken();
                    RegisterToken(c.ToString(), TokenType.RParen, charPosition);
                }
                else if (c == CARRIAGE_RETURN)
                {
                    RegisterUnknownToken();
                    ChangeLine();
                    PlatformBounceNextNewLine(ref i);
                    continue; // Skip advancing character position.
                }
                else if (c == NEW_LINE)
                {
                    RegisterUnknownToken();
                    ChangeLine();
                    continue; // Skip advancing character position.
                }
                else if (c == SPACE || c == TAB)
                    RegisterUnknownToken();
                
                else
                    lexemeStringBuilder.Append(c);

                charPosition++;
            }

            if (charInstring)
                throw new NonTerminatedStringException(line, GetLastStringStartPosition());

            RegisterUnknownToken();
            RegisterEOF();

            return tokens;
        }

        private static TokenType GetTokenType(string value)
        {
            TokenType GetOtherType()
            {
                if (double.TryParse(value, out double _))
                    return TokenType.Number;

                else
                    return TokenType.Identifier;
            }

            return value switch
            {
                TRUE or FALSE => TokenType.Boolean,
                QUOTE => TokenType.Quote,
                SET => TokenType.Assignment,
                DEFINE => TokenType.Definition,
                IF => TokenType.If,
                COND => TokenType.Cond,
                ELSE => TokenType.Else,
                BEGIN => TokenType.Begin,
                LAMBDA => TokenType.Lambda,
                LET => TokenType.Let,
                AND => TokenType.And,
                OR => TokenType.Or,
                _ => GetOtherType()
            };
        }
    }
}
