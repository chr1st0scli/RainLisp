using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Body of a function in the abstract syntax tree.
    /// </summary>
    public class Body : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body"/> class.
        /// </summary>
        /// <param name="definitions">An optional list of definitions.</param>
        /// <param name="expressions">The list of expressions included in the function's body.</param>
        public Body(IList<Definition>? definitions, IList<Expression> expressions)
        {
            Definitions = definitions;
            Expressions = expressions;
        }

        /// <summary>
        /// Gets or sets the optional list of definitions.
        /// </summary>
        public IList<Definition>? Definitions { get; init; }

        /// <summary>
        /// Gets or sets the list of expressions included in the function's body.
        /// </summary>
        public IList<Expression> Expressions { get; init; }

        /// <summary>
        /// Evaluates the function's body and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBody(this, environment);
    }
}
