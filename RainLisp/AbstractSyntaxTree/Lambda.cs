using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Lambda, i.e. an anonymous function, in the abstract syntax tree.
    /// </summary>
    public class Lambda : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lambda"/> class.
        /// </summary>
        /// <param name="parameters">An optional list of parameters for the function.</param>
        /// <param name="body">The body of the function.</param>
        public Lambda(IList<string>? parameters, Body body)
        {
            Parameters = parameters;
            Body = body;
        }

        /// <summary>
        /// Gets or sets the function's optional parameters.
        /// </summary>
        public IList<string>? Parameters { get; init; }

        /// <summary>
        /// Gets or sets the function's body.
        /// </summary>
        public Body Body { get; init; }

        /// <summary>
        /// Evaluates the lambda expression and returns the result.
        /// </summary>
        /// <param name="visitor">The visitor that implements the evaluation.</param>
        /// <param name="environment">The environment the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateLambda(this, environment);
    }
}
