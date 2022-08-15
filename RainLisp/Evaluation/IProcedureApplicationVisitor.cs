using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public interface IProcedureApplicationVisitor
    {
        object ApplyUserProcedure(UserProcedure procedure, object[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor);

        object ApplyPrimitiveProcedure(PrimitiveProcedure procedure, object[] evaluatedArguments);
    }
}
