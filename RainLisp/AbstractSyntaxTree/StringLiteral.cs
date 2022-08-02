namespace RainLisp.AbstractSyntaxTree
{
    public class StringLiteral : Expression
    {
        public StringLiteral(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; init; }

        public override void AcceptVisitor(IVisitor visitor)
            => visitor.VisitStringLiteral(this);
    }
}
