namespace RainLisp.Evaluation.Results
{
    public class StringDatum : PrimitiveDatum<string>
    {
        public StringDatum(string value) : base(value)
        {
        }

        public override TResult AcceptVisitor<TResult>(IEvaluationResultVisitor<TResult> visitor)
            => visitor.VisitStringDatum(this);
    }
}
