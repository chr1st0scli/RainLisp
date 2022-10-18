namespace RainLisp.Evaluation
{
    public class Pair : EvaluationResult
    {
        public Pair(object first, object second)
        {
            First = first ?? throw new ArgumentNullException(nameof(first));
            Second = second ?? throw new ArgumentNullException(nameof(second));
        }

        public object First { get; init; }

        public object Second { get; init; }
    }
}
