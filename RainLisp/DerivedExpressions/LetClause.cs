using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class LetClause
    {
        public LetClause(string identifierName, Expression expression)
        {
            IdentifierName = identifierName ?? throw new ArgumentNullException(nameof(identifierName));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public string IdentifierName { get; init; }

        public Expression Expression { get; init; }
    }
}
