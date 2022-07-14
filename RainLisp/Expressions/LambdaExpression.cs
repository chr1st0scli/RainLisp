namespace RainLisp.Expressions
{
    internal class LambdaExpression : Expression
    {
        public string[] Parameters { get; set; } = Array.Empty<string>();

        public Expression Body { get; set; } = new Expression();
    }
}
