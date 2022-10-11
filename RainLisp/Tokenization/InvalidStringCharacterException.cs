namespace RainLisp.Tokenization
{
    public class InvalidStringCharacterException : StringCharacterException
    {
        public InvalidStringCharacterException(uint line, uint position, char character) : base(line, position, character)
        {
        }
    }
}
