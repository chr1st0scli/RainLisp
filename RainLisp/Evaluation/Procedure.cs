namespace RainLisp.Evaluation
{
    public abstract class Procedure : EvaluationResult
    {
        public abstract object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, EvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor);
    }
}
