using RainLisp.Environment;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class NumberLiteral : Expression
    {
        public NumberLiteral(double value) => Value = value;

        public double Value { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateNumberLiteral(this);
    }
}
