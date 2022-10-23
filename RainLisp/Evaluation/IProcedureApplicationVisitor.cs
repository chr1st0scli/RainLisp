using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public interface IProcedureApplicationVisitor
    {
        EvaluationResult ApplyUserProcedure(UserProcedure procedure, EvaluationResult[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor);

        EvaluationResult ApplyPrimitiveProcedure(PrimitiveProcedure procedure, EvaluationResult[] evaluatedArguments);
    }
}
