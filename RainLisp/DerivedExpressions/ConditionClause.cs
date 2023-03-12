using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Condition clause, that is part of a condition, as described in the syntax grammar.
    /// </summary>
    public class ConditionClause
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionClause"/> class.
        /// </summary>
        /// <param name="predicate">The expression whose value determines if <paramref name="expressions"/> are to be evaluated.</param>
        /// <param name="expressions">A list of expressions that are to be evaluated if <paramref name="predicate"/> is true.</param>
        public ConditionClause(Expression predicate, IList<Expression> expressions)
        {
            Predicate = predicate;
            Expressions = expressions;
        }

        /// <summary>
        /// Gets or sets the expression whose value determines if <see cref="Expressions"/> are to be evaluated.
        /// </summary>
        public Expression Predicate { get; init; }

        /// <summary>
        /// Gets or sets the list of expressions that are to be evaluated if <see cref="Predicate"/> is true.
        /// </summary>
        public IList<Expression> Expressions { get; init; }
    }
}
