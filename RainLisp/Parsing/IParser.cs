using RainLisp.AbstractSyntaxTree;
using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    public interface IParser
    {
        Program Parse(IList<Token> tokens);
    }
}
