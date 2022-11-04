namespace RainLisp.Evaluation.Results
{
    public class PrimitiveProcedure : EvaluationResult
    {
        public PrimitiveProcedure(PrimitiveProcedureType procedureType)
            => ProcedureType = procedureType;

        public PrimitiveProcedureType ProcedureType { get; init; }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitPrimitiveProcedure(this);

        public override EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => visitor.ApplyPrimitiveProcedure(this, evaluatedArguments);
    }
}
