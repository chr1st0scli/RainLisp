using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Let clause, that is part of a let expression, as described in the syntax grammar.
    /// </summary>
    public class LetClause
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LetClause"/> class.
        /// </summary>
        /// <param name="identifierName">An identifier name that is to be bound to <paramref name="expression"/>'s value with local scope for the let expression's body.</param>
        /// <param name="expression">The expression whose value is to be bound to <paramref name="identifierName"/>.</param>
        public LetClause(string identifierName, Expression expression)
        {
            IdentifierName = identifierName;
            Expression = expression;
        }

        /// <summary>
        /// Gets or sets the identifier name that is to be bound to <see cref="Expression"/>'s value with local scope for the let expression's body.
        /// </summary>
        public string IdentifierName { get; init; }

        /// <summary>
        /// Gets or sets the expression whose value is to be bound to <see cref="IdentifierName"/>.
        /// </summary>
        public Expression Expression { get; init; }
    }
}
