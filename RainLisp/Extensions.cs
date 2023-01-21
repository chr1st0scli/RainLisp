using RainLisp.AbstractSyntaxTree;
using RainLisp.Tokenization;

namespace RainLisp
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

        public static void CopyDebugInfoFrom(this IDebugInfo target, IDebugInfo source)
        {
            if (!source.HasDebugInfo)
                return;

            target.Line = source.Line;
            target.Position = source.Position;
            target.HasDebugInfo = true;
        }
    }
}
