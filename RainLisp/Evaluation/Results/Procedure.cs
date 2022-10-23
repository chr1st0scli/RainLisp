namespace RainLisp.Evaluation.Results
{
    public abstract class Procedure : EvaluationResult
    {
        public abstract EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor);
    }
}
