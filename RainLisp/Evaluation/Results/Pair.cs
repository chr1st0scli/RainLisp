namespace RainLisp.Evaluation.Results
{
    public class Pair : EvaluationResult
    {
        public Pair(EvaluationResult first, EvaluationResult second)
        {
            First = first;
            Second = second;
        }

        public EvaluationResult First { get; init; }

        public EvaluationResult Second { get; init; }
    }
}
