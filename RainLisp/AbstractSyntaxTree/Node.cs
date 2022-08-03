namespace RainLisp.AbstractSyntaxTree
{
    public abstract class Node
    {
        public string TypeName => GetType().Name;

        public abstract object AcceptVisitor(IVisitor visitor, Environment environment);
    }
}
