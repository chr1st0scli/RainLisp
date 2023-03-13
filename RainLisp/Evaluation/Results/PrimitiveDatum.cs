namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a primitive datum as a result of an evaluation.
    /// </summary>
    /// <typeparam name="T">The underlying primitive datum's type.</typeparam>
    public abstract class PrimitiveDatum<T> : EvaluationResult, IPrimitiveDatum where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveDatum{T}"/> class.
        /// </summary>
        /// <param name="value">The underlying primitive datum's value.</param>
        public PrimitiveDatum(T value)
            => Value = value;

        /// <summary>
        /// Gets or sets the primitive datum's value.
        /// </summary>
        public T Value { get; init; }

        /// <summary>
        /// Returns the underlying primitive datum's value as an object.
        /// </summary>
        /// <returns>The underlying primitive datum's value as an object.</returns>
        public object GetValueAsObject() => Value;

        /// <summary>
        /// Determines if the specified evaluation result is equal to the current primitive datum.
        /// If <paramref name="other"/> is also a primitive datum, the underlying primitive values are compared; otherwise, a reference equality check is performed.
        /// </summary>
        /// <param name="other">The evaluation result to compare the current primitive datum with.</param>
        /// <returns>true if the specified evaluation result is equal to the current primitive datum; otherwise, false.</returns>
        public override bool Equals(EvaluationResult? other)
            => this == other;

        /// <summary>
        /// Determines if the specified object is equal to the current primitive datum.
        /// If <paramref name="obj"/> is also a primitive datum, the underlying primitive values are compared; otherwise, a reference equality check is performed.
        /// </summary>
        /// <param name="obj">The object to compare the current primitive datum with.</param>
        /// <returns>true if the specified object is equal to the current primitive datum; otherwise, false.</returns>
        public override bool Equals(object? obj)
            => this == obj as EvaluationResult;

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => Value.GetHashCode();

        /// <summary>
        /// Compares a primitive datum and another evaluation result for equality.
        /// If <paramref name="right"/> is also a primitive datum, the underlying primitive values are compared; otherwise, a reference equality check is performed.
        /// </summary>
        /// <param name="left">The primitive datum to compare.</param>
        /// <param name="right">The evaluation result to compare.</param>
        /// <returns>true if <paramref name="right"/> is equal to <paramref name="left"/>; otherwise, false.</returns>
        public static bool operator ==(PrimitiveDatum<T>? left, EvaluationResult? right)
        {
            if (left is null || right is null)
                return ReferenceEquals(left, right);

            if (ReferenceEquals(left, right))
                return true;

            if (right is PrimitiveDatum<T> rightAsPrimitive)
                return left.Value.Equals(rightAsPrimitive.Value);

            return false;
        }

        /// <summary>
        /// Compares a primitive datum and another evaluation result for inequality.
        /// If <paramref name="right"/> is also a primitive datum, the underlying primitive values are compared; otherwise, a reference equality check is performed.
        /// </summary>
        /// <param name="left">The primitive datum to compare.</param>
        /// <param name="right">The evaluation result to compare.</param>
        /// <returns>true if <paramref name="right"/> is not equal to <paramref name="left"/>; otherwise, false.</returns>
        public static bool operator !=(PrimitiveDatum<T>? left, EvaluationResult? right)
            => !(left == right);
    }
}
