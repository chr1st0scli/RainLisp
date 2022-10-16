namespace RainLisp.Evaluation
{
    public class Pair : EvaluationResult
    {
        public Pair(object car, object cdr)
        {
            Car = car ?? throw new ArgumentNullException(nameof(car));
            Cdr = cdr ?? throw new ArgumentNullException(nameof(cdr));
        }

        public object Car { get; init; }

        public object Cdr { get; init; }
    }
}
