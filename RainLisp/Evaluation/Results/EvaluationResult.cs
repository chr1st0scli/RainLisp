namespace RainLisp.Evaluation.Results
{
    public class EvaluationResult
    {
        public virtual EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotImplementedException();
    }
}
