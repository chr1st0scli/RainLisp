using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public abstract class Procedure : EvaluationResult
    {
        public abstract object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor);
    }
}
