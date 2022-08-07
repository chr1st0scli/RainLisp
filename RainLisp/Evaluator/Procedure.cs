namespace RainLisp.Evaluator
{
    public abstract class Procedure : EvaluationResult
    {
        public abstract object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, Environment environment, IEvaluatorVisitor evaluatorVisitor);
    }
}
