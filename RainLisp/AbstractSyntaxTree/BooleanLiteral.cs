using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class BooleanLiteral : Expression
    {
        public BooleanLiteral(bool value) => Value = value;

        public bool Value { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, EvaluationEnvironment environment)
            => visitor.EvaluateBooleanLiteral(this);
    }
}
