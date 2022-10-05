namespace RainLisp.Tokenization
{
    public class NonTerminatedStringException : Exception
    {
        public NonTerminatedStringException(uint line, uint position)
        {
            Line = line;
            Position = position;
        }

        public NonTerminatedStringException(uint line, uint position, string? message) : base(message)
        {
            Line = line;
            Position = position;
        }

        public NonTerminatedStringException(uint line, uint position, string? message, Exception? innerException) : base(message, innerException)
        {
            Line = line;
            Position = position;
        }

        public uint Line { get; init; }

        public uint Position { get; init; }
    }
}
