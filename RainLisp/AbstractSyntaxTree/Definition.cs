using RainLisp.Evaluator;

namespace RainLisp.AbstractSyntaxTree
{
    public class Definition : Node
    {
        public Definition(string identifierName, Expression value)
        {
            IdentifierName = identifierName ?? throw new ArgumentNullException(nameof(identifierName));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string IdentifierName { get; init; }

        public Expression Value { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, Environment environment)
            => visitor.EvaluateDefinition(this, environment);
    }
}
