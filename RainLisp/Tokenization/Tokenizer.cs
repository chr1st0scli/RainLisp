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

            uint line = 1, position = 1;
            bool charInstring = false, charInComment = false;
            StringTokenizer? stringTokenizer = null;

            void RegisterToken(string value, TokenType tokenType, uint tokenPosition)
            {
                var token = new Token { Value = value, Type = tokenType, Line = line, Position = tokenPosition };
                tokens.Add(token);
            }

            void RegisterStringLiteralToken()
            {
                charInstring = false;
                RegisterToken(stringTokenizer!.GetString(), TokenType.String, position);
                stringTokenizer = null;
            }

            void RegisterUnknownToken()
            {
                string tokenValue = tokenStringBuilder.ToString();
                tokenStringBuilder.Clear();

                if (string.IsNullOrWhiteSpace(tokenValue))
                    return;

                RegisterToken(tokenValue, GetTokenType(tokenValue), position - (uint)tokenValue.Length);
            }

            void ChangeLine()
            {
                line++;
                position = 0;
            }

            foreach (char c in expression)
            {
                if (charInstring)
                    stringTokenizer!.AddToString(c);

                else if (charInComment)
                {
                    // Disregard a character in a comment, but end the comment when reached a new line.
                    if (c == CARRIAGE_RETURN || c == NEW_LINE)
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
                    RegisterToken(c.ToString(), TokenType.LParen, position);
                }
                else if (c == RPAREN)
                {
                    RegisterUnknownToken();
                    RegisterToken(c.ToString(), TokenType.RParen, position);
                }
                else if (c == CARRIAGE_RETURN || c == NEW_LINE)
                {
                    RegisterUnknownToken();
                    ChangeLine();
                }
                else if (c == SPACE || c == TAB)
                    RegisterUnknownToken();
                
                else
                    tokenStringBuilder.Append(c);

                position++;
            }

            RegisterUnknownToken();
            RegisterToken(string.Empty, TokenType.EOF, position);

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
