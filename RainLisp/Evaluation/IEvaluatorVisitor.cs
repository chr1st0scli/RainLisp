using RainLisp.AbstractSyntaxTree;
using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public interface IEvaluatorVisitor
    {
        EvaluationResult EvaluateNumberLiteral(NumberLiteral numberLiteral);

        EvaluationResult EvaluateStringLiteral(StringLiteral stringLiteral);

        EvaluationResult EvaluateBooleanLiteral(BooleanLiteral boolLiteral);

        EvaluationResult EvaluateIdentifier(Identifier identifier, IEvaluationEnvironment environment);

        EvaluationResult EvaluateQuote(Quote quote);

        EvaluationResult EvaluateAssignment(Assignment assignment, IEvaluationEnvironment environment);

        EvaluationResult EvaluateDefinition(Definition definition, IEvaluationEnvironment environment);

        EvaluationResult EvaluateLambda(Lambda lambda, IEvaluationEnvironment environment);

        EvaluationResult EvaluateIf(If ifExpression, IEvaluationEnvironment environment);

        EvaluationResult EvaluateBegin(Begin begin, IEvaluationEnvironment environment);

        EvaluationResult EvaluateApplication(Application application, IEvaluationEnvironment environment);

        EvaluationResult EvaluateBody(Body body, IEvaluationEnvironment environment);

        EvaluationResult EvaluateProgram(Program program, IEvaluationEnvironment environment);
    }
}
