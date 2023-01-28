using RainLisp.Grammar;

namespace RainLisp.Tokenization
{
    public static class Extensions
    {
        public static string ToWord(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.LParen => Delimiters.LPAREN.ToString(),
                TokenType.RParen => Delimiters.RPAREN.ToString(),
                TokenType.Quote => Keywords.QUOTE,
                TokenType.Assignment => Keywords.SET,
                TokenType.Definition => Keywords.DEFINE,
                TokenType.If => Keywords.IF,
                TokenType.Cond => Keywords.COND,
                TokenType.Else => Keywords.ELSE,
                TokenType.Begin => Keywords.BEGIN,
                TokenType.Lambda => Keywords.LAMBDA,
                TokenType.Let => Keywords.LET,
                TokenType.And => Keywords.AND,
                TokenType.Or => Keywords.OR,
                _ => tokenType.ToString(),
            };
        }

        public static TokenType ToTokenType(this string word)
        {
            return word switch
            {
                Keywords.QUOTE => TokenType.Quote,
                Keywords.SET => TokenType.Assignment,
                Keywords.DEFINE => TokenType.Definition,
                Keywords.IF => TokenType.If,
                Keywords.COND => TokenType.Cond,
                Keywords.ELSE => TokenType.Else,
                Keywords.BEGIN => TokenType.Begin,
                Keywords.LAMBDA => TokenType.Lambda,
                Keywords.LET => TokenType.Let,
                Keywords.AND => TokenType.And,
                Keywords.OR => TokenType.Or,
                _ => TokenType.Identifier
            };
        }
    }
}
