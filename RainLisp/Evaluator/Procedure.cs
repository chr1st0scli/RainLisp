namespace RainLisp.Evaluator
{
    public class Procedure : EvaluationResult
    {
        public virtual object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, Environment environment, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotImplementedException();
    }
}
