namespace RainLisp.Expressions
{
    internal class IfExpression : Expression
    {
        public Expression Predicate { get; set; } = new Expression();

        public Expression Consequent { get; set; } = new Expression();

        public Expression Alternative { get; set; } = new Expression();
    }
}
