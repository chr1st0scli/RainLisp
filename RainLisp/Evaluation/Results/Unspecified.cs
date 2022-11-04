namespace RainLisp.Evaluation.Results
{
    public class Unspecified : EvaluationResult
    {
        private Unspecified()
        {
        }

        private static Unspecified? unspecified;

        public static Unspecified GetUnspecified()
        {
            unspecified ??= new Unspecified();
            return unspecified;
        }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitUnspecified(this);
    }
}
