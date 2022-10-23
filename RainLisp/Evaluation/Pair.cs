namespace RainLisp.Evaluation
{
    public class Pair : EvaluationResult
    {
        public Pair(EvaluationResult first, EvaluationResult second)
        {
            First = first ?? throw new ArgumentNullException(nameof(first));
            Second = second ?? throw new ArgumentNullException(nameof(second));
        }

        public EvaluationResult First { get; init; }

        public EvaluationResult Second { get; init; }
    }
}
