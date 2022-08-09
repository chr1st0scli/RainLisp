using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluator
{
    public interface IEvaluatorVisitor
    {
        object EvaluateNumberLiteral(NumberLiteral numberLiteral);

        object EvaluateStringLiteral(StringLiteral stringLiteral);

        object EvaluateBooleanLiteral(BooleanLiteral boolLiteral);

        object EvaluateIdentifier(Identifier identifier, Environment environment);

        object EvaluateQuote(Quote quote);

        object EvaluateAssignment(Assignment assignment, Environment environment);

        object EvaluateDefinition(Definition definition, Environment environment);

        object EvaluateLambda(Lambda lambda, Environment environment);

        object EvaluateIf(If ifExpression, Environment environment);

        object EvaluateBegin(Begin begin, Environment environment);

        object EvaluateApplication(Application application, Environment environment);

        object EvaluateBody(Body body, Environment environment);

        object EvaluateProgram(Program program);
    }
}
