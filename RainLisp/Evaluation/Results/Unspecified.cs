namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents an unspecified result of an evaluation.
    /// </summary>
    public class Unspecified : EvaluationResult
    {
        // A private constructor prohibits others from creating instances of this type.
        private Unspecified()
        {
        }

        private static Unspecified? unspecified;

        /// <summary>
        /// Returns the unspecified evaluation result which is the same instance in a system wide scope.
        /// </summary>
        /// <returns>The unspecified evaluation result.</returns>
        public static Unspecified GetUnspecified()
        {
            unspecified ??= new Unspecified();
            return unspecified;
        }

        /// <summary>
        /// Accepts a visitor that performs some operation on the unspecified evaluation result and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the unspecified evaluation result.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitUnspecified(this);
    }
}
