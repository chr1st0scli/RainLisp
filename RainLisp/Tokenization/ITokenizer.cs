namespace RainLisp.Tokenization
{
    public interface ITokenizer
    {
        IList<Token> Tokenize(string? expression);
    }
}
