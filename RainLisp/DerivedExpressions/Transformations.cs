using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// A derived expression is one that is derived from another one and the two are functionally equivalent.
    /// This is a technique to avoid implementing the evaluation of a new kind of expression when it can be transformed
    /// to another one whose evaluation is already implemented.
    /// </summary>
    public static class Transformations
    {
        public static If ConditionToIf(Condition condition)
        {
            ArgumentNullException.ThrowIfNull(condition, nameof(condition));

            Expression ToSingleExpression(IList<Expression> expressions)
                => expressions.Count == 1 ? expressions[0] : new Begin(expressions);

            If MakeIf(int clauseIndex = 0)
            {
                var clause = condition.Clauses[clauseIndex];

                // Traditionally, in a Lisp cond expression, each conditional clause can contain many expressions (actions).
                // So, in order to tranform it to a valid if, a single expression becomes the consequent as is, whereas many
                // expressions are included in a begin expression to be treated as one.
                var consequent = ToSingleExpression(clause.Expressions);

                Expression? alternative = null;
                // If we are dealing with the last clause (other than the else),
                // the else clause, if any, becomes the alternative of the if that is currently being built.
                if (clauseIndex == condition.Clauses.Count - 1)
                {
                    if (condition.ElseClause != null)
                        alternative = ToSingleExpression(condition.ElseClause.Expressions);
                }
                // If there are more clauses, the alternative is a new nested if.
                else
                    alternative = MakeIf(++clauseIndex);

                return new If(clause.Predicate, consequent, alternative);
            }

            return MakeIf();
        }
    }
}
