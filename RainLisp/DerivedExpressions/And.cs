using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class And
    {
        public And(IList<Expression> expressions)
            => Expressions = expressions;

        public IList<Expression> Expressions { get; init; }
    }
}
