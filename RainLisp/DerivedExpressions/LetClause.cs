using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class LetClause
    {
        public LetClause(string identifierName, Expression expression)
        {
            IdentifierName = identifierName;
            Expression = expression;
        }

        public string IdentifierName { get; init; }

        public Expression Expression { get; init; }
    }
}
