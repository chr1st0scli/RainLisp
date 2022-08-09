using RainLisp.Evaluator;

namespace RainLisp.AbstractSyntaxTree
{
    public class If : Expression
    {
        public If(Expression predicate, Expression consequent, Expression? alternative = null)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            Consequent = consequent ?? throw new ArgumentNullException(nameof(consequent));
            Alternative = alternative;
        }

        public Expression Predicate { get; init; }

        public Expression Consequent { get; init; }

        public Expression? Alternative { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, Environment environment)
            => visitor.EvaluateIf(this, environment);
    }
}
