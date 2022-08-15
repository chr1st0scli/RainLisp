namespace RainLisp.Environment
{
    public interface IEvaluationEnvironment
    {
        IEvaluationEnvironment ExtendEnvironment(IList<string>? parameters, object[]? evaluatedArguments);

        void DefineIdentifier(string identifierName, object value);

        void SetIdentifierValue(string identifierName, Func<object> valueProvider);

        object LookupIdentifierValue(string identifierName);
    }
}
