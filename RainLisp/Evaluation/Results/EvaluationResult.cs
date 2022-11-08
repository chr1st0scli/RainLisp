namespace RainLisp.Evaluation.Results
{
    public abstract class EvaluationResult
    {
        public abstract T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor);

        public virtual EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotProcedureException();
    }
}
