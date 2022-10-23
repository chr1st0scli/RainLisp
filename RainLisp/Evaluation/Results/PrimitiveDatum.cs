namespace RainLisp.Evaluation.Results
{
    public class PrimitiveDatum : EvaluationResult
    {
        public PrimitiveDatum(object value)
            => Value = value;

        public object Value { get; init; }
    }
}
