using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class ConditionClause
    {
        public ConditionClause(Expression predicate, IList<Expression> expressions)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            Expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
        }

        public Expression Predicate { get; init; }

        public IList<Expression> Expressions { get; init; }
    }
}
