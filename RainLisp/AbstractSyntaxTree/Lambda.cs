using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Lambda : Expression
    {
        public Lambda(IList<string>? parameters, Body body)
        {
            Parameters = parameters;
            Body = body;
        }

        public IList<string>? Parameters { get; init; }

        public Body Body { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateLambda(this, environment);
    }
}
