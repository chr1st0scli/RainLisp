using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class Program : Node
    {
        public IList<Definition>? Definitions { get; set; }

        public IList<Expression>? Expressions { get; set; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
        {
            EvaluationResult? lastResult = null;
            foreach (var result in visitor.EvaluateProgram(this, environment))
                lastResult = result;

            // Practically, this is never null.
            return lastResult!;
        }
    }
}
