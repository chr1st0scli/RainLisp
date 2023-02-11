using RainLisp.Evaluation.Results;
using System.Diagnostics.CodeAnalysis;

namespace RainLisp.Evaluation
{
    public class EvaluationEnvironment : IEvaluationEnvironment
    {
        private readonly IDictionary<string, EvaluationResult> _definitions;
        private EvaluationEnvironment? _previousEnvironment;

        // Quote symbols are unique accross the entire system.
        private static Dictionary<string, QuoteSymbol>? _quoteSymbols;

        public EvaluationEnvironment()
            => _definitions = new Dictionary<string, EvaluationResult>();

        public IEvaluationEnvironment ExtendEnvironment(IList<string>? parameters, EvaluationResult[]? evaluatedArguments)
        {
            int parametersCount = parameters?.Count ?? 0;
            int argumentsCount = evaluatedArguments?.Length ?? 0;

            if (parametersCount != argumentsCount)
                throw new WrongNumberOfArgumentsException(argumentsCount, parametersCount);

            var extendedEnvironment = new EvaluationEnvironment { _previousEnvironment = this };

            for (int i = 0; i < parametersCount; i++)
                extendedEnvironment.DefineIdentifier(parameters![i], evaluatedArguments![i]);

            return extendedEnvironment;
        }

        public void DefineIdentifier(string identifierName, EvaluationResult value)
            => _definitions[identifierName] = value;

        public void SetIdentifierValue(string identifierName, Func<EvaluationResult> valueProvider)
        {
            // Check that the identifier we want to assign to exists.
            LookupIdentifierValue(identifierName, out EvaluationEnvironment environment);

            environment.DefineIdentifier(identifierName, valueProvider());
        }

        public EvaluationResult LookupIdentifierValue(string identifierName)
            => LookupIdentifierValue(identifierName, out EvaluationEnvironment _);

        public string[] GetIdentifierNames()
            => _definitions.Keys.ToArray();

        public static void RegisterQuoteSymbol(QuoteSymbol symbol)
        {
            _quoteSymbols ??= new();
            _quoteSymbols.TryAdd(symbol.SymbolText, symbol);
        }

        public static bool TryGetQuoteSymbol(string symbolText, [MaybeNullWhen(false)] out QuoteSymbol quoteSymbol)
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
