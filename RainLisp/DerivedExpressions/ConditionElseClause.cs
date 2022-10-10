using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class ConditionElseClause
    {
        public ConditionElseClause(IList<Expression> expressions)
            => Expressions = expressions;

        public IList<Expression> Expressions { get; init; }
    }
}
