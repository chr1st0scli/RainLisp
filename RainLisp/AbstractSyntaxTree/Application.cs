using RainLisp.Evaluator;

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

        public override object AcceptVisitor(IEvaluatorVisitor visitor, Environment environment)
            => visitor.VisitApplication(this, environment);
    }
}
