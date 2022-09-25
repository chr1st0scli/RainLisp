using static RainLisp.Grammar.SpecialSymbols;
using static RainLisp.Grammar.Keywords;
using System.Text;

namespace RainLisp.Tokenization
{
    public class Tokenizer : ITokenizer
    {
        public IList<Token> Tokenize(string expression)
        {
            ArgumentNullException.ThrowIfNull(expression, nameof(expression));

            expression = expression.Trim();
            var tokens = new List<Token>();
            var tokenStringBuilder = new StringBuilder();

            uint line = 1, charPosition = 1;
            bool charInstring = false, charInComment = false;
            StringTokenizer? stringTokenizer = null;

            void RegisterToken(string value, TokenType tokenType, uint position)
            {
                var token = new Token { Value = value, Type = tokenType, Line = line, Position = position };
                tokens.Add(token);
            }

            void RegisterStringLiteralToken()
            {
                charInstring = false;
                RegisterToken(stringTokenizer!.GetString(), TokenType.String, charPosition - (uint)stringTokenizer.CharactersProcessed);
                stringTokenizer = null;
            }

            void RegisterUnknownToken()
            {
                if (tokenStringBuilder.Length == 0)
                    return;

                string tokenValue = tokenStringBuilder.ToString();
                tokenStringBuilder.Clear();

                if (string.IsNullOrWhiteSpace(tokenValue))
                    return;

                RegisterToken(tokenValue, GetTokenType(tokenValue), charPosition - (uint)tokenValue.Length);
            }

            void ChangeLine()
            {
                line++;
                charPosition = 0;
            }

            void PlatformBounceNextNewLine(ref int i)
            {
                // Skip the next \n character, so that \r\n is treated as a single new line if the platform dictates it.
                if (i < expression.Length - 1 && expression[i + 1] == NEW_LINE && System.Environment.NewLine == $"{CARRIAGE_RETURN}{NEW_LINE}")
                    i++;
            }

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (charInstring)
                    stringTokenizer!.AddToString(c);

                else if (charInComment)
                {
                    // Disregard a character in a comment, but end the comment when reached a new line.
                    if (c == CARRIAGE_RETURN)
                    {
                        charInComment = false;
                        ChangeLine();
                        PlatformBounceNextNewLine(ref i);
                    }
                    else if (c == NEW_LINE)
                    {
                        charInComment = false;
                        ChangeLine();
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
                    //RegisterUnknownToken(); ???
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
                }
                else if (c == NEW_LINE)
                {
                    RegisterUnknownToken();
                    ChangeLine();
                }
                else if (c == SPACE || c == TAB)
                    RegisterUnknownToken();
                
                else
                    tokenStringBuilder.Append(c);

                charPosition++;
            }

            RegisterUnknownToken();
            RegisterToken(string.Empty, TokenType.EOF, charPosition);

            return tokens;
        }

        private static TokenType GetTokenType(string token)
        {
            TokenType GetOtherType()
            {
                if (double.TryParse(token, out double _))
                    return TokenType.Number;

                else
                    return TokenType.Identifier;
            }

            return token switch
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
                _ => GetOtherType()
            };
        }
    }
}
