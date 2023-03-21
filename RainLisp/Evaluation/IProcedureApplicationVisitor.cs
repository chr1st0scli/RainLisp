using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents a visitor that is capable of applying, i.e. calling, user and primitive procedures.
    /// </summary>
    public interface IProcedureApplicationVisitor
    {
        /// <summary>
        /// Returns the result of applying a user procedure.
        /// </summary>
        /// <param name="procedure">The user procedure to be called. It is the result of an evaluation.</param>
        /// <param name="evaluatedArguments">The evaluated arguments to be passed to the procedure's parameters.</param>
        /// <param name="evaluatorVisitor">An evaluator that is capable of evaluating the body of the procedure.</param>
        /// <returns>The result of calling the user procedure.</returns>
        EvaluationResult ApplyUserProcedure(UserProcedure procedure, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor);

        /// <summary>
        /// Returns the result of applying a primitive procedure.
        /// </summary>
        /// <param name="procedure">The primitive procedure to be called. It is the result of an evaluation.</param>
        /// <param name="evaluatedArguments">The evaluated arguments to be passed to the procedure's parameters.</param>
        /// <returns>The result of calling the primitive procedure.</returns>
        EvaluationResult ApplyPrimitiveProcedure(PrimitiveProcedure procedure, EvaluationResult[]? evaluatedArguments);
    }
}
