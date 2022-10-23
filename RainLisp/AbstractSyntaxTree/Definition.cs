using RainLisp.Environment;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class Definition : Node
    {
        public Definition(string identifierName, Expression value)
        {
            IdentifierName = identifierName;
            Value = value;
        }

        public string IdentifierName { get; init; }

        public Expression Value { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateDefinition(this, environment);
    }
}
