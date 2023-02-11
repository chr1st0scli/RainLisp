namespace RainLisp.Evaluation.Results
{
    public class QuoteSymbol : EvaluationResult
    {
        public QuoteSymbol(string symbolText)
            => SymbolText = symbolText;

        public string SymbolText { get; init; }

        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitQuoteSymbol(this);
    }
}
