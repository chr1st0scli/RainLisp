namespace RainLisp.AbstractSyntaxTree
{
    public class Program : Node
    {
        public IList<Definition> Definitions { get; set; } = new List<Definition>();

        public IList<Node> Expressions { get; set; } = new List<Node>();

        public override void AcceptVisitor(IVisitor visitor)
            => visitor.VisitProgram(this);
    }
}
