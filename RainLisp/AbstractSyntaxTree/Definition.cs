using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Definition in the abstract syntax tree.
    /// </summary>
    public class Definition : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Definition"/> class.
        /// </summary>
        /// <param name="identifierName">The identifier name that represents the definition.</param>
        /// <param name="value">The expression whose value is to be bound to <paramref name="identifierName"/>.</param>
        public Definition(string identifierName, Expression value)
        {
            IdentifierName = identifierName;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the identifier name that represents the definition.
        /// </summary>
        public string IdentifierName { get; init; }

        /// <summary>
        /// Gets or sets the expression whose value is to be bound to <see cref="IdentifierName"/>.
        /// </summary>
        public Expression Value { get; init; }

        /// <summary>
        /// Evaluates the current definition and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of this instance.</exception>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateDefinition(this, environment);

        /// <summary>
        /// Returns a string that represents the current definition.
        /// </summary>
        /// <returns>A string that represents the current definition.</returns>
        public override string? ToString() => $"{TypeName} {IdentifierName}";
    }
}
