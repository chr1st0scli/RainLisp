namespace RainLisp.Tokenization
{
    public class InvalidStringCharacterException : StringCharacterException
    {
        public InvalidStringCharacterException(char character) : base(character)
        {
        }

        public InvalidStringCharacterException(char character, string? message) : base(character, message)
        {
        }

        public InvalidStringCharacterException(char character, string? message, Exception? innerException) : base(character, message, innerException)
        {
        }
    }
}
