using RainLisp.AbstractSyntaxTree;
using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public interface IEvaluatorVisitor
    {
        PrimitiveDatum EvaluateNumberLiteral(NumberLiteral numberLiteral);

        PrimitiveDatum EvaluateStringLiteral(StringLiteral stringLiteral);

        PrimitiveDatum EvaluateBooleanLiteral(BooleanLiteral boolLiteral);

        EvaluationResult EvaluateIdentifier(Identifier identifier, IEvaluationEnvironment environment);

        EvaluationResult EvaluateQuote(Quote quote);

        Unspecified EvaluateAssignment(Assignment assignment, IEvaluationEnvironment environment);

        Unspecified EvaluateDefinition(Definition definition, IEvaluationEnvironment environment);

        UserProcedure EvaluateLambda(Lambda lambda, IEvaluationEnvironment environment);

        EvaluationResult EvaluateIf(If ifExpression, IEvaluationEnvironment environment);

        EvaluationResult EvaluateBegin(Begin begin, IEvaluationEnvironment environment);

        EvaluationResult EvaluateApplication(Application application, IEvaluationEnvironment environment);

        EvaluationResult EvaluateBody(Body body, IEvaluationEnvironment environment);

        EvaluationResult EvaluateProgram(Program program, IEvaluationEnvironment environment);
    }
}
