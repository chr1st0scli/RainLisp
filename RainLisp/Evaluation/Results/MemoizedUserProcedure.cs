using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a memoized user procedure as a result of an evaluation.
    /// </summary>
    public class MemoizedUserProcedure : UserProcedure
    {
        private EvaluationResult? _evaluationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoizedUserProcedure"/> class.
        /// </summary>
        /// <param name="parameters">An optional list of the procedure's parameter names.</param>
        /// <param name="body">The procedure's body.</param>
        /// <param name="environment">The evaluation environment the procedure is created in.</param>
        public MemoizedUserProcedure(IList<string>? parameters, Body body, IEvaluationEnvironment environment) : base(parameters, body, environment)
        {
        }

        /// <summary>
        /// Accepts a visitor that performs some operation on the memoized user procedure and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the memoized user procedure.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitMemoizedUserProcedure(this);

        /// <summary>
        /// Accepts a visitor that performs this memoized user procedure's application. The result is memoized and returned in subsequent applications.
        /// </summary>
        /// <param name="visitor">The visitor that performs the procedure application.</param>
        /// <param name="evaluatedArguments">Optional evaluated arguments passed to the procedure's parameters.</param>
        /// <param name="evaluatorVisitor">An evaluator visitor that will evaluate the procedure's body.</param>
        /// <returns>The memoized evaluation result of the procedure application.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The procedure is called with the wrong number of arguments.</exception>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <see cref="Body"/>.</exception>
        public override EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
        {
            if (_evaluationResult != null)
                return _evaluationResult;

            _evaluationResult = base.AcceptVisitor(visitor, evaluatedArguments, evaluatorVisitor);

            return _evaluationResult;
        }
    }
}
