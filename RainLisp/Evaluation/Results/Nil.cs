namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a nil result of an evaluation, also known as the empty list.
    /// </summary>
    public class Nil : EvaluationResult
    {
        // A private constructor prohibits others from creating instances of this type.
        private Nil()
        {
        }

        private static Nil? nil;

        /// <summary>
        /// Returns the nil evaluation result which is the same instance in a system wide scope.
        /// </summary>
        /// <returns>The nil evaluation result.</returns>
        public static Nil GetNil()
        {
            nil ??= new Nil();
            return nil;
        }

        /// <summary>
        /// Accepts a visitor that performs some operation on the nil evaluation result and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the nil evaluation result.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitNil(this);
    }
}
