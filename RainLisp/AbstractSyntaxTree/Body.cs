using RainLisp.Environment;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class Body : Node
    {
        public Body(IList<Definition>? definitions, IList<Expression> expressions)
        {
            Definitions = definitions;
            Expressions = expressions;
        }

        public IList<Definition>? Definitions { get; init; }

        public IList<Expression> Expressions { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBody(this, environment);
    }
}
