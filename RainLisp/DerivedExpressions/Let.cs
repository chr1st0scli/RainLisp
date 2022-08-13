using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    public class Let
    {
        public Let(IList<LetClause> clauses, Body body)
        {
            Clauses = clauses ?? throw new ArgumentNullException(nameof(clauses));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public IList<LetClause> Clauses { get; init; }

        public Body Body { get; init; }
    }
}
