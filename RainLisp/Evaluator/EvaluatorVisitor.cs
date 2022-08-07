using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluator
{
    public class EvaluatorVisitor : IEvaluatorVisitor
    {
        private readonly IProcedureApplicationVisitor _procedureVisitor;

        public EvaluatorVisitor(IProcedureApplicationVisitor procedureVisitor)
            => _procedureVisitor = procedureVisitor ?? throw new ArgumentNullException(nameof(procedureVisitor));

        public object VisitNumberLiteral(NumberLiteral numberLiteral)
            => numberLiteral.Value;

        public object VisitStringLiteral(StringLiteral stringLiteral)
            => stringLiteral.Value;

        public object VisitBooleanLiteral(BooleanLiteral boolLiteral)
            => boolLiteral.Value;

        public object VisitIdentifier(Identifier identifier, Environment environment)
        {
            // This should be handled in the environment!
            // User cannot redefine primitive procedures.
            var primitiveProcedure = PrimitiveProcedure.CreatePrimitiveProcedure(identifier.Name);

            if (primitiveProcedure != null)
                return primitiveProcedure;

            return environment.LookupIdentifier(identifier.Name);
        }

        public object VisitQuote(Quote quote)
            => throw new NotImplementedException();

        public object VisitAssignment(Assignment assignment, Environment environment)
        {
            // Check that the identifier we want to assign to exists.
            environment.LookupIdentifier(assignment.IdentifierName);

            // Evaluate the expression to get the value to assign to the identifier.
            var value = assignment.Value.AcceptVisitor(this, environment);

            environment.SetIdentifier(assignment.IdentifierName, value);

            return "undefined";
        }

        public object VisitDefinition(Definition definition, Environment environment)
        {
            // Evaluate the expression to get the initial value to assign to the identifier.
            var value = definition.Value.AcceptVisitor(this, environment);

            environment.SetIdentifier(definition.IdentifierName, value);

            return "undefined";
        }

        public object VisitLambda(Lambda lambda, Environment environment)
            => new UserProcedure(lambda.Parameters, lambda.Body, environment);

        public object VisitIf(If ifExpression, Environment environment)
        {
            // Evaluate the predicate of the if expression and if the result is a boolean, carry on.
            if (ifExpression.Predicate.AcceptVisitor(this, environment) is bool condition)
            {
                // If true, evaluate the consequent part.
                if (condition)
                    return ifExpression.Consequent.AcceptVisitor(this, environment);

                // Otherwise, evaluate the optional alternative part.
                else if (ifExpression.Alternative != null)
                    return ifExpression.Alternative.AcceptVisitor(this, environment);

                // If no alternative is provided.
                else
                    return "undefined";
            }
            else
                throw new InvalidOperationException();
        }

        public object VisitBegin(Begin begin, Environment environment)
        {
            // Evaluate every expression in order and return the result of the last one.
            object result = "undefined";
            foreach (var expression in begin.Expressions)
                result = expression.AcceptVisitor(this, environment);

            return result;
        }

        public object VisitApplication(Application application, Environment environment)
        {
            // Operator is either a lambda that is evaluate to a user procedure
            // or an identifier that evaluates to a defined procedure (either user or primitive).
            var evaluatedOperator = application.Operator
                .AcceptVisitor(this, environment);

            // Evaluate arguments from left to right.
            var evaluatedArguments = application.Operands
                ?.Select(expr => expr.AcceptVisitor(this, environment))
                .ToArray();

            if (evaluatedOperator is Procedure procedure)
                return procedure.AcceptVisitor(_procedureVisitor, evaluatedArguments, environment, this);
            else
                throw new InvalidOperationException("Unknown procedure type.");
        }

        public object VisitBody(Body body, Environment environment)
        {
            // If the body contains any definitions, evaluate them to establish them in the environment.
            if (body.Definitions?.Count > 0)
            {
                foreach (var definition in body.Definitions)
                    VisitDefinition(definition, environment);
            }

            // Evalute the single body's expression.
            return body.Expression.AcceptVisitor(this, environment);
        }

        public object VisitProgram(Program program)
        {
            // Establish all definitions in the environment.
            foreach (var definition in program.Definitions)
                VisitDefinition(definition, Environment.GlobalEnvironment);
            //definition.AcceptVisitor(this, Environment.RootEnvironment);

            object result = "undefined";
            // Evaluate all program expressions and the return the last result.
            foreach (var expression in program.Expressions)
                result = expression.AcceptVisitor(this, Environment.GlobalEnvironment);

            return result;
        }
    }
}
