using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluator;

namespace RainLisp
{
    public class EvaluatorVisitor : IVisitor
    {
        public object VisitApplication(Application application, Environment environment)
        {
            var evaluatedOperator = application.Operator
                .AcceptVisitor(this, environment);

            var arguments = application.Operands
                ?.Select(expr => expr.AcceptVisitor(this, environment))
                .ToArray();

            // Primitive procedure
            if (evaluatedOperator is Func<double[], object> func)
            {
                return func(arguments?.Cast<double>().ToArray() ?? Array.Empty<double>());
            }
            else if (evaluatedOperator is Procedure procedure)
            {
                if (procedure.Parameters?.Count != application.Operands?.Count)
                    throw new InvalidOperationException("Wrong number of arguments.");

                // We extend the procedure environment instead of the give one?
                var env = procedure.Environment.ExtendEnvironment();

                if (procedure.Parameters?.Count > 0 && arguments?.Length > 0)
                {
                    for (int i = 0; i < procedure.Parameters.Count; i++)
                        env.SetIdentifier(procedure.Parameters[i], arguments[i]);
                }

                return VisitBody(procedure.Body, env);

            }
            else
                throw new InvalidOperationException("Unknown procedure type.");
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
            if (body.Definitions?.Count > 0)
            {
                foreach (var definition in body.Definitions)
                    definition.AcceptVisitor(this, environment);
            }

            return body.Expression.AcceptVisitor(this, environment);
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
            var primitiveProcedure = PrimitiveProcedures.GetPrimitiveProcedure(identifier.Name);

            if (primitiveProcedure != null)
                return primitiveProcedure;

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
            return new Procedure(lambda.Parameters, lambda.Body, environment);
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
