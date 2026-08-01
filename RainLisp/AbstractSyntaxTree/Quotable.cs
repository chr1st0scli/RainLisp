namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// The quotable part of a quote expression in the abstract syntax tree.
    /// </summary>
    /// <remarks>An instance of this class either contains text or a list of other quotables.</remarks>
    /// <param name="text">The optional text of the quotable.</param>
    /// <param name="quotables">An optional list of other quotables.</param>
    public class Quotable(string? text, IList<Quotable>? quotables = null)
    {
        /// <summary>
        /// Gets or sets the optional text of the quotable.
        /// </summary>
        public string? Text { get; init; } = text;

        /// <summary>
        /// Gets or sets the optional list of other quotables.
        /// </summary>
        public IList<Quotable>? Quotables { get; init; } = quotables;
    }
}
