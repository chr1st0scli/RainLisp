using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Delayed expression in the abstract syntax tree.
    /// </summary>
    /// <param name="delayed">The expression whose evaluation is meant to be delayed.</param>
    public class Delay(Expression delayed) : Expression
    {
        /// <summary>
        /// Gets ot sets the expression whose evaluation is meant to be delayed.
        /// </summary>
        public Expression Delayed { get; init; } = delayed;

        /// <summary>
        /// Evaluates the delay expression and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateDelay(this, environment);
    }
}
