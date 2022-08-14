using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluation
{
    public interface IEvaluatorVisitor
    {
        object EvaluateNumberLiteral(NumberLiteral numberLiteral);

        object EvaluateStringLiteral(StringLiteral stringLiteral);

        object EvaluateBooleanLiteral(BooleanLiteral boolLiteral);

        object EvaluateIdentifier(Identifier identifier, EvaluationEnvironment environment);

        object EvaluateQuote(Quote quote);

        object EvaluateAssignment(Assignment assignment, EvaluationEnvironment environment);

        object EvaluateDefinition(Definition definition, EvaluationEnvironment environment);

        object EvaluateLambda(Lambda lambda, EvaluationEnvironment environment);

        object EvaluateIf(If ifExpression, EvaluationEnvironment environment);

        object EvaluateBegin(Begin begin, EvaluationEnvironment environment);

        object EvaluateApplication(Application application, EvaluationEnvironment environment);

        object EvaluateBody(Body body, EvaluationEnvironment environment);

        object EvaluateProgram(Program program, EvaluationEnvironment environment);
    }
}
