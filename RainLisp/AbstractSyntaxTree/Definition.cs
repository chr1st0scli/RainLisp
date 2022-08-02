namespace RainLisp.AbstractSyntaxTree
{
    public class Definition : Node
    {
        public Definition(string identifierName, Expression value)
        {
            IdentifierName = identifierName ?? throw new ArgumentNullException(nameof(identifierName));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string IdentifierName { get; init; }

        public Expression Value { get; init; }

        public override void AcceptVisitor(IVisitor visitor)
            => visitor.VisitDefinition(this);
    }
}
