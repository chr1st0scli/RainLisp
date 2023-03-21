using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents an evaluator that is capable of evaluating an abstract syntax tree and its components.
    /// </summary>
    public class EvaluatorVisitor : IEvaluatorVisitor
    {
        private readonly IProcedureApplicationVisitor _procedureApplicationVisitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluatorVisitor"/> class.
        /// </summary>
        /// <param name="procedureApplicationVisitor">An evaluator that is capable of evaluating user and primitive procedure applications (calls).</param>
        /// <exception cref="ArgumentNullException">The <paramref name="procedureApplicationVisitor"/> is null.</exception>
        public EvaluatorVisitor(IProcedureApplicationVisitor procedureApplicationVisitor)
            => _procedureApplicationVisitor = procedureApplicationVisitor ?? throw new ArgumentNullException(nameof(procedureApplicationVisitor));

        /// <summary>
        /// Returns the result of evaluating a numeric literal.
        /// </summary>
        /// <param name="numberLiteral">The numeric literal to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateNumberLiteral(NumberLiteral numberLiteral)
            => new NumberDatum(numberLiteral.Value);

        /// <summary>
        /// Returns the result of evaluating a string literal.
        /// </summary>
        /// <param name="stringLiteral">The string literal to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateStringLiteral(StringLiteral stringLiteral)
            => new StringDatum(stringLiteral.Value);

        /// <summary>
        /// Returns the result of evaluating a boolean literal.
        /// </summary>
        /// <param name="boolLiteral">The boolean literal to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateBooleanLiteral(BooleanLiteral boolLiteral)
            => new BoolDatum(boolLiteral.Value);

        /// <summary>
        /// Returns the result of evaluating an identifier.
        /// </summary>
        /// <param name="identifier">The identifier to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateIdentifier(Identifier identifier, IEvaluationEnvironment environment)
            => EvaluateWithDebugInfo(() => environment.LookupIdentifierValue(identifier.Name), identifier);

        /// <summary>
        /// Returns the result of evaluating a quote.
        /// </summary>
        /// <param name="quote">The quote to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateQuote(Quote quote, IEvaluationEnvironment environment)
            => EvaluateQuotable(quote.Quotable, environment);

        /// <summary>
        /// Returns the result of evaluating an assignment.
        /// </summary>
        /// <param name="assignment">The assignment to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateAssignment(Assignment assignment, IEvaluationEnvironment environment)
        {
            EvaluationResult Evaluate()
            {
                // Defer the evaluation of the expression to get the value to assign to the identifier, until it is certain that the definition exists.
                var valueProvider = () => assignment.Value.AcceptVisitor(this, environment);

                environment.SetIdentifierValue(assignment.IdentifierName, valueProvider);

                return Unspecified.GetUnspecified();
            }

            return EvaluateWithDebugInfo(Evaluate, assignment);
        }

        /// <summary>
        /// Returns the result of evaluating a definition.
        /// </summary>
        /// <param name="definition">The definition to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateDefinition(Definition definition, IEvaluationEnvironment environment)
        {
            // Evaluate the expression to get the initial value to assign to the identifier.
            var value = definition.Value.AcceptVisitor(this, environment);

            environment.DefineIdentifier(definition.IdentifierName, value);

            return Unspecified.GetUnspecified();
        }

        /// <summary>
        /// Returns the result of evaluating a lambda.
        /// </summary>
        /// <param name="lambda">The lambda to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateLambda(Lambda lambda, IEvaluationEnvironment environment)
            => new UserProcedure(lambda.Parameters, lambda.Body, environment);

        /// <summary>
        /// Returns the result of evaluating an if expression.
        /// </summary>
        /// <param name="ifExpression">The if expression to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateIf(If ifExpression, IEvaluationEnvironment environment)
        {
            // Evaluate the predicate of the if expression.
            var conditionValue = ifExpression.Predicate.AcceptVisitor(this, environment);

            // Anything but false is true and the consequent part is evaluated.
            if (conditionValue is not BoolDatum primitiveBool || primitiveBool.Value)
                return ifExpression.Consequent.AcceptVisitor(this, environment);

            // Otherwise, evaluate the optional alternative part.
            else if (ifExpression.Alternative != null)
                return ifExpression.Alternative.AcceptVisitor(this, environment);

            // If no alternative is provided.
            else
                return Unspecified.GetUnspecified();
        }

        /// <summary>
        /// Returns the result of evaluating a begin code block.
        /// </summary>
        /// <param name="begin">The begin code block to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateBegin(Begin begin, IEvaluationEnvironment environment)
            => EvaluateSequence(begin.Expressions, environment);

        /// <summary>
        /// Returns the result of evaluating a procedure application (call).
        /// </summary>
        /// <param name="application">The application to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateApplication(Application application, IEvaluationEnvironment environment)
        {
            EvaluationResult Evaluate()
            {
                // Operator is either a lambda that is evaluated to a user procedure, or another application that returns a user procedure, 
                // or an identifier that evaluates to an already defined procedure (either user or primitive).
                var procedure = application.Operator
                    .AcceptVisitor(this, environment);

                // Evaluate arguments from left to right.
                var evaluatedArguments = application.Operands
                    ?.Select(expr => expr.AcceptVisitor(this, environment))
                    .ToArray();

                return procedure.AcceptVisitor(_procedureApplicationVisitor, evaluatedArguments, this);
            }

            return EvaluateWithDebugInfo(Evaluate, application);
        }

        /// <summary>
        /// Returns the result of evaluating a procedure body.
        /// </summary>
        /// <param name="body">The body to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        public EvaluationResult EvaluateBody(Body body, IEvaluationEnvironment environment)
        {
            // If the body contains any definitions, evaluate them to establish them in the environment.
            if (body.Definitions?.Count > 0)
            {
                foreach (var definition in body.Definitions)
                    EvaluateDefinition(definition, environment);
            }

            return EvaluateSequence(body.Expressions, environment);
        }

        /// <summary>
        /// Returns the result of evaluating a program.
        /// </summary>
        /// <param name="program">The program to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the program's evaluation.</returns>
        public IEnumerable<EvaluationResult> EvaluateProgram(Program program, IEvaluationEnvironment environment)
        {
            if (program.DefinitionsAndExpressions == null || program.DefinitionsAndExpressions.Count == 0)
            {
                yield return Unspecified.GetUnspecified();
                yield break;
            }

            foreach (var node in program.DefinitionsAndExpressions)
                yield return node.AcceptVisitor(this, environment);
        }

        private EvaluationResult EvaluateSequence(IList<Expression> expressions, IEvaluationEnvironment environment)
        {
            // Evaluate every expression in order and return the result of the last one.
            EvaluationResult? result = null;
            foreach (var expression in expressions)
                result = expression.AcceptVisitor(this, environment);

            // result is not null because the syntax grammar specifies there is at least one expression in both a body and begin.
            return result!;
        }

        private static EvaluationResult EvaluateQuotable(Quotable quotable, IEvaluationEnvironment environment)
        {
            EvaluationResult MakeQuoteSymbolsList(IList<Quotable> quotables, int currentIndex)
            {
                var quoteSymbol = EvaluateQuotable(quotables[currentIndex], environment);

                if (currentIndex == quotables.Count - 1)
                    return new Pair(quoteSymbol, Nil.GetNil());

                return new Pair(quoteSymbol, MakeQuoteSymbolsList(quotables, ++currentIndex));
            }

            if (quotable.Text != null)
            {
                if (environment.TryGetQuoteSymbol(quotable.Text, out var quoteSymbol))
                    return quoteSymbol;
                else
                {
                    quoteSymbol = new QuoteSymbol(quotable.Text);
                    environment.RegisterQuoteSymbol(quoteSymbol);

                    return quoteSymbol;
                }
            }
            else
            {
                if (quotable.Quotables == null || quotable.Quotables.Count == 0)
                    return Nil.GetNil();

                return MakeQuoteSymbolsList(quotable.Quotables, 0);
            }
        }

        private static EvaluationResult EvaluateWithDebugInfo(Func<EvaluationResult> evaluateCallback, IDebugInfo debugInfoSource)
        {
            try
            {
                return evaluateCallback();
            }
            catch (EvaluationException ex)
            {
                ex.AddToCallStack(debugInfoSource);
                throw;
            }
        }
    }
}
