namespace RainLisp.Tokenization
{
    /// <summary>
    /// Specifies the type of a <see cref="Token"/> that can be produced by the lexical analysis.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// The token is a number literal.
        /// </summary>
        Number,
        /// <summary>
        /// The token is a string literal.
        /// </summary>
        String,
        /// <summary>
        /// The token is a boolean literal.
        /// </summary>
        Boolean,
        /// <summary>
        /// The token is an identifier.
        /// </summary>
        Identifier,
        /// <summary>
        /// The token is an opening parenthesis.
        /// </summary>
        LParen,
        /// <summary>
        /// The token is a closing parenthesis.
        /// </summary>
        RParen,
        /// <summary>
        /// The token is a quote keyword.
        /// </summary>
        Quote,
        /// <summary>
        /// The token is the quote alternative keyword.
        /// </summary>
        QuoteAlt,
        /// <summary>
        /// The token is the assignment keyword.
        /// </summary>
        Assignment,
        /// <summary>
        /// The token is the definition keyword.
        /// </summary>
        Definition,
        /// <summary>
        /// The token is the if keyword.
        /// </summary>
        If,
        /// <summary>
        /// The token is the cond keyword.
        /// </summary>
        Cond,
        /// <summary>
        /// The token is the else keyword.
        /// </summary>
        Else,
        /// <summary>
        /// The token is the begin keyword.
        /// </summary>
        Begin,
        /// <summary>
        /// The token is the lambda keyword.
        /// </summary>
        Lambda,
        /// <summary>
        /// The token is the let keyword.
        /// </summary>
        Let,
        /// <summary>
        /// The token is the and keyword.
        /// </summary>
        And,
        /// <summary>
        /// The token is the or keyword.
        /// </summary>
        Or,
        /// <summary>
        /// The token is the delay keyword.
        /// </summary>
        Delay,
        /// <summary>
        /// The token is the cons-stream keyword.
        /// </summary>
        ConsStream,
        /// <summary>
        /// The token represents the end of file.
        /// </summary>
        EOF
    }
}
