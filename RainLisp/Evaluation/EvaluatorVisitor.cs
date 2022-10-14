using RainLisp.AbstractSyntaxTree;
using RainLisp.Environment;

namespace RainLisp.Evaluation
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

        public object EvaluateIdentifier(Identifier identifier, IEvaluationEnvironment environment)
            => environment.LookupIdentifierValue(identifier.Name);

        public object EvaluateQuote(Quote quote)
            => throw new NotImplementedException();

        public object EvaluateAssignment(Assignment assignment, IEvaluationEnvironment environment)
        {
            // Defer the evaluation of the expression to get the value to assign to the identifier, until it is certain that the definition exists.
            var valueProvider = () => assignment.Value.AcceptVisitor(this, environment);

            environment.SetIdentifierValue(assignment.IdentifierName, valueProvider);

            return "undefined";
        }

        public object EvaluateDefinition(Definition definition, IEvaluationEnvironment environment)
        {
            // Evaluate the expression to get the initial value to assign to the identifier.
            var value = definition.Value.AcceptVisitor(this, environment);

            environment.DefineIdentifier(definition.IdentifierName, value);

            return "undefined";
        }

        public object EvaluateLambda(Lambda lambda, IEvaluationEnvironment environment)
            => new UserProcedure(lambda.Parameters, lambda.Body, environment);

        public object EvaluateIf(If ifExpression, IEvaluationEnvironment environment)
        {
            // Evaluate the predicate of the if expression.
            var conditionValue = ifExpression.Predicate.AcceptVisitor(this, environment);

            // Anything but false is true and the consequent part is evaluated.
            if (!conditionValue.Equals(false))
                return ifExpression.Consequent.AcceptVisitor(this, environment);

            // Otherwise, evaluate the optional alternative part.
            else if (ifExpression.Alternative != null)
                return ifExpression.Alternative.AcceptVisitor(this, environment);

            // If no alternative is provided.
            else
                return "undefined";
        }

        public object EvaluateBegin(Begin begin, IEvaluationEnvironment environment)
            => EvaluateSequence(begin.Expressions, environment);

        public object EvaluateApplication(Application application, IEvaluationEnvironment environment)
        {
            // Operator is either a lambda that is evaluated to a user procedure, or another application that returns a user procedure, 
            // or an identifier that evaluates to an already defined procedure (either user or primitive).
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

        public object EvaluateBody(Body body, IEvaluationEnvironment environment)
        {
            // If the body contains any definitions, evaluate them to establish them in the environment.
            if (body.Definitions?.Count > 0)
            {
                foreach (var definition in body.Definitions)
                    EvaluateDefinition(definition, environment);
            }

            return EvaluateSequence(body.Expressions, environment);
        }

        public object EvaluateProgram(Program program, IEvaluationEnvironment environment)
        {
            // Establish all definitions in the environment.
            foreach (var definition in program.Definitions)
                EvaluateDefinition(definition, environment);

            object result = "undefined";
            // Evaluate all program expressions and the return the last result.
            foreach (var expression in program.Expressions)
                result = expression.AcceptVisitor(this, environment);

            return result;
        }

        private object EvaluateSequence(IList<Expression> expressions, IEvaluationEnvironment environment)
        {
            // Evaluate every expression in order and return the result of the last one.
            object result = "undefined";
            foreach (var expression in expressions)
                result = expression.AcceptVisitor(this, environment);

            return result;
        }
    }
}
