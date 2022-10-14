using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class Or
    {
        public Or(IList<Expression> expressions)
            => Expressions = expressions;

        public IList<Expression> Expressions { get; init; }
    }
}
