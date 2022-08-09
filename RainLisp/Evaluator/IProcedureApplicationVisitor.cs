namespace RainLisp.Evaluator
{
    public interface IProcedureApplicationVisitor
    {
        object ApplyUserProcedure(UserProcedure procedure, object[]? evaluatedArguments, Environment environment, IEvaluatorVisitor evaluatorVisitor);

        object ApplyPrimitiveProcedure(PrimitiveProcedure procedure, object[] evaluatedArguments);
    }
}
