using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Node of the abstract syntax tree.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Gets the name of the node's runtime type.
        /// </summary>
        public string TypeName => GetType().Name;

        /// <summary>
        /// Evaluates the current abstract syntax tree node and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public abstract EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns a string that represents the current node.
        /// </summary>
        /// <returns>A string that represents the current node.</returns>
        public override string? ToString() => TypeName;
    }
}
