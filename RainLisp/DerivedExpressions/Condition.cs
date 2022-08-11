namespace RainLisp.DerivedExpressions
{
    public class Condition
    {
        public Condition(IList<ConditionClause> clauses, ConditionElseClause? elseClause)
        {
            Clauses = clauses ?? throw new ArgumentNullException(nameof(clauses));
            ElseClause = elseClause;
        }

        public IList<ConditionClause> Clauses { get; init; }

        public ConditionElseClause? ElseClause { get; init; }
    }
}
