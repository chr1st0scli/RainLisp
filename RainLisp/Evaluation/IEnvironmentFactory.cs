namespace RainLisp.Evaluation
{
    public interface IEnvironmentFactory
    {
        IEvaluationEnvironment CreateEnvironment();
    }
}
