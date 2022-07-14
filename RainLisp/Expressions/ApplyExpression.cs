namespace RainLisp.Expressions
{
    internal class ApplyExpression : Expression
    {
        public SymbolExpression Operator { get; set; } = new SymbolExpression();

        public Expression[] Operands { get; set; } = Array.Empty<Expression>();
    }
}
