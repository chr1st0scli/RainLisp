using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public abstract class Node
    {
        public string TypeName => GetType().Name;

        public abstract EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment);
    }
}
