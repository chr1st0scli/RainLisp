using RainLisp.AbstractSyntaxTree;

namespace RainLisp
{
    public interface IVisitor
    {
        object VisitNumberLiteral(NumberLiteral numberLiteral);

        object VisitStringLiteral(StringLiteral stringLiteral);

        object VisitBooleanLiteral(BooleanLiteral boolLiteral);
        
        object VisitIdentifier(Identifier identifier, Environment environment);
        
        object VisitQuote(Quote quote);
        
        object VisitAssignment(Assignment assignment, Environment environment);
        
        object VisitIf(If ifExpression, Environment environment);
        
        object VisitBegin(Begin begin, Environment environment);
        
        object VisitLambda(Lambda lambda, Environment environment);
        
        object VisitApplication(Application application, Environment environment);
        
        object VisitBody(Body body, Environment environment);
        
        object VisitDefinition(Definition definition, Environment environment);
        
        object VisitProgram(Program program);
    }
}
