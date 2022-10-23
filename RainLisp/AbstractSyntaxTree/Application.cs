using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Application : Expression
    {
        public Application(Expression operatorToApply, IList<Expression>? operands)
        {
            Operator = operatorToApply;
            Operands = operands;
        }

        public Expression Operator { get; init; }

        public IList<Expression>? Operands { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateApplication(this, environment);
    }
}
