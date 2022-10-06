using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class StringLiteral : Expression
    {
        public StringLiteral(string value)
            => Value = value;

        public string Value { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateStringLiteral(this);
    }
}
