namespace RainLisp.Tokenization
{
    public class NonTerminatedStringException : Exception
    {
        public NonTerminatedStringException()
        {
        }

        public NonTerminatedStringException(string? message) : base(message)
        {
        }

        public NonTerminatedStringException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
