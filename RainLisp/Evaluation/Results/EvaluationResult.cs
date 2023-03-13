namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents the result of an evaluation.
    /// </summary>
    public abstract class EvaluationResult : IEquatable<EvaluationResult>
    {
        /// <summary>
        /// Accepts a visitor that performs some operation on the evaluation result and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the evaluation result.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public abstract T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor);

        /// <summary>
        /// Accepts a visitor that performs a procedure application if the current evaluation result is indeed a procedure.
        /// </summary>
        /// <param name="visitor">The visitor that performs the procedure application.</param>
        /// <param name="evaluatedArguments">Optional evaluated arguments passed to the procedure's parameters.</param>
        /// <param name="evaluatorVisitor">An evaluator visitor that will evaluate the procedure's body.</param>
        /// <returns>The evaluation result of the procedure application.</returns>
        /// <exception cref="NotProcedureException">Thrown by default since most evaluation results are not procedures.</exception>
        public virtual EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotProcedureException();

        /// <summary>
        /// Determines if the specified evaluation result is equal to the current object.
        /// By default, a reference equality check is performed.
        /// </summary>
        /// <param name="other">The evaluation result to compare the current object with.</param>
        /// <returns>true if the specified evaluation result is equal to the current object; otherwise, false.</returns>
        public virtual bool Equals(EvaluationResult? other)
            => base.Equals(other);

        #region Some needless overrides to make messages and warnings disappear.
        /// <summary>
        /// Determines if the specified object is equal to the current one.
        /// </summary>
        /// <param name="obj">The object to compare the current one with.</param>
        /// <returns>true if the specified object is equal to the current one; otherwise, false.</returns>
        public override bool Equals(object? obj)
            => base.Equals(obj);

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();
        #endregion
    }
}
