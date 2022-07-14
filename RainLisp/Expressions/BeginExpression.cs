namespace RainLisp.Expressions
{
    internal class BeginExpression : Expression
    {
        public Expression[] Actions { get; set; } = Array.Empty<Expression>();
    }
}
