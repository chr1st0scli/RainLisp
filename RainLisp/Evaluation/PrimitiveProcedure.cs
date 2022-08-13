using static RainLisp.Grammar.Primitives;

namespace RainLisp.Evaluation
{
    public class PrimitiveProcedure : Procedure
    {
        public PrimitiveProcedure(PrimitiveProcedureType procedureType)
            => ProcedureType = procedureType;

        public PrimitiveProcedureType ProcedureType { get; init; }

        public override object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, EvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor)
        {
            return visitor.ApplyPrimitiveProcedure(this, evaluatedArguments!); // A primitive procedure does not expect null for arguments.
        }

        public static PrimitiveProcedure? CreatePrimitiveProcedure(string identifierName)
        {
            PrimitiveProcedureType? proc = identifierName switch
            {
                PLUS => PrimitiveProcedureType.Add,
                MINUS => PrimitiveProcedureType.Subtract,
                MULTIPLY => PrimitiveProcedureType.Multiply,
                DIVIDE => PrimitiveProcedureType.Divide,
                MODULO => PrimitiveProcedureType.Modulo,
                GREATER => PrimitiveProcedureType.GreaterThan,
                GREATER_OR_EQUAL => PrimitiveProcedureType.GreaterThanOrEqualTo,
                LESS => PrimitiveProcedureType.LessThan,
                LESS_OR_EQUAL => PrimitiveProcedureType.LessThanOrEqualTo,
                AND => PrimitiveProcedureType.LogicalAnd,
                OR => PrimitiveProcedureType.LogicalOr,
                XOR => PrimitiveProcedureType.LogicalXor,
                NOT => PrimitiveProcedureType.LogicalNot,
                _ => null
            };

            return proc != null ? new PrimitiveProcedure(proc.Value) : null;
        }
    }
}
