namespace RainLisp.Evaluation.Results
{
    public class BoolDatum : PrimitiveDatum<bool>
    {
        public BoolDatum(bool value) : base(value)
        {
        }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitBoolDatum(this);
    }
}
