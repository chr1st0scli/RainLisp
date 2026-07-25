using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Condition else clause, that is part of a condition, as described in the syntax grammar.
    /// </summary>
    /// <param name="expressions">A list of expressions that are to be evaluated if all other condition clauses were not chosen.</param>
    public class ConditionElseClause(IList<Expression> expressions)
    {
        /// <summary>
        /// Gets or sets the list of expressions that are to be evaluated if all other condition clauses were not chosen.
        /// </summary>
        public IList<Expression> Expressions { get; init; } = expressions;
    }
}
