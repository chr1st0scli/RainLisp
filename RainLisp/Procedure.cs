using RainLisp.AbstractSyntaxTree;

namespace RainLisp
{
    public class Procedure
    {
        public Procedure(IList<string>? parameters, Body body, Environment environment)
        {
            Parameters = parameters;
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IList<string>? Parameters { get; init; }

        public Body Body { get; init; }

        public Environment Environment { get; init; }
    }
}
