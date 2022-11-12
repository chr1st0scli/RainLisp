namespace RainLisp.Evaluation.Results
{
    public class DateTimeDatum : PrimitiveDatum<DateTime>
    {
        public DateTimeDatum(DateTime value) : base(value)
        {
        }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitDateTimeDatum(this);
    }
}
