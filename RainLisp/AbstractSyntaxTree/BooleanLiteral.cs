using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Boolean literal expression in the abstract syntax tree.
    /// </summary>
    /// <param name="value">The contained boolean value.</param>
    public class BooleanLiteral(bool value) : Expression
    {
        /// <summary>
        /// Gets or sets the contained boolean value of the current literal.
        /// </summary>
        public bool Value { get; init; } = value;

        /// <summary>
        /// Evaluates the boolean literal and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBooleanLiteral(this);
    }
}
