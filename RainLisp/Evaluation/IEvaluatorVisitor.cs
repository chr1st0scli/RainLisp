using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents an evaluator that is capable of evaluating an abstract syntax tree and its components.
    /// </summary>
    public interface IEvaluatorVisitor
    {
        /// <summary>
        /// Returns the result of evaluating a numeric literal.
        /// </summary>
        /// <param name="numberLiteral">The numeric literal to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        EvaluationResult EvaluateNumberLiteral(NumberLiteral numberLiteral);

        /// <summary>
        /// Returns the result of evaluating a string literal.
        /// </summary>
        /// <param name="stringLiteral">The string literal to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        EvaluationResult EvaluateStringLiteral(StringLiteral stringLiteral);

        /// <summary>
        /// Returns the result of evaluating a boolean literal.
        /// </summary>
        /// <param name="boolLiteral">The boolean literal to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        EvaluationResult EvaluateBooleanLiteral(BooleanLiteral boolLiteral);

        /// <summary>
        /// Returns the result of evaluating an identifier.
        /// </summary>
        /// <param name="identifier">The identifier to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="UnknownIdentifierException">The <paramref name="identifier"/> is not defined.</exception>
        EvaluationResult EvaluateIdentifier(Identifier identifier, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a quote.
        /// </summary>
        /// <param name="quote">The quote to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        EvaluationResult EvaluateQuote(Quote quote, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating an assignment.
        /// </summary>
        /// <param name="assignment">The assignment to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="UnknownIdentifierException">The <paramref name="assignment"/>'s identifier name is not defined.</exception>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <paramref name="assignment"/>'s value.</exception>
        EvaluationResult EvaluateAssignment(Assignment assignment, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a definition.
        /// </summary>
        /// <param name="definition">The definition to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <paramref name="definition"/>'s value.</exception>
        EvaluationResult EvaluateDefinition(Definition definition, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a lambda.
        /// </summary>
        /// <param name="lambda">The lambda to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        EvaluationResult EvaluateLambda(Lambda lambda, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating an if expression.
        /// </summary>
        /// <param name="ifExpression">The if expression to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <paramref name="ifExpression"/>.</exception>
        EvaluationResult EvaluateIf(If ifExpression, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a begin code block.
        /// </summary>
        /// <param name="begin">The begin code block to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <paramref name="begin"/>.</exception>
        EvaluationResult EvaluateBegin(Begin begin, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a procedure application (call).
        /// </summary>
        /// <param name="application">The application to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="NotProcedureException">The <paramref name="application"/>'s operator does not evaluate to a procedure.</exception>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <paramref name="application"/>.</exception>
        EvaluationResult EvaluateApplication(Application application, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a procedure body.
        /// </summary>
        /// <param name="body">The body to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <paramref name="body"/>.</exception>
        EvaluationResult EvaluateBody(Body body, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a delay.
        /// </summary>
        /// <param name="delay">The delay to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>The result of the evaluation.</returns>
        EvaluationResult EvaluateDelay(Delay delay, IEvaluationEnvironment environment);

        /// <summary>
        /// Returns the result of evaluating a program. The evaluation occurs lazily on a per request basis, while the return value is being enumerated.
        /// </summary>
        /// <param name="program">The program to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the program's evaluation.</returns>
        /// <exception cref="EvaluationException">An error occurs during the evaluation of the <paramref name="program"/>.</exception>
        IEnumerable<EvaluationResult> EvaluateProgram(Program program, IEvaluationEnvironment environment);
    }
}
