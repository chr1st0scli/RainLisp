using RainLisp.Evaluator;

namespace RainLisp.AbstractSyntaxTree
{
    public class Begin : Expression
    {
        public Begin(IList<Expression> expressions)
        {
            Expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
        }

        public IList<Expression> Expressions { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, Environment environment)
            => visitor.EvaluateBegin(this, environment);
    }
}
