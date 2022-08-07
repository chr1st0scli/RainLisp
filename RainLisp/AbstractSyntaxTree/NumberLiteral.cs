using RainLisp.Evaluator;

namespace RainLisp.AbstractSyntaxTree
{
    public class NumberLiteral : Expression
    {
        public NumberLiteral(double value) => Value = value;

        public double Value { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, Environment environment)
            => visitor.VisitNumberLiteral(this);
    }
}
