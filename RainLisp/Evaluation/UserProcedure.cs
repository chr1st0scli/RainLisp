using RainLisp.AbstractSyntaxTree;
using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public class UserProcedure : Procedure
    {
        public UserProcedure(IList<string>? parameters, Body body, IEvaluationEnvironment environment)
        {
            Parameters = parameters;
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IList<string>? Parameters { get; init; }

        public Body Body { get; init; }

        public IEvaluationEnvironment Environment { get; init; }

        public override object AcceptVisitor(IProcedureApplicationVisitor visitor, object[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor)
            => visitor.ApplyUserProcedure(this, evaluatedArguments, environment, evaluatorVisitor);
    }
}
