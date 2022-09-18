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

            uint lineNumber = 0, position = 0;
            bool tokenInstring = false;
            StringTokenizer? stringTokenizer = null;

            void RegisterToken(string value, TokenType tokenType)
            {
                var token = new Token { Value = value, Type = tokenType, LineNumber = lineNumber, Position = position };
                tokens.Add(token);
            }

            void RegisterStringLiteralToken()
            {
                tokenInstring = false;
                RegisterToken(stringTokenizer!.GetString(), TokenType.String);
                stringTokenizer = null;
            }

            void RegisterUnknownToken()
            {
                string tokenValue = tokenStringBuilder.ToString();
                tokenStringBuilder.Clear();

                if (string.IsNullOrWhiteSpace(tokenValue))
                    return;

                RegisterToken(tokenValue, GetTokenType(tokenValue));
            }

            foreach (char c in expression)
            {
                position++; // TODO should be 0-based.

                if (tokenInstring)
                    stringTokenizer!.AddToString(c);
                
                // Start of a string.
                else if (c == DOUBLE_QUOTE)
                {
                    tokenInstring = true;
                    stringTokenizer = new StringTokenizer(RegisterStringLiteralToken);
                }
                else if (c == LPAREN)
                {
                    RegisterUnknownToken();
                    RegisterToken(c.ToString(), TokenType.LParen);
                }
                else if (c == RPAREN)
                {
                    RegisterUnknownToken();
                    RegisterToken(c.ToString(), TokenType.RParen);
                }
                else if (c == CARRIAGE_RETURN || c == NEW_LINE)
                {
                    RegisterUnknownToken();

                    lineNumber++;
                    position = 0;
                }
                else if (c == SPACE || c == TAB)
                    RegisterUnknownToken();
                
                else
                    tokenStringBuilder.Append(c);
            }
            RegisterUnknownToken();

            tokens.Add(new Token { Type = TokenType.EOF });

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
