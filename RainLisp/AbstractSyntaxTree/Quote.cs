using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Quote expression in the abstract syntax tree.
    /// </summary>
    /// <param name="quotable">The quotable contained in the quote epression.</param>
    public class Quote(Quotable quotable) : Expression
    {
        /// <summary>
        /// Gets or sets the quotable contained in the quote expression.
        /// </summary>
        public Quotable Quotable { get; init; } = quotable;

        /// <summary>
        /// Evaluates the quote expression and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateQuote(this, environment);
    }
}
