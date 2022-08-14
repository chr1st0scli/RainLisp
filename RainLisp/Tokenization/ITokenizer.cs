namespace RainLisp.Tokenization
{
    public interface ITokenizer
    {
        List<Token> Tokenize(string expression);
    }
}
