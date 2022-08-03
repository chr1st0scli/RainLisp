namespace RainLisp.AbstractSyntaxTree
{
    public class Assignment : Expression
    {
        public Assignment(string identifierName, Expression value)
        {
            IdentifierName = identifierName ?? throw new ArgumentNullException(nameof(identifierName));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string IdentifierName { get; init; }

        public Expression Value { get; init; }

        public override object AcceptVisitor(IVisitor visitor, Environment environment)
            => visitor.VisitAssignment(this, environment);
    }
}
