namespace RainLisp.Environment
{
    public class EvaluationEnvironment : IEvaluationEnvironment
    {
        private readonly IDictionary<string, object> definitions;

        private EvaluationEnvironment? previousEnvironment;

        public EvaluationEnvironment()
        {
            definitions = new Dictionary<string, object>();
        }

        public IEvaluationEnvironment ExtendEnvironment(IList<string>? parameters, object[]? evaluatedArguments)
        {
            if (parameters?.Count != evaluatedArguments?.Length)
                throw new InvalidOperationException("Wrong number of arguments.");

            var extendedEnvironment = new EvaluationEnvironment { previousEnvironment = this };

            if (parameters?.Count > 0 && evaluatedArguments?.Length > 0)
            {
                for (int i = 0; i < parameters.Count; i++)
                    extendedEnvironment.DefineIdentifier(parameters[i], evaluatedArguments[i]);
            }

            return extendedEnvironment;
        }

        public void DefineIdentifier(string identifierName, object value)
        {
            if (string.IsNullOrWhiteSpace(identifierName))
                throw new ArgumentException("Identifier name is required.", nameof(identifierName));

            ArgumentNullException.ThrowIfNull(value, nameof(value));

            definitions[identifierName] = value;
        }

        public void SetIdentifierValue(string identifierName, Func<object> valueProvider)
        {
            ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

            // Check that the identifier we want to assign to exists.
            LookupIdentifierValue(identifierName, out EvaluationEnvironment environment);

            environment.DefineIdentifier(identifierName, valueProvider());
        }

        public object LookupIdentifierValue(string identifierName)
            => LookupIdentifierValue(identifierName, out EvaluationEnvironment _);

        private object LookupIdentifierValue(string identifierName, out EvaluationEnvironment environment)
        {
            if (string.IsNullOrWhiteSpace(identifierName))
                throw new ArgumentException("Identifier name is required.", nameof(identifierName));

            for (var env = this; env != null; env = env.previousEnvironment)
            {
                if (env.definitions.TryGetValue(identifierName, out object? value))
                {
                    environment = env;
                    return value;
                }
            }

            throw new InvalidOperationException($"{identifierName} is undefined.");
        }
    }
}
