namespace RainLisp.Evaluation.Results
{
    /// <summary>
    /// Represents a quote symbol as a result of an evaluation.
    /// </summary>
    public class QuoteSymbol : EvaluationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteSymbol"/> class.
        /// </summary>
        /// <param name="symbolText">The text of the quote symbol.</param>
        public QuoteSymbol(string symbolText)
            => SymbolText = symbolText;

        /// <summary>
        /// Gets or sets the text of the quote symbol.
        /// </summary>
        public string SymbolText { get; init; }

        /// <summary>
        /// Accepts a visitor that performs some operation on the quote symbol and returns a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The result of the visitor's operation on the quote symbol.</typeparam>
        /// <param name="visitor">The visitor that performs an operation.</param>
        /// <returns>A <typeparamref name="T"/> as a result of the visitor's operation.</returns>
        public override T AcceptVisitor<T>(IEvaluationResultVisitor<T> visitor)
            => visitor.VisitQuoteSymbol(this);
    }
}
