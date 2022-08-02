namespace RainLisp.AbstractSyntaxTree
{
    public abstract class Node
    {
        public string TypeName => GetType().Name;

        public object? EvaluationResult { get; set; }

        public abstract void AcceptVisitor(IVisitor visitor);
    }
}
