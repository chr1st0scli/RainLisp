using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Number literal expression in the abstract syntax tree.
    /// </summary>
    /// <param name="value">The contained numeric value.</param>
    public class NumberLiteral(double value) : Expression
    {
        /// <summary>
        /// Gets or sets the contained numeric value of the current literal.
        /// </summary>
        public double Value { get; init; } = value;

        /// <summary>
        /// Evaluates the number literal and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateNumberLiteral(this);
    }
}
