using RainLisp.Evaluator;

namespace RainLisp.AbstractSyntaxTree
{
    public abstract class Node
    {
        public string TypeName => GetType().Name;

        public abstract object AcceptVisitor(IEvaluatorVisitor visitor, Environment environment);
    }
}
