namespace RainLisp.Environment
{
    public interface IEnvironmentFactory
    {
        IEvaluationEnvironment CreateEnvironment();
    }
}
