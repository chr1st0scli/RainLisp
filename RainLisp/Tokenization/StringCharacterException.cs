namespace RainLisp.Tokenization
{
    public class StringCharacterException : TokenizationException
    {
        public StringCharacterException(uint line, uint position, char character) : base(line, position)
        {
            Character = character;
        }

        public char Character { get; init; }
    }
}
