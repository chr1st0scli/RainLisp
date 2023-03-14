namespace RainLisp
{
    /// <summary>
    /// Debugging information.
    /// </summary>
    public interface IDebugInfo
    {
        /// <summary>
        /// Gets or sets the line in the source code.
        /// </summary>
        public uint Line { get; set; }

        /// <summary>
        /// Gets or sets the starting character position in the <see cref="Line"/>.
        /// </summary>
        public uint Position { get; set; }

        /// <summary>
        /// Gets or sets if relevant debugging info has been set.
        /// </summary>
        public bool HasDebugInfo { get; set; }
    }
}
