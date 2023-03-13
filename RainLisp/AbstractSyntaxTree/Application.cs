using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Function application in the abstract syntax tree.
    /// </summary>
    public class Application : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="operatorToApply">The expression whose value is the function to apply.</param>
        /// <param name="operands">An optional list of expressions whose values are to be passed as arguments to the function.</param>
        public Application(Expression operatorToApply, IList<Expression>? operands)
        {
            Operator = operatorToApply;
            Operands = operands;
        }

        /// <summary>
        /// Gets or sets the expression whose value is the function to apply.
        /// </summary>
        public Expression Operator { get; init; }

        /// <summary>
        /// Gets or sets an optional list of expressions whose values are to be passed as arguments to the function.
        /// </summary>
        public IList<Expression>? Operands { get; init; }

        /// <summary>
        /// Evaluates the function application and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateApplication(this, environment);

        /// <summary>
        /// Returns a string that represents the current application.
        /// </summary>
        /// <returns>A string that represents the current application.</returns>
        public override string? ToString() => $"{TypeName} {Operator}";
    }
}
