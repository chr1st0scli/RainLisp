namespace RainLisp.Evaluation.Results
{
    public class BoolDatum : PrimitiveDatum<bool>
    {
        public BoolDatum(bool value) : base(value)
        {
        }

        public override TResult AcceptVisitor<TResult>(IEvaluationResultVisitor<TResult> visitor)
            => visitor.VisitBoolDatum(this);
    }
}
