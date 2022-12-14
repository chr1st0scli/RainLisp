using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    public interface IProcedureApplicationVisitor
    {
        EvaluationResult ApplyUserProcedure(UserProcedure procedure, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor);

        EvaluationResult ApplyPrimitiveProcedure(PrimitiveProcedure procedure, EvaluationResult[]? evaluatedArguments);
    }
}
