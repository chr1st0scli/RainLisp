namespace RainLisp.Evaluation.Results
{
    public class NumberDatum : PrimitiveDatum<double>
    {
        public NumberDatum(double value) : base(value)
        {
        }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitNumberDatum(this);
    }
}
