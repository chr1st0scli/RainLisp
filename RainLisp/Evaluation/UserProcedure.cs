using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluation
{
    public class UserProcedure : Procedure
    {
        public UserProcedure(IList<string>? parameters, Body body, EvaluationEnvironment environment)
        {
            Parameters = parameters;
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IList<string>? Parameters { get; init; }

        public Body Body { get; init; }

        public EvaluationEnvironment Environment { get; init; }

        public override object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, EvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor)
        {
            return visitor.ApplyUserProcedure(this, evaluatedArguments, environment, evaluatorVisitor);
        }
    }
}
