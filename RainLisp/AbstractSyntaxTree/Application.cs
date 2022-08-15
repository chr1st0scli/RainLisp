using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Application : Expression
    {
        public Application(Expression @operator, IList<Expression>? operands)
        {
            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            Operands = operands;
        }

        public Expression Operator { get; init; }

        public IList<Expression>? Operands { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateApplication(this, environment);
    }
}
