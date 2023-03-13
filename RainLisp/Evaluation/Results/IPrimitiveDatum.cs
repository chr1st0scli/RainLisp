namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a primitive datum as a result of an evaluation.
    /// </summary>
    public interface IPrimitiveDatum
    {
        /// <summary>
        /// Returns the underlying primitive datum's value as an object.
        /// </summary>
        /// <returns>The underlying primitive datum's value as an object.</returns>
        object GetValueAsObject();
    }
}
