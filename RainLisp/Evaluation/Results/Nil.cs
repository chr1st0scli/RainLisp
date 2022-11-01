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

        public override string? ToString()
            => "()";
    }
}
