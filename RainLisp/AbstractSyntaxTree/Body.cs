using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Body : Node
    {
        public Body(IList<Definition>? definitions, IList<Expression> expressions)
        {
            Definitions = definitions;
            Expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
        }

        public IList<Definition>? Definitions { get; init; }

        public IList<Expression> Expressions { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBody(this, environment);
    }
}
