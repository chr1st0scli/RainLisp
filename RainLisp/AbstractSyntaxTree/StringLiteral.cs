namespace RainLisp.AbstractSyntaxTree
{
    public class StringLiteral : Expression
    {
        public StringLiteral(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; init; }

        public override object AcceptVisitor(IVisitor visitor, Environment environment)
            => visitor.VisitStringLiteral(this);
    }
}
