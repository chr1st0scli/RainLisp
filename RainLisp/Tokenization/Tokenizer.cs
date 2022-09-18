using static RainLisp.Grammar.SpecialSymbols;
using static RainLisp.Grammar.Keywords;

namespace RainLisp.Tokenization
{
    public class Tokenizer : ITokenizer
    {
        public IList<Token> Tokenize(string expression)
        {
            ArgumentNullException.ThrowIfNull(expression, nameof(expression));

            expression = expression.Trim();
            var tokens = new List<Token>();
            string tokenValue = string.Empty;

            uint lineNumber = 0, position = 0;
            bool tokenInstring = false;
            StringTokenizer? stringTokenizer = null;

            void AddCharToToken(char c) => tokenValue += c;

            void RegisterToken(TokenType tokenType)
            {
                // DELETE
                //if (string.IsNullOrWhiteSpace(token))
                //    return;

                var token = new Token { Value = tokenValue, Type = tokenType, LineNumber = lineNumber, Position = position };
                tokens.Add(token);
                tokenValue = string.Empty;
            }

            void RegisterSingleCharToken(char c, TokenType tokenType)
            {
                tokenValue = c.ToString();
                RegisterToken(tokenType);
            }

            void RegisterStringLiteralToken()
            {
                tokenInstring = false;
                tokenValue = stringTokenizer!.GetString();
                RegisterToken(TokenType.String);
                stringTokenizer = null;
            }

            void RegisterUnknownToken()
            {
                if (string.IsNullOrWhiteSpace(tokenValue))
                    return;

                RegisterToken(GetTokenType(tokenValue));
            }

            foreach (char c in expression)
            {
                position++; // TODO should be 0-based.

                if (tokenInstring)
                {
                    stringTokenizer!.AddToString(c);
                }
                // Start of a string.
                else if (c == DOUBLE_QUOTE)
                {
                    tokenInstring = true;
                    stringTokenizer = new StringTokenizer(RegisterStringLiteralToken);
                }
                else if (c == LPAREN)
                {
                    RegisterUnknownToken();
                    RegisterSingleCharToken(c, TokenType.LParen);
                }
                else if (c == RPAREN)
                {
                    RegisterUnknownToken();
                    RegisterSingleCharToken(c, TokenType.RParen);
                }
                else if (c == CARRIAGE_RETURN || c == NEW_LINE)
                {
                    RegisterUnknownToken();

                    lineNumber++;
                    position = 0;
                }
                else if (c == SPACE || c == TAB)
                {
                    RegisterUnknownToken();
                }
                else
                {
                    AddCharToToken(c);
                }
            }
            RegisterUnknownToken();

            tokens.Add(new Token { Type = TokenType.EOF });

            return tokens;
        }

        private static TokenType GetTokenType(string token)
        {
            ArgumentNullException.ThrowIfNull(token, nameof(token));

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Invalid token.", nameof(token));

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
