namespace RainLisp
{
    public class Environment
    {
        private readonly IDictionary<string, object> definitions;

        private Environment? previousEnvironment;
        private Environment? nextEnvironment;

        public static Environment GlobalEnvironment { get; } = new Environment();

        public Environment()
        {
            definitions = new Dictionary<string, object>();
        }

        // consider puting parameters and arguments as parameters for this method
        public Environment ExtendEnvironment()
        {
            nextEnvironment = new Environment { previousEnvironment = this };

            return nextEnvironment;
        }

        public void SetIdentifier(string identifierName, object value)
        {
            if (string.IsNullOrWhiteSpace(identifierName))
                throw new ArgumentException("Identifier name is required.", nameof(identifierName));

            ArgumentNullException.ThrowIfNull(value, nameof(value));

            definitions[identifierName] = value;
        }

        public object LookupIdentifier(string identifierName)
        {
            if (string.IsNullOrWhiteSpace(identifierName))
                throw new ArgumentException("Identifier name is required.", nameof(identifierName));

            for (var env = this; env != null; env = env.previousEnvironment)
            {
                if (env.definitions.TryGetValue(identifierName, out object? value))
                    return value;
            }

            throw new InvalidOperationException($"{identifierName} is undefined.");
        }
    }
}
