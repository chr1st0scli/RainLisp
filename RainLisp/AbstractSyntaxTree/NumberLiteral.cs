namespace RainLisp.AbstractSyntaxTree
{
    public class NumberLiteral : Expression
    {
        public NumberLiteral(double value) => Value = value;

        public double Value { get; init; }

        public override object AcceptVisitor(IVisitor visitor, Environment environment)
            => visitor.VisitNumberLiteral(this);
    }
}
