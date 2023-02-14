namespace RainLisp.Evaluation.Results
{
    public abstract class EvaluationResult : IEquatable<EvaluationResult>
    {
        public abstract T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor);

        public virtual EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotProcedureException();

        public virtual bool Equals(EvaluationResult? other)
            => base.Equals(other);
    }
}
