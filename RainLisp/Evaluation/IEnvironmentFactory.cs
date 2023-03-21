namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents a factory for creating instances of <see cref="IEvaluationEnvironment"/>.
    /// </summary>
    public interface IEnvironmentFactory
    {
        /// <summary>
        /// Returns an evaluation environment.
        /// </summary>
        /// <returns>The new evaluation environment.</returns>
        IEvaluationEnvironment CreateEnvironment();
    }
}
