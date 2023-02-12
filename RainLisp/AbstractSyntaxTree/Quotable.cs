namespace RainLisp.AbstractSyntaxTree
{
    public class Quotable
    {
        public Quotable(string? text, IList<Quotable>? quotables = null)
        {
            Text = text;
            Quotables = quotables;
        }

        public string? Text { get; init; }

        public IList<Quotable>? Quotables { get; init; }
    }
}
