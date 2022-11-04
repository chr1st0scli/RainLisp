namespace RainLisp.Evaluation.Results
{
    public class StringDatum : PrimitiveDatum<string>
    {
        public StringDatum(string value) : base(value)
        {
        }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitStringDatum(this);
    }
}
