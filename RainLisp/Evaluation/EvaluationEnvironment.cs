using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    public class EvaluationEnvironment : IEvaluationEnvironment
    {
        private readonly IDictionary<string, EvaluationResult> _definitions;
        private EvaluationEnvironment? _previousEnvironment;

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
