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
            string token = string.Empty;

            uint lineNumber = 0, position = 0;

            void AddCharToToken(char c) => token += c;

            void RegisterUnknownToken()
            {
                if (string.IsNullOrWhiteSpace(token))
                    return;

                RegisterToken(GetTokenType(token));
            }

            void RegisterToken(TokenType tokenType)
            {
                // DELETE
                //if (string.IsNullOrWhiteSpace(token))
                //    return;

                tokens.Add(CreateToken(token, tokenType, lineNumber, position));
                token = string.Empty;
            }

            void RegisterSingleCharToken(char c, TokenType tokenType)
            {
                token = c.ToString();
                RegisterToken(tokenType);
            }

            bool tokenInstring = false, escaping = false;

            foreach (char c in expression)
            {
                position++; // TODO should be 0-based.

                if (tokenInstring)
                {
                    if (escaping)
                    {
                        if (c == DOUBLE_QUOTE || c == ESCAPE)
                            AddCharToToken(c);
                        else if (c == 'n')
                            AddCharToToken(NEW_LINE);
                        else if (c == 'r')
                            AddCharToToken(CARRIAGE_RETURN);
                        else if (c == 't')
                            AddCharToToken(TAB);
                        else
                        {
                            AddCharToToken(ESCAPE);
                            AddCharToToken(c);
                        }

                        // Stop escaping, escaping applies to one character only.
                        escaping = false;
                    }
                    else
                    {
                        if (c == ESCAPE)
                        {
                            escaping = true;
                        }
                        // A not escaped double quote ends the string.
                        else if (c == DOUBLE_QUOTE)
                        {
                            tokenInstring = false;
                            RegisterToken(TokenType.String);
                        }
                        else
                            AddCharToToken(c);
                    }
                }
                // Start of a string.
                else if (c == DOUBLE_QUOTE)
                {
                    tokenInstring = true;
                    //AddCharToToken(c);
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
                    RegisterUnknownToken();
                else
                    AddCharToToken(c);
            }
            RegisterUnknownToken();

            tokens.Add(new Token { Type = TokenType.EOF });

            return tokens;
        }

        private static Token CreateToken(string token, TokenType tokenType, uint lineNumber, uint position)
            => new()
            {
                Value = token,
                Type = tokenType,
                LineNumber = lineNumber,
                Position = position
            };

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
