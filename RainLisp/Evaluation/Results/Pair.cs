namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents the basic data structure as a result of an evaluation.
    /// It consists of two parts and based on it, other more complex data structures can be built.
    /// </summary>
    /// <param name="first">The first part of the pair.</param>
    /// <param name="second">The second part of the pair.</param>
    public class Pair(EvaluationResult first, EvaluationResult second) : EvaluationResult
    {
        /// <summary>
        /// Gets or sets the first part of the pair.
        /// </summary>
        public EvaluationResult First { get; set; } = first;

        /// <summary>
        /// Gets or sets the second part of the pair.
        /// </summary>
        public EvaluationResult Second { get; set; } = second;

        /// <summary>
        /// Accepts a visitor that performs some operation on the pair and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the pair.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitPair(this);
    }
}
