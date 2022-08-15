using RainLisp.AbstractSyntaxTree;
using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public interface IEvaluatorVisitor
    {
        object EvaluateNumberLiteral(NumberLiteral numberLiteral);

        object EvaluateStringLiteral(StringLiteral stringLiteral);

        object EvaluateBooleanLiteral(BooleanLiteral boolLiteral);

        object EvaluateIdentifier(Identifier identifier, IEvaluationEnvironment environment);

        object EvaluateQuote(Quote quote);

        object EvaluateAssignment(Assignment assignment, IEvaluationEnvironment environment);

        object EvaluateDefinition(Definition definition, IEvaluationEnvironment environment);

        object EvaluateLambda(Lambda lambda, IEvaluationEnvironment environment);

        object EvaluateIf(If ifExpression, IEvaluationEnvironment environment);

        object EvaluateBegin(Begin begin, IEvaluationEnvironment environment);

        object EvaluateApplication(Application application, IEvaluationEnvironment environment);

        object EvaluateBody(Body body, IEvaluationEnvironment environment);

        object EvaluateProgram(Program program, IEvaluationEnvironment environment);
    }
}
