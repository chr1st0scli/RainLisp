namespace RainLisp.Evaluator
{
    public interface IProcedureApplicationVisitor
    {
        object VisitUserProcedure(UserProcedure procedure, object[]? evaluatedArguments, Environment environment, IEvaluatorVisitor evaluatorVisitor);

        object VisitPrimitiveProcedure(PrimitiveProcedure procedure, object[]? evaluatedArguments);
    }
}
