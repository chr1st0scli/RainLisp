using RainLisp.AbstractSyntaxTree;

namespace RainLisp
{
    public class EvaluatorVisitor : IVisitor
    {
        public object VisitApplication(Application application, Environment environment)
        {
            throw new NotImplementedException();
        }

        public object VisitAssignment(Assignment assignment, Environment environment)
        {
            throw new NotImplementedException();
        }

        public object VisitBegin(Begin begin, Environment environment)
        {
            object result = "undefined";
            foreach (var expression in begin.Expressions)
                result = expression.AcceptVisitor(this, environment);

            return result;
        }

        public object VisitBody(Body body, Environment environment)
        {
            throw new NotImplementedException();
        }

        public object VisitBooleanLiteral(BooleanLiteral boolLiteral)
        {
            return boolLiteral.Value;
        }

        public object VisitDefinition(Definition definition, Environment environment)
        {
            var value = definition.Value.AcceptVisitor(this, environment);

            environment.SetIdentifier(definition.IdentifierName, value);

            return "undefined";
        }

        public object VisitIdentifier(Identifier identifier, Environment environment)
        {
            return environment.LookupIdentifier(identifier.Name);
        }

        public object VisitIf(If ifExpression, Environment environment)
        {
            if (ifExpression.Predicate.AcceptVisitor(this, environment) is bool condition)
            {
                if (condition)
                {
                    return ifExpression.Consequent.AcceptVisitor(this, environment);
                }
                else if (ifExpression.Alternative != null)
                {
                    return ifExpression.Alternative.AcceptVisitor(this, environment);
                }
                else
                    return "undefined";
            }
            else
                throw new InvalidOperationException();
        }

        public object VisitLambda(Lambda lambda, Environment environment)
        {
            throw new NotImplementedException();
        }

        public object VisitNumberLiteral(NumberLiteral numberLiteral)
        {
            return numberLiteral.Value;
        }

        public object VisitProgram(Program program)
        {
            foreach (var definition in program.Definitions)
                definition.AcceptVisitor(this, Environment.RootEnvironment);

            object result = "undefined";
            foreach (var expression in program.Expressions)
                result = expression.AcceptVisitor(this, Environment.RootEnvironment);

            return result;
        }

        public object VisitQuote(Quote quote)
        {
            throw new NotImplementedException();
        }

        public object VisitStringLiteral(StringLiteral stringLiteral)
        {
            return stringLiteral.Value;
        }
    }
}
