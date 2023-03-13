namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Condition expression as described in the syntax grammar.
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="clauses">A list of conditional clauses contained in the condition.</param>
        /// <param name="elseClause">An optional alternative clause.</param>
        public Condition(IList<ConditionClause> clauses, ConditionElseClause? elseClause)
        {
            Clauses = clauses;
            ElseClause = elseClause;
        }

        /// <summary>
        /// Gets or sets the list of conditional clauses contained in the condition.
        /// </summary>
        public IList<ConditionClause> Clauses { get; init; }

        /// <summary>
        /// Gets or sets the optional alternative clause.
        /// </summary>
        public ConditionElseClause? ElseClause { get; init; }
    }
}
