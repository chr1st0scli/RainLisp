namespace RainLisp.Evaluation.Results
{
    public abstract class PrimitiveDatum<T> : EvaluationResult, IPrimitiveDatum where T : notnull
    {
        public PrimitiveDatum(T value)
            => Value = value;

        public T Value { get; init; }

        public object GetValueAsObject() => Value;

        public override bool Equals(EvaluationResult? other)
            => this == other;

        public override bool Equals(object? obj)
            => this == obj as EvaluationResult;

        public override int GetHashCode()
            => Value.GetHashCode();

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

        public static bool operator !=(PrimitiveDatum<T>? left, EvaluationResult? right)
            => !(left == right);
    }
}
