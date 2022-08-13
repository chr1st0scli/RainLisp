using static RainLisp.Grammar.SpecialSymbols;
using static RainLisp.Grammar.Keywords;

namespace RainLisp
{
    public static class Tokenizer
    {
        public static List<string>? Tokenize(string expression) => null;

        public static List<Token> TokenizeExt(string expression)
        {
            ArgumentNullException.ThrowIfNull(expression, nameof(expression));

            expression = expression.Trim();
            var tokens = new List<Token>();
            string token = string.Empty;

            void AddCharToToken(char c) => token += c;

            void RegisterToken()
            {
                if (string.IsNullOrWhiteSpace(token))
                    return;

                tokens.Add(CreateToken(token));
                token = string.Empty;
            }

            bool stringToken = false, escapeChar = false;

            foreach (char c in expression)
            {
                if (!stringToken && c == DOUBLE_QUOTE)
                {
                    stringToken = true;
                    AddCharToToken(c);
                }
                else if (stringToken)
                {
                    AddCharToToken(c);
                    
                    if (!escapeChar)
                    {
                        if (c == ESCAPE)
                            escapeChar = true;
                        else if (c == DOUBLE_QUOTE)
                        {
                            stringToken = false;
                            RegisterToken();
                        }
                    }
                    else
                        escapeChar = false;
                }
                else if (c == LPAREN || c == RPAREN)
                {
                    RegisterToken();
                    token = c.ToString();
                    RegisterToken();
                }
                else if (c == SPACE || c == CARRIAGE_RETURN || c == NEW_LINE || c == TAB)
                    RegisterToken();
                else
                    AddCharToToken(c);
            }
            RegisterToken();

            tokens.Add(new Token { Type = TokenType.EOF });

            return tokens;
        }

        private static Token CreateToken(string token)
            => new()
            {
                Value = token,
                Type = GetTokenType(token)
            };

        private static TokenType GetTokenType(string token)
        {
            ArgumentNullException.ThrowIfNull(token, nameof(token));

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Invalid token.", nameof(token));

            TokenType GetOtherType()
            {
                if (token == LPAREN.ToString())
                    return TokenType.LParen;
                
                else if (token == RPAREN.ToString())
                    return TokenType.RParen;

                else if (token.StartsWith("\"") && token.EndsWith("\""))
                    return TokenType.String;

                else if (double.TryParse(token, out double _))
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
