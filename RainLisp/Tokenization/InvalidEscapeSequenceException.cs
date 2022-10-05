namespace RainLisp.Tokenization
{
    public class InvalidEscapeSequenceException : StringCharacterException
    {
        public InvalidEscapeSequenceException(char character) : base(character)
        {
        }

        public InvalidEscapeSequenceException(char character, string? message) : base(character, message)
        {
        }

        public InvalidEscapeSequenceException(char character, string? message, Exception? innerException) : base(character, message, innerException)
        {
        }
    }
}
