using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluation.Results
{
    public class UserProcedure : EvaluationResult
    {
        public UserProcedure(IList<string>? parameters, Body body, IEvaluationEnvironment environment)
        {
            Parameters = parameters;
            Body = body;
            Environment = environment;
        }

        public IList<string>? Parameters { get; init; }

        public Body Body { get; init; }

        public IEvaluationEnvironment Environment { get; init; }

        public override EvaluationResult AcceptVisitor(IProcedureApplicationVisitor visitor, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
            => visitor.ApplyUserProcedure(this, evaluatedArguments, evaluatorVisitor);
    }
}
