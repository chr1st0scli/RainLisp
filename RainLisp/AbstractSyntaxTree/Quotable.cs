namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// The quotable part of a quote expression in the abstract syntax tree.
    /// </summary>
    public class Quotable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Quotable"/> class that either contains text or a list of other quotables.
        /// </summary>
        /// <param name="text">The optional text of the quotable.</param>
        /// <param name="quotables">An optional list of other quotables.</param>
        public Quotable(string? text, IList<Quotable>? quotables = null)
        {
            Text = text;
            Quotables = quotables;
        }

        /// <summary>
        /// Gets or sets the optional text of the quotable.
        /// </summary>
        public string? Text { get; init; }

        /// <summary>
        /// Gets or sets the optional list of other quotables.
        /// </summary>
        public IList<Quotable>? Quotables { get; init; }
    }
}
