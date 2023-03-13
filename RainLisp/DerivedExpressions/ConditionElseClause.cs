using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Condition else clause, that is part of a condition, as described in the syntax grammar.
    /// </summary>
    public class ConditionElseClause
    {
        /// <summary>
        /// Initializes a new instannce of the <see cref="ConditionElseClause"/> class.
        /// </summary>
        /// <param name="expressions">A list of expressions that are to be evaluated if all other condition clauses were not chosen.</param>
        public ConditionElseClause(IList<Expression> expressions)
            => Expressions = expressions;

        /// <summary>
        /// Gets or sets the list of expressions that are to be evaluated if all other condition clauses were not chosen.
        /// </summary>
        public IList<Expression> Expressions { get; init; }
    }
}
