using RainLisp.Evaluation.Results;
using System.Diagnostics.CodeAnalysis;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents an entity that facilitates the environment model of evaluation. It defines the scope for function parameters 
    /// and an environment chain in a way such that identifiers declared in outer scopes are accessible in inner scopes.
    /// </summary>
    public class EvaluationEnvironment : IEvaluationEnvironment
    {
        private readonly IDictionary<string, EvaluationResult> _definitions;
        private EvaluationEnvironment? _previousEnvironment;

        // Quote symbols are unique accross an evaluation environment chain.
        private Dictionary<string, QuoteSymbol> _quoteSymbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationEnvironment"/> class.
        /// </summary>
        public EvaluationEnvironment()
        {
            _definitions = new Dictionary<string, EvaluationResult>();
            _quoteSymbols = new Dictionary<string, QuoteSymbol>();
        }

        /// <summary>
        /// Returns a new environment by extending the current environment instance.
        /// When a user procedure is called, the procedure's environment should be extended with the potential procedure's parameters
        /// bound to the evaluated arguments. Then, the body of the procedure should be executed on that extended environment.
        /// </summary>
        /// <param name="parameters">The optional function's parameter names to bind with the respective <paramref name="evaluatedArguments"/> in the extended environment.</param>
        /// <param name="evaluatedArguments">The optional argument values to bind with the respective <paramref name="parameters"/> in the extended environment.</param>
        /// <returns>The extended environment where each potential parameter name is bound to the respective argument.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The parameters given are not as many as the arguments.</exception>
        public IEvaluationEnvironment ExtendEnvironment(IList<string>? parameters, EvaluationResult[]? evaluatedArguments)
        {
            int parametersCount = parameters?.Count ?? 0;
            int argumentsCount = evaluatedArguments?.Length ?? 0;

            if (parametersCount != argumentsCount)
                throw new WrongNumberOfArgumentsException(argumentsCount, parametersCount);

            var extendedEnvironment = new EvaluationEnvironment
            {
                _previousEnvironment = this,
                // Quote symbols are shared between environments in a chain. I.e. they are unique per environment chain.
                _quoteSymbols = _quoteSymbols
            };

            for (int i = 0; i < parametersCount; i++)
                extendedEnvironment.DefineIdentifier(parameters![i], evaluatedArguments![i]);

            return extendedEnvironment;
        }

        /// <summary>
        /// Defines an identifier, or sets it if it already exists, in the current environment by binding it to <paramref name="value"/>.
        /// </summary>
        /// <param name="identifierName">The identifier name to bind with <paramref name="value"/>.</param>
        /// <param name="value">The result of an evaluation to be bound with <paramref name="identifierName"/>.</param>
        public void DefineIdentifier(string identifierName, EvaluationResult value)
            => _definitions[identifierName] = value;

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
        public void SetIdentifierValue(string identifierName, Func<EvaluationResult> valueProvider)
        {
            // Check that the identifier we want to assign to exists.
            LookupIdentifierValue(identifierName, out EvaluationEnvironment environment);

            environment.DefineIdentifier(identifierName, valueProvider());
        }

        /// <summary>
        /// Returns the value that is bound to <paramref name="identifierName"/>.
        /// It looks for the identifier by moving up the environment chain, from inner to outer scope.
        /// </summary>
        /// <param name="identifierName">The identifier to look for.</param>
        /// <returns>The value that is bound to the identifier if the latter is found.</returns>
        /// <exception cref="UnknownIdentifierException">The <paramref name="identifierName"/> is not found in the environment chain.</exception>
        public EvaluationResult LookupIdentifierValue(string identifierName)
            => LookupIdentifierValue(identifierName, out EvaluationEnvironment _);

        /// <summary>
        /// Returns the identifier names that are defined in the current environment.
        /// </summary>
        /// <returns>The identifier names that are defined in the current environment.</returns>
        public string[] GetIdentifierNames()
            => _definitions.Keys.ToArray();

        /// <summary>
        /// Registers a quote symbol in the current environment. Quote symbols are commonly unique in an environment chain.
        /// </summary>
        /// <param name="symbol">The symbol to register.</param>
        public void RegisterQuoteSymbol(QuoteSymbol symbol)
            => _quoteSymbols.TryAdd(symbol.SymbolText, symbol);

        /// <summary>
        /// Gets the quote symbol associated with the specified <paramref name="symbolText"/> in the current environment.
        /// </summary>
        /// <param name="symbolText">The text of the symbol to get.</param>
        /// <param name="quoteSymbol">When this method returns, contains the quote symbol associated with <paramref name="symbolText"/> if the latter is found; otherwise, it is null.</param>
        /// <returns>true if the quote symbol is found; otherwise, false.</returns>
        public bool TryGetQuoteSymbol(string symbolText, [MaybeNullWhen(false)] out QuoteSymbol quoteSymbol)
        {
            quoteSymbol = null;
            if (_quoteSymbols == null)
                return false;

            return _quoteSymbols.TryGetValue(symbolText, out quoteSymbol);
        }

        private EvaluationResult LookupIdentifierValue(string identifierName, out EvaluationEnvironment environment)
        {
            for (var env = this; env != null; env = env._previousEnvironment)
            {
                if (env._definitions.TryGetValue(identifierName, out EvaluationResult? value))
                {
                    environment = env;
                    return value;
                }
            }

            throw new UnknownIdentifierException(identifierName);
        }
    }
}
