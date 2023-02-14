namespace RainLisp.Evaluation.Results
{
    public abstract class PrimitiveDatum<T> : EvaluationResult, IPrimitiveDatum where T : notnull
    {
        public PrimitiveDatum(T value)
            => Value = value;

        public T Value { get; init; }

        public object GetValueAsObject() => Value;

        public override bool Equals(EvaluationResult? other)
        {
            if (other is PrimitiveDatum<T> datum)
                return Value.Equals(datum.Value);

            return base.Equals(other);
        }
    }
}
