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

        public override string? ToString()
            => string.Empty;
    }
}
