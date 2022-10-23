using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class BooleanLiteral : Expression
    {
        public BooleanLiteral(bool value) => Value = value;

        public bool Value { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBooleanLiteral(this);
    }
}
