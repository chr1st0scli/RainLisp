namespace RainLisp.Evaluation.Results
{
    public class PrimitiveDatum<T> : EvaluationResult, IPrimitiveDatum
    {
        public PrimitiveDatum(T value)
            => Value = value;

        public T Value { get; init; }

        public object GetValueAsObject() => Value!; // This is never null.
    }
}
