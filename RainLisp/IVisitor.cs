using RainLisp.AbstractSyntaxTree;

namespace RainLisp
{
    public interface IVisitor
    {
        void VisitNumberLiteral(NumberLiteral numberLiteral);

        void VisitStringLiteral(StringLiteral stringLiteral);

        void VisitBooleanLiteral(BooleanLiteral boolLiteral);

        void VisitIdentifier(Identifier identifier);

        void VisitQuote(Quote quote);

        void VisitAssignment(Assignment assignment);

        void VisitIf(If ifExpression);

        void VisitBegin(Begin begin);

        void VisitLambda(Lambda lambda);

        void VisitApplication(Application application);

        void VisitBody(Body body);

        void VisitDefinition(Definition definition);

        void VisitProgram(Program program);
    }
}
