namespace RainLisp.Evaluation.Results
{
    public class PrimitiveDatum<T> : EvaluationResult
    {
        public PrimitiveDatum(T value)
            => Value = value;

        public T Value { get; init; }
    }
}
