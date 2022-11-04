namespace RainLisp.Evaluation.Results
{
    public abstract class PrimitiveDatum<T> : EvaluationResult, IPrimitiveDatum where T : notnull
    {
        public PrimitiveDatum(T value)
            => Value = value;

        public T Value { get; init; }

        public object GetValueAsObject() => Value;
    }
}
