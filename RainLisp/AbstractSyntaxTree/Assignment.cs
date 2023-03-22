using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Assignment of a value to an identifier in the abstract syntax tree.
    /// </summary>
    public class Assignment : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Assignment"/> class.
        /// </summary>
        /// <param name="identifierName">The identifier name to be bound to the new value.</param>
        /// <param name="value">The expression whose value is to be bound to <paramref name="identifierName"/>.</param>
        public Assignment(string identifierName, Expression value)
        {
            IdentifierName = identifierName;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the identifier name to be bound to the new value.
        /// </summary>
        public string IdentifierName { get; init; }

        /// <summary>
        /// Gets or sets the expression whose value is to be bound to <see cref="IdentifierName"/>.
        /// </summary>
        public Expression Value { get; init; }

        /// <summary>
        /// Evaluates the assignment and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="UnknownIdentifierException">The <see cref="IdentifierName"/> is not defined.</exception>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of this instance.</exception>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateAssignment(this, environment);

        /// <summary>
        /// Returns a string that represents the current assignment.
        /// </summary>
        /// <returns>A string that represents the current assignment.</returns>
        public override string? ToString() => $"{TypeName} {IdentifierName}";
    }
}
