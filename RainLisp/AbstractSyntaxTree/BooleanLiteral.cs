namespace RainLisp.AbstractSyntaxTree
{
    public class BooleanLiteral : Expression
    {
        public BooleanLiteral(bool value) => Value = value;

        public bool Value { get; init; }

        public override void AcceptVisitor(IVisitor visitor)
            => visitor.VisitBooleanLiteral(this);
    }
}
