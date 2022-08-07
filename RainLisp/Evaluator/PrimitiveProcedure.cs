namespace RainLisp.Evaluator
{
    public class PrimitiveProcedure : Procedure
    {
        public PrimitiveProcedure(PrimitiveProcedureType procedureType)
            => ProcedureType = procedureType;

        public PrimitiveProcedureType ProcedureType { get; init; }

        public override object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, Environment environment, IEvaluatorVisitor evaluatorVisitor)
        {
            return visitor.VisitPrimitiveProcedure(this, evaluatedArguments);
        }

        public static PrimitiveProcedure? CreatePrimitiveProcedure(string identifierName)
        {
            PrimitiveProcedureType? proc = identifierName switch
            {
                "+" => PrimitiveProcedureType.Add,
                "-" => PrimitiveProcedureType.Subtract,
                "*" => PrimitiveProcedureType.Multiply,
                "/" => PrimitiveProcedureType.Divide,
                "%" => PrimitiveProcedureType.Remainder,
                ">" => PrimitiveProcedureType.GreaterThan,
                ">=" => PrimitiveProcedureType.GreaterThanOrEqualTo,
                "<" => PrimitiveProcedureType.LessThan,
                "<=" => PrimitiveProcedureType.LessThanOrEqualTo,
                "and" => PrimitiveProcedureType.LogicalAnd,
                "or" => PrimitiveProcedureType.LogicalOr,
                "not" => PrimitiveProcedureType.LogicalNot,
                _ => null
            };

            return proc != null ? new PrimitiveProcedure(proc.Value) : null;
        }
    }
}
