using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Identifier : Expression
    {
        public Identifier(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; init; }

        public override object AcceptVisitor(IEvaluatorVisitor visitor, EvaluationEnvironment environment)
            => visitor.EvaluateIdentifier(this, environment);
    }
}
