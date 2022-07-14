namespace RainLisp.Expressions
{
    internal class QuotedExpression : Expression
    {
        public string QuoteText { get; set; } = string.Empty;
    }
}
