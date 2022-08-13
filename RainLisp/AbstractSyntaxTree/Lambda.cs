using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Lambda : Expression
    {
        public Lambda(IList<string>? parameters, Body body)
        {
            Parameters = parameters;
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IList<string>? Parameters { get; init; }

        public Body Body { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, EvaluationEnvironment environment)
            => visitor.EvaluateLambda(this, environment);
    }
}
