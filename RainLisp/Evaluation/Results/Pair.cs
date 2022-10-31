namespace RainLisp.Evaluation.Results
{
    public class Pair : EvaluationResult
    {
        public Pair(EvaluationResult first, EvaluationResult second)
        {
            First = first;
            Second = second;
        }

        public EvaluationResult First { get; set; }

        public EvaluationResult Second { get; set; }
    }
}
