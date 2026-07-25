namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a boolean primitive datum as a result of an evaluation.
    /// </summary>
    /// <param name="value">The underlying primitive value.</param>
    public class BoolDatum(bool value) : PrimitiveDatum<bool>(value)
    {
        /// <summary>
        /// Accepts a visitor that performs some operation on the boolean primitive and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the boolean primitive.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitBoolDatum(this);
    }
}
