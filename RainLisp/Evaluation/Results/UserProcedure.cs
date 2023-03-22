using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a user procedure as a result of an evaluation.
    /// </summary>
    public class UserProcedure : EvaluationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProcedure"/> class.
        /// </summary>
        /// <param name="parameters">An optional list of the procedure's parameter names.</param>
        /// <param name="body">The procedure's body.</param>
        /// <param name="environment">The evaluation environment the procedure is created in.</param>
        public UserProcedure(IList<string>? parameters, Body body, IEvaluationEnvironment environment)
        {
            Parameters = parameters;
            Body = body;
            Environment = environment;
        }

        /// <summary>
        /// Gets or sets the optional list of the procedure's parameter names.
        /// </summary>
        public IList<string>? Parameters { get; init; }

        /// <summary>
        /// Gets or sets the procedure's body.
        /// </summary>
        public Body Body { get; init; }

        /// <summary>
        /// Gets or sets the evaluation environment the procedure belongs to.
        /// </summary>
        public IEvaluationEnvironment Environment { get; init; }

        /// <summary>
        /// Accepts a visitor that performs some operation on the user procedure and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the user procedure.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitUserProcedure(this);

        /// <summary>
        /// Accepts a visitor that performs this user procedure's application.
        /// </summary>
        /// <param name="visitor">The visitor that performs the procedure application.</param>
        /// <param name="evaluatedArguments">Optional evaluated arguments passed to the procedure's parameters.</param>
        /// <param name="evaluatorVisitor">An evaluator visitor that will evaluate the procedure's body.</param>
        /// <returns>The evaluation result of the procedure application.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The procedure is called with the wrong number of arguments.</exception>
        public override EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => visitor.ApplyUserProcedure(this, evaluatedArguments, evaluatorVisitor);
    }
}
