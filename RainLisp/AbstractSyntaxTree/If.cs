using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// If expression in the abstract syntax tree.
    /// </summary>
    public class If : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="If"/> class.
        /// </summary>
        /// <param name="predicate">The expression whose value determines whether the <paramref name="consequent"/> or <paramref name="alternative"/> is to be executed.</param>
        /// <param name="consequent">The expression to be evaluated if <paramref name="predicate"/>'s value is true.</param>
        /// <param name="alternative">The optional expression to be evaluated if <paramref name="predicate"/>'s value is false.</param>
        public If(Expression predicate, Expression consequent, Expression? alternative = null)
        {
            Predicate = predicate;
            Consequent = consequent;
            Alternative = alternative;
        }

        /// <summary>
        /// Gets or sets the expression whose value determines whether the <see cref="Consequent"/> or <see cref="Alternative"/> is to be executed.
        /// </summary>
        public Expression Predicate { get; init; }

        /// <summary>
        /// Gets or sets the expression to be evaluated if <see cref="Predicate"/>'s value is true.
        /// </summary>
        public Expression Consequent { get; init; }

        /// <summary>
        /// Gets or sets the optional expression to be evaluated if <see cref="Predicate"/>'s value is false.
        /// </summary>
        public Expression? Alternative { get; init; }

        /// <summary>
        /// Evaluates the if expression and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateIf(this, environment);
    }
}
