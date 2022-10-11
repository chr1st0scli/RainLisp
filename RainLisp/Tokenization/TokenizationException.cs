namespace RainLisp.Tokenization
{
    public class TokenizationException : Exception
    {
        public TokenizationException(uint line, uint position)
        {
            Line = line;
            Position = position;
        }

        public uint Line { get; init; }

        public uint Position { get; init; }
    }
}
