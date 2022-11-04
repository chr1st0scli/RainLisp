namespace RainLisp.Evaluation.Results
{
    public class PrimitiveDatum<T> : EvaluationResult, IPrimitiveDatum where T : notnull
    {
        public PrimitiveDatum(T value)
            => Value = value;

        public T Value { get; init; }

        public object GetValueAsObject() => Value;

        public override TResult AcceptVisitor<TResult>(IEvaluationResultVisitor<TResult> visitor)
            => visitor.VisitPrimitiveDatum(this);
    }
}
