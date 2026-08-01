using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Let expression as described in the syntax grammar.
    /// </summary>
    /// <param name="clauses">A list of clauses that setup definitions with local scope for the <paramref name="body"/>.</param>
    /// <param name="body">The body to be evaluated.</param>
    public class Let(IList<LetClause> clauses, Body body)
    {
        /// <summary>
        /// Gets or sets the list of clauses that setup definitions with local scope for the <see cref="Body"/>.
        /// </summary>
        public IList<LetClause> Clauses { get; init; } = clauses;

        /// <summary>
        /// Gets or sets the body to be evaluated.
        /// </summary>
        public Body Body { get; init; } = body;
    }
}
