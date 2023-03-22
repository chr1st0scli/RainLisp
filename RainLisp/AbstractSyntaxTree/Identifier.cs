using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Identifier in the abstract syntax tree.
    /// </summary>
    public class Identifier : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier"/> class.
        /// </summary>
        /// <param name="name">The name of the identifier.</param>
        public Identifier(string name)
            => Name = name;

        /// <summary>
        /// Gets or sets the name of the identifier.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Evaluates the identifier and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="UnknownIdentifierException">This identifier's <see cref="Name"/> is not defined.</exception>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateIdentifier(this, environment);

        /// <summary>
        /// Returns a string that represents the current identifier.
        /// </summary>
        /// <returns>A string that represents the current identifier.</returns>
        public override string? ToString() => Name;
    }
}
