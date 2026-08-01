namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Condition expression as described in the syntax grammar.
    /// </summary>
    /// <param name="clauses">A list of conditional clauses contained in the condition.</param>
    /// <param name="elseClause">An optional alternative clause.</param>
    public class Condition(IList<ConditionClause> clauses, ConditionElseClause? elseClause)
    {
        /// <summary>
        /// Gets or sets the list of conditional clauses contained in the condition.
        /// </summary>
        public IList<ConditionClause> Clauses { get; init; } = clauses;

        /// <summary>
        /// Gets or sets the optional alternative clause.
        /// </summary>
        public ConditionElseClause? ElseClause { get; init; } = elseClause;
    }
}
