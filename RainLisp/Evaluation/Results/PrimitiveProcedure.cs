namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a primitive procedure as a result of an evaluation.
    /// </summary>
    public class PrimitiveProcedure : EvaluationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveProcedure"/> class.
        /// </summary>
        /// <param name="name">The name of the primitive procedure.</param>
        /// <param name="implementation">A callback that implements the procedure. It accepts optional evaluated arguments and returns an evaluation result.</param>
        public PrimitiveProcedure(string name, Func<EvaluationResult[]?, EvaluationResult> implementation)
        {
            Name = name;
            Implementation = implementation;
        }

        /// <summary>
        /// Gets or sets the name of the primitive procedure.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets or sets the callback that implements the procedure. It accepts optional evaluated arguments and returns an evaluation result.
        /// </summary>
        public Func<EvaluationResult[]?, EvaluationResult> Implementation { get; init; }

        /// <summary>
        /// Accepts a visitor that performs some operation on the primitive procedure and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the primitive procedure.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitPrimitiveProcedure(this);

        /// <summary>
        /// Accepts a visitor that performs this primitive procedure's application.
        /// </summary>
        /// <param name="visitor">The visitor that performs the procedure application.</param>
        /// <param name="evaluatedArguments">Optional evaluated arguments passed to the procedure's parameters.</param>
        /// <param name="evaluatorVisitor">An evaluator visitor capable of evaluating a procedure's body. It is ignored for a primitive procedure.</param>
        /// <returns>The evaluation result of the procedure application.</returns>
        public override EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => visitor.ApplyPrimitiveProcedure(this, evaluatedArguments);
    }
}
