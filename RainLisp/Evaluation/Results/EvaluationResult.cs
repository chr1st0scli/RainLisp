namespace RainLisp.Evaluation.Results
{
    public abstract class EvaluationResult
    {
        public abstract TResult AcceptVisitor<TResult>(IEvaluationResultVisitor<TResult> visitor);

        public virtual EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotImplementedException();
    }
}
