namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Expression in the abstract syntax tree.
    /// </summary>
    public abstract class Expression : Node, IDebugInfo
    {
        /// <summary>
        /// Gets or sets the expression's line in the source code.
        /// </summary>
        public uint Line { get; set; }

        /// <summary>
        /// Gets or sets the expression's starting character position on the <see cref="Line"/> in the source code.
        /// </summary>
        public uint Position { get; set; }

        /// <summary>
        /// Gets or sets if relevant debugging info has been set.
        /// </summary>
        public bool HasDebugInfo { get; set; }
    }
}
