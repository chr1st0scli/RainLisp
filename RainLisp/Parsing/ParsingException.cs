using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    public class ParsingException : Exception
    {
        public ParsingException(uint line, uint position, TokenType[] missingSymbols)
        {
            Line = line;
            Position = position;
            MissingSymbols = missingSymbols;
        }

        public ParsingException(uint line, uint position, TokenType[] missingSymbols, string? message) : base(message)
        {
            Line = line;
            Position = position;
            MissingSymbols = missingSymbols;
        }

        public ParsingException(uint line, uint position, TokenType[] missingSymbols, string? message, Exception? innerException) : base(message, innerException)
        {
            Line = line;
            Position = position;
            MissingSymbols = missingSymbols;
        }

        public uint Line { get; init; }

        public uint Position { get; init; }

        public TokenType[] MissingSymbols { get; init; }
    }
}
