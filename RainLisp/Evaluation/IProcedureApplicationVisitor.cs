namespace RainLisp.Evaluation
{
    public interface IProcedureApplicationVisitor
    {
        object ApplyUserProcedure(UserProcedure procedure, object[]? evaluatedArguments, EvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor);

        object ApplyPrimitiveProcedure(PrimitiveProcedure procedure, object[] evaluatedArguments);
    }
}
