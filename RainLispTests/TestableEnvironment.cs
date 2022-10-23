using RainLisp.Environment;
using RainLisp.Evaluation.Results;

namespace RainLispTests
{
    /// <summary>
    /// This environment simply saves next environments when a procedure is called,
    /// so that the complete environment structure can be checked by the tests.
    /// </summary>
    internal class TestableEnvironment : IEvaluationEnvironment
    {
        private IEvaluationEnvironment actualEnvironment;
        private readonly IList<IEvaluationEnvironment> nextEnvironments;

        public TestableEnvironment()
        {
            actualEnvironment = new EvaluationEnvironment();
            nextEnvironments = new List<IEvaluationEnvironment>();
        }

        public void DefineIdentifier(string identifierName, EvaluationResult value)
            => actualEnvironment.DefineIdentifier(identifierName, value);

        public IEvaluationEnvironment ExtendEnvironment(IList<string>? parameters, EvaluationResult[]? evaluatedArguments)
        {
            var extendedEnvironment = new TestableEnvironment
            {
                actualEnvironment = actualEnvironment.ExtendEnvironment(parameters, evaluatedArguments)
            };

            nextEnvironments.Add(extendedEnvironment);

            return extendedEnvironment;
        }

        public EvaluationResult LookupIdentifierValue(string identifierName)
            => actualEnvironment.LookupIdentifierValue(identifierName);

        public void SetIdentifierValue(string identifierName, Func<EvaluationResult> valueProvider)
            => actualEnvironment.SetIdentifierValue(identifierName, valueProvider);
    }
}
