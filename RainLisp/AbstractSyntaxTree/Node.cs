namespace RainLisp.AbstractSyntaxTree
{
    public class Node
    {
        //public string ExpressionText { get; set; }
        public string TypeName => GetType().Name;
    }
}
