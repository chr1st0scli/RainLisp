namespace RainLisp.Evaluation.Results
{
    public class PrimitiveDatum<T> : EvaluationResult, IPrimitiveDatum where T : notnull
    {
        public PrimitiveDatum(T value)
            => Value = value;

        public T Value { get; init; }

        public object GetValueAsObject() => Value;

        public override string? ToString()
            => Value.ToString();
    }
}
