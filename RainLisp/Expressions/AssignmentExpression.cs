namespace RainLisp.Expressions
{
    internal class AssignmentExpression : Expression
    {
        public string VariableName { get; set; } = string.Empty;

        public Expression Value { get; set; } = new Expression();
    }
}
