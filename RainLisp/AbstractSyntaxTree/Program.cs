using RainLisp.Evaluator;

namespace RainLisp.AbstractSyntaxTree
{
    public class Program : Node
    {
        public IList<Definition> Definitions { get; set; } = new List<Definition>();

        public IList<Expression> Expressions { get; set; } = new List<Expression>();

        public override object AcceptVisitor(IEvaluatorVisitor visitor, Environment environment)
            => visitor.VisitProgram(this);
    }
}
