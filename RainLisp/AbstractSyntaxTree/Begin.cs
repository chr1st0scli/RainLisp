using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Begin expression, i.e. a code block, in the abstract syntax tree.
    /// </summary>
    public class Begin : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Begin"/> class.
        /// </summary>
        /// <param name="expressions">A list of expressions in the code block.</param>
        public Begin(IList<Expression> expressions)
            => Expressions = expressions;

        /// <summary>
        /// Gets or sets the expressions in the code block.
        /// </summary>
        public IList<Expression> Expressions { get; init; }

        /// <summary>
        /// Evaluates the begin code block and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBegin(this, environment);
    }
}
