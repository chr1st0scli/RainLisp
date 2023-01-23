using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class Assignment : Expression
    {
        public Assignment(string identifierName, Expression value)
        {
            IdentifierName = identifierName;
            Value = value;
        }

        public string IdentifierName { get; init; }

        public Expression Value { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateAssignment(this, environment);

        public override string? ToString() => $"{TypeName} {IdentifierName}";
    }
}
