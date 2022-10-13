using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// A derived expression is one that is derived from another one and the two are functionally equivalent.
    /// This is a technique to avoid implementing the evaluation of a new kind of expression when it can be transformed
    /// to another one whose evaluation is already feasible.
    /// </summary>
    public static class Transformations
    {
        public static If ToIf(this Condition condition)
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

        public static Application ToLambdaApplication(this Let let)
        {
            // Transform a let expression to an application of a lambda.
            ArgumentNullException.ThrowIfNull(let, nameof(let));

            var parameters = new List<string>();
            var operands = new List<Expression>();

            foreach (var letClause in let.Clauses)
            {
                parameters.Add(letClause.IdentifierName);
                operands.Add(letClause.Expression);
            }

            var lambda = new Lambda(parameters, let.Body);

            return new Application(lambda, operands);
        }

        public static Expression ToIf(this And and)
        {
            // And is turned into an if expression in a way that each and's operand will be evaluated until an operand evaluates to false
            // or the last operand is reached, in which case it will be the result of the evaluation.
            if (and.Expressions.Count == 1)
                return and.Expressions[0];

            // In and, a nested if goes inside the consequent of the outer if.
            return AndOrToIf(and.Expressions, (expr, nestedExpr) => new If(expr, nestedExpr, new BooleanLiteral(false)));
        }

        public static Expression ToIf(this Or or)
        {
            // Or is turned into an if expression in a way that each or's operand will be evaluated until an operand evaluates to true
            // or the last operand is reached, in which case it will be the result of the evaluation.
            if (or.Expressions.Count == 1)
                return or.Expressions[0];

            // In or, a nested if goes inside the alternative of the outer if.
            return AndOrToIf(or.Expressions, (expr, nestedExpr) => new If(expr, new BooleanLiteral(true), nestedExpr));
        }

        private static If AndOrToIf(IList<Expression> expressions, Func<Expression, Expression, If> createNestedIf, int expressionIndex = 0)
        {
            var expression = expressions[expressionIndex];

            Expression nestedExpression;
            // If expression is next to last, it does not create another nested if but uses the the last expression instead.
            if (expressionIndex == expressions.Count - 2)
                nestedExpression = expressions[expressionIndex + 1];
            else
                nestedExpression = AndOrToIf(expressions, createNestedIf, ++expressionIndex);

            return createNestedIf(expression, nestedExpression);
        }
    }
}
