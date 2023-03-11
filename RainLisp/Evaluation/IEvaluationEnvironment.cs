using RainLisp.Evaluation.Results;
using System.Diagnostics.CodeAnalysis;

namespace RainLisp.Evaluation
{
    public interface IEvaluationEnvironment
    {
        IEvaluationEnvironment ExtendEnvironment(IList<string>? parameters, EvaluationResult[]? evaluatedArguments);

        void DefineIdentifier(string identifierName, EvaluationResult value);

        void SetIdentifierValue(string identifierName, Func<EvaluationResult> valueProvider);

        EvaluationResult LookupIdentifierValue(string identifierName);

        string[] GetIdentifierNames();

        void RegisterQuoteSymbol(QuoteSymbol symbol);

        bool TryGetQuoteSymbol(string symbolText, [MaybeNullWhen(false)] out QuoteSymbol quoteSymbol);
    }
}
