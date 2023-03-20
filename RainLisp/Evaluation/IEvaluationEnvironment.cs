using RainLisp.Evaluation.Results;
using System.Diagnostics.CodeAnalysis;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents an entity that facilitates the environment model of evaluation. It defines the scope for function parameters 
    /// and an environment chain in a way such that identifiers declared in outer scopes are accessible in inner scopes.
    /// </summary>
    public interface IEvaluationEnvironment
    {
        /// <summary>
        /// Returns a new environment by extending the current environment instance.
        /// When a user procedure is called, the procedure's environment should be extended with the potential procedure's parameters
        /// bound to the evaluated arguments. Then, the body of the procedure should be executed on that extended environment.
        /// </summary>
        /// <param name="parameters">The optional function's parameter names to bind with the respective <paramref name="evaluatedArguments"/> in the extended environment.</param>
        /// <param name="evaluatedArguments">The optional argument values to bind with the respective <paramref name="parameters"/> in the extended environment.</param>
        /// <returns>The extended environment where each potential parameter name is bound to the respective argument.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The parameters given are not as many as the arguments.</exception>
        IEvaluationEnvironment ExtendEnvironment(IList<string>? parameters, EvaluationResult[]? evaluatedArguments);

        /// <summary>
        /// Defines an identifier, or sets it if it already exists, in the current environment by binding it to <paramref name="value"/>.
        /// </summary>
        /// <param name="identifierName">The identifier name to bind with <paramref name="value"/>.</param>
        /// <param name="value">The result of an evaluation to be bound with <paramref name="identifierName"/>.</param>
        void DefineIdentifier(string identifierName, EvaluationResult value);

        /// <summary>
        /// It looks for the identifier name by moving up the environment chain, from inner to outer scope.
        /// If it is found, it is bound to the new value provided by <paramref name="valueProvider"/>.
        /// </summary>
        /// <param name="identifierName">The identifier to look for.</param>
        /// <param name="valueProvider">
        /// A deferred callback that returns the value to be bound with <paramref name="identifierName"/>.
        /// It is deferred because the evaluation to get the value, does not need to occur if the identifier does not exist.
        /// </param>
        /// <exception cref="UnknownIdentifierException">The <paramref name="identifierName"/> is not found in the environment chain.</exception>
        void SetIdentifierValue(string identifierName, Func<EvaluationResult> valueProvider);

        /// <summary>
        /// Returns the value that is bound to <paramref name="identifierName"/>.
        /// It looks for the identifier by moving up the environment chain, from inner to outer scope.
        /// </summary>
        /// <param name="identifierName">The identifier to look for.</param>
        /// <returns>The value that is bound to the identifier if the latter is found.</returns>
        /// <exception cref="UnknownIdentifierException">The <paramref name="identifierName"/> is not found in the environment chain.</exception>
        EvaluationResult LookupIdentifierValue(string identifierName);

        /// <summary>
        /// Returns the identifier names that are defined in the current environment.
        /// </summary>
        /// <returns>The identifier names that are defined in the current environment.</returns>
        string[] GetIdentifierNames();

        /// <summary>
        /// Registers a quote symbol in the current environment. Quote symbols are commonly unique in an environment chain.
        /// </summary>
        /// <param name="symbol">The symbol to register.</param>
        void RegisterQuoteSymbol(QuoteSymbol symbol);

        /// <summary>
        /// Gets the quote symbol associated with the specified <paramref name="symbolText"/> in the current environment.
        /// </summary>
        /// <param name="symbolText">The text of the symbol to get.</param>
        /// <param name="quoteSymbol">When this method returns, contains the quote symbol associated with <paramref name="symbolText"/> if the latter is found; otherwise, it is null.</param>
        /// <returns>true if the quote symbol is found; otherwise, false.</returns>
        bool TryGetQuoteSymbol(string symbolText, [MaybeNullWhen(false)] out QuoteSymbol quoteSymbol);
    }
}
