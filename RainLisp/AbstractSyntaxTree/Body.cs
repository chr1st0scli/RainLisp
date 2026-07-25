using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Body of a function in the abstract syntax tree.
    /// </summary>
    /// <param name="definitions">An optional list of definitions.</param>
    /// <param name="expressions">The list of expressions included in the function's body.</param>
    public class Body(IList<Definition>? definitions, IList<Expression> expressions) : Node
    {
        /// <summary>
        /// Gets or sets the optional list of definitions.
        /// </summary>
        public IList<Definition>? Definitions { get; init; } = definitions;

        /// <summary>
        /// Gets or sets the list of expressions included in the function's body.
        /// </summary>
        public IList<Expression> Expressions { get; init; } = expressions;

        /// <summary>
        /// Evaluates the function's body and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of this instance.</exception>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBody(this, environment);
    }
}
