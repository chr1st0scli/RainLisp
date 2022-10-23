using RainLisp.Environment;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class If : Expression
    {
        public If(Expression predicate, Expression consequent, Expression? alternative = null)
        {
            Predicate = predicate;
            Consequent = consequent;
            Alternative = alternative;
        }

        public Expression Predicate { get; init; }

        public Expression Consequent { get; init; }

        public Expression? Alternative { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateIf(this, environment);
    }
}
