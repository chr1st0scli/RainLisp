namespace RainLisp.Evaluation.Results
{
    public abstract class EvaluationResult : IEquatable<EvaluationResult>
    {
        public abstract T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor);

        public virtual EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => throw new NotProcedureException();

        public virtual bool Equals(EvaluationResult? other)
            => base.Equals(other);

        #region Some needless overrides to make messages and warnings disappear.
        public override bool Equals(object? obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();
        #endregion
    }
}
