using RainLisp.Grammar;

namespace RainLisp.Tokenization
{
    /// <summary>
    /// Extension methods for tokens.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the string representation of a token type in a human friendly format.
        /// </summary>
        /// <param name="tokenType">The token type to get the string representation from.</param>
        /// <returns>The string representation of a token type in a human friendly format.</returns>
        public static string ToWord(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.LParen => Delimiters.LPAREN.ToString(),
                TokenType.RParen => Delimiters.RPAREN.ToString(),
                TokenType.QuoteAlt => Delimiters.SINGLE_QUOTE.ToString(),
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
                TokenType.Delay => Keywords.DELAY,
                TokenType.ConsStream => Keywords.CONS_STREAM,
                _ => tokenType.ToString(),
            };
        }

        /// <summary>
        /// Returns the token type that corresponds to a word.
        /// If the word is a keyword, the corresponding token type is returned.
        /// Otherwise, it is assumed to be an identifier.
        /// </summary>
        /// <param name="word">The word to infer the token type from.</param>
        /// <returns>The token type that corresponds to <paramref name="word"/>.</returns>
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
                Keywords.DELAY => TokenType.Delay,
                Keywords.CONS_STREAM => TokenType.ConsStream,
                _ => TokenType.Identifier
            };
        }
    }
}
