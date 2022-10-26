namespace RainLisp.Evaluation.Results
{
    public class EvaluationResult
    {
        public virtual EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotImplementedException();
    }
}
