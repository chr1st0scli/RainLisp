namespace RainLisp.Tokenization
{
    public class NonTerminatedStringException : TokenizationException
    {
        public NonTerminatedStringException(uint line, uint position) : base(line, position)
        {
        }
    }
}
