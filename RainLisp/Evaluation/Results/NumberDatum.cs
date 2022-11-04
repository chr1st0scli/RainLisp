namespace RainLisp.Evaluation.Results
{
    public class NumberDatum : PrimitiveDatum<double>
    {
        public NumberDatum(double value) : base(value)
        {
        }

        public override TResult AcceptVisitor<TResult>(IEvaluationResultVisitor<TResult> visitor)
            => visitor.VisitNumberDatum(this);
    }
}
