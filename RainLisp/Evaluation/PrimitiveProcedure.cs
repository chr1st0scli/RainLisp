using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public class PrimitiveProcedure : Procedure
    {
        public PrimitiveProcedure(PrimitiveProcedureType procedureType)
            => ProcedureType = procedureType;

        public PrimitiveProcedureType ProcedureType { get; init; }

        public override object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor)
            => visitor.ApplyPrimitiveProcedure(this, evaluatedArguments!); // A primitive procedure does not expect null for arguments.
    }
}
