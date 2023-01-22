using RainLisp.AbstractSyntaxTree;
using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    public static class Extensions
    {
        public static Expression AddDebugInfo(this Expression expression, Token token)
        {
            expression.Line = token.Line;
            expression.Position = token.Position;
            expression.HasDebugInfo = true;

            return expression;
        }
    }
}
