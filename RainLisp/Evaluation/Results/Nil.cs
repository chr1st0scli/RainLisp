namespace RainLisp.Evaluation.Results
{
    public class Nil : EvaluationResult
    {
        private Nil()
        {
        }

        private static Nil? nil;

        public static Nil GetNil()
        {
            nil ??= new Nil();
            return nil;
        }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitNil(this);
    }
}
