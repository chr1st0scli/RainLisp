using RainLisp.Evaluation;

namespace RainLispTests
{
    internal class TestableEnvironmentFactory : IEnvironmentFactory
    {
        public IEvaluationEnvironment CreateEnvironment()
            => new TestableEnvironment();
    }
}
