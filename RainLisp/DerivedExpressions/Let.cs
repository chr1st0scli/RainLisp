using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class Let
    {
        public Let(IList<LetClause> clauses, Body body)
        {
            Clauses = clauses;
            Body = body;
        }

        public IList<LetClause> Clauses { get; init; }

        public Body Body { get; init; }
    }
}
