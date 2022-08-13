namespace RainLisp
{
    public class Environment
    {
        private readonly IDictionary<string, object> definitions;

        private Environment? previousEnvironment;
        private Environment? nextEnvironment;

        public static Environment GlobalEnvironment { get; private set; } = new Environment();

        public static void ResetGlobalEnvironment() => GlobalEnvironment = new Environment();

        public Environment()
        {
            definitions = new Dictionary<string, object>();
        }

        public Environment ExtendEnvironment(IList<string>? parameters, object[]? evaluatedArguments)
        {
            if (parameters?.Count != evaluatedArguments?.Length)
                throw new InvalidOperationException("Wrong number of arguments.");

            nextEnvironment = new Environment { previousEnvironment = this };

            if (parameters?.Count > 0 && evaluatedArguments?.Length > 0)
            {
                for (int i = 0; i < parameters.Count; i++)
                    nextEnvironment.DefineIdentifier(parameters[i], evaluatedArguments[i]);
            }

            return nextEnvironment;
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
            LookupIdentifierValue(identifierName, out Environment environment);

            environment.DefineIdentifier(identifierName, valueProvider());
        }

        public object LookupIdentifierValue(string identifierName)
            => LookupIdentifierValue(identifierName, out Environment _);

        private object LookupIdentifierValue(string identifierName, out Environment environment)
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
