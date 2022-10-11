namespace RainLisp.Tokenization
{
    public class InvalidEscapeSequenceException : StringCharacterException
    {
        public InvalidEscapeSequenceException(uint line, uint position, char character) : base(line, position, character)
        {
        }
    }
}
