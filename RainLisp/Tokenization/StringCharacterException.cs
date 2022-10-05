namespace RainLisp.Tokenization
{
    public class StringCharacterException : Exception
    {
        public StringCharacterException(char character)
        {
            Character = character;
        }

        public StringCharacterException(char character, string? message) : base(message)
        {
            Character = character;
        }

        public StringCharacterException(char character, string? message, Exception? innerException) : base(message, innerException)
        {
            Character = character;
        }

        public char Character { get; init; }
    }
}
