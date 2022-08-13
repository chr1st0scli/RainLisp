using RainLisp.AbstractSyntaxTree;

namespace RainLisp.Evaluator
{
    public class EvaluatorVisitor : IEvaluatorVisitor
    {
        private readonly IProcedureApplicationVisitor _procedureApplicationVisitor;

        public EvaluatorVisitor(IProcedureApplicationVisitor procedureApplicationVisitor)
            => _procedureApplicationVisitor = procedureApplicationVisitor ?? throw new ArgumentNullException(nameof(procedureApplicationVisitor));

        public object EvaluateNumberLiteral(NumberLiteral numberLiteral)
            => numberLiteral.Value;

        public object EvaluateStringLiteral(StringLiteral stringLiteral)
            => stringLiteral.Value;

        public object EvaluateBooleanLiteral(BooleanLiteral boolLiteral)
            => boolLiteral.Value;

        public object EvaluateIdentifier(Identifier identifier, Environment environment)
        {
            // This should be handled in the environment!
            // User cannot redefine primitive procedures.
            var primitiveProcedure = PrimitiveProcedure.CreatePrimitiveProcedure(identifier.Name);

            if (primitiveProcedure != null)
                return primitiveProcedure;

            return environment.LookupIdentifierValue(identifier.Name);
        }

        public object EvaluateQuote(Quote quote)
            => throw new NotImplementedException();

        public object EvaluateAssignment(Assignment assignment, Environment environment)
        {
            // Defer the evaluation of the expression to get the value to assign to the identifier, until it is certain that the definition exists.
            var valueProvider = () => assignment.Value.AcceptVisitor(this, environment);

            environment.SetIdentifierValue(assignment.IdentifierName, valueProvider);

            return "undefined";
        }

        public object EvaluateDefinition(Definition definition, Environment environment)
        {
            // Evaluate the expression to get the initial value to assign to the identifier.
            var value = definition.Value.AcceptVisitor(this, environment);

            environment.DefineIdentifier(definition.IdentifierName, value);

            return "undefined";
        }

        public object EvaluateLambda(Lambda lambda, Environment environment)
            => new UserProcedure(lambda.Parameters, lambda.Body, environment);

        public object EvaluateIf(If ifExpression, Environment environment)
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

        public object EvaluateBegin(Begin begin, Environment environment)
        {
            // Evaluate every expression in order and return the result of the last one.
            object result = "undefined";
            foreach (var expression in begin.Expressions)
                result = expression.AcceptVisitor(this, environment);

            return result;
        }

        public object EvaluateApplication(Application application, Environment environment)
        {
            // Operator is either a lambda that is evaluated to a user procedure
            // or an identifier that evaluates to a defined procedure (either user or primitive).
            var evaluatedOperator = application.Operator
                .AcceptVisitor(this, environment);

            // Evaluate arguments from left to right.
            var evaluatedArguments = application.Operands
                ?.Select(expr => expr.AcceptVisitor(this, environment))
                .ToArray();

            if (evaluatedOperator is Procedure procedure)
                return procedure.AcceptVisitor(_procedureApplicationVisitor, evaluatedArguments, environment, this);
            else
                throw new InvalidOperationException("Unknown procedure type.");
        }

        public object EvaluateBody(Body body, Environment environment)
        {
            // If the body contains any definitions, evaluate them to establish them in the environment.
            if (body.Definitions?.Count > 0)
            {
                foreach (var definition in body.Definitions)
                    EvaluateDefinition(definition, environment);
            }

            // Evalute the single body's expression.
            return body.Expression.AcceptVisitor(this, environment);
        }

        public object EvaluateProgram(Program program)
        {
            // Establish all definitions in the environment.
            foreach (var definition in program.Definitions)
                EvaluateDefinition(definition, Environment.GlobalEnvironment);
            //definition.AcceptVisitor(this, Environment.RootEnvironment);

            object result = "undefined";
            // Evaluate all program expressions and the return the last result.
            foreach (var expression in program.Expressions)
                result = expression.AcceptVisitor(this, Environment.GlobalEnvironment);

            return result;
        }
    }
}
