using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// String literal expression in the abstract syntax tree.
    /// </summary>
    public class StringLiteral : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLiteral"/> class.
        /// </summary>
        /// <param name="value">The contained string value.</param>
        public StringLiteral(string value)
            => Value = value;

        /// <summary>
        /// Gets or sets the contained string value of the current literal.
        /// </summary>
        public string Value { get; init; }

        /// <summary>
        /// Evaluates the string literal and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateStringLiteral(this);
    }
}
