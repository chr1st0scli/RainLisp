using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class Program : Node
    {
        public IList<Definition> Definitions { get; set; } = new List<Definition>();

        public IList<Expression> Expressions { get; set; } = new List<Expression>();

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateProgram(this, environment);
    }
}
