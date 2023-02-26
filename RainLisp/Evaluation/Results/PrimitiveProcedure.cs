namespace RainLisp.Evaluation.Results
{
    public class PrimitiveProcedure : EvaluationResult
    {
        public PrimitiveProcedure(string name, Func<EvaluationResult[]?, EvaluationResult> implementation)
        {
            Name = name;
            Implementation = implementation;
        }

        public string Name { get; init; }

        public Func<EvaluationResult[]?, EvaluationResult> Implementation { get; init; }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitPrimitiveProcedure(this);

        public override EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => visitor.ApplyPrimitiveProcedure(this, evaluatedArguments);
    }
}
