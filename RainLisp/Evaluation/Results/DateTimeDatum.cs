namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a date and time primitive datum as a result of an evaluation.
    /// </summary>
    public class DateTimeDatum : PrimitiveDatum<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeDatum"/> class.
        /// </summary>
        /// <param name="value">The underlying primitive value.</param>
        public DateTimeDatum(DateTime value) : base(value)
        {
        }

        /// <summary>
        /// Accepts a visitor that performs some operation on the date and time primitive and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the date and time primitive.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitDateTimeDatum(this);
    }
}
