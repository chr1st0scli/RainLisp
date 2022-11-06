namespace RainLisp.Evaluation.Results
{
    public class ProgramResult : EvaluationResult
    {
        public IList<EvaluationResult>? Results { get; set; }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitProgramResult(this);
    }
}
