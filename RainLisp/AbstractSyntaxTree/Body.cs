namespace RainLisp.AbstractSyntaxTree
{
    public class Body : Node
    {
        public Body(IList<Definition>? definitions, Expression expression)
        {
            Definitions = definitions;
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public IList<Definition>? Definitions { get; init; }

        public Expression Expression { get; init; }

        public override object AcceptVisitor(IVisitor visitor, Environment environment)
            => visitor.VisitBody(this, environment);
    }
}
