using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public abstract class Procedure : EvaluationResult
    {
        public abstract EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor);
    }
}
