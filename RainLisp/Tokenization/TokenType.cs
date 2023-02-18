namespace RainLisp.Tokenization
{
    public enum TokenType
    {
        Number,
        String,
        Boolean,
        Identifier,
        LParen,
        RParen,
        Quote,
        QuoteAlt,
        Assignment,
        Definition,
        If,
        Cond,
        Else,
        Begin,
        Lambda,
        Let,
        And,
        Or,
        EOF
    }
}
