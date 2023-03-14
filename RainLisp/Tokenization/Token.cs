namespace RainLisp.Tokenization
{
    /// <summary>
    /// Represents the product of lexical analysis.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        public TokenType Type { get; init; }

        /// <summary>
        /// Gets or sets the sequence of characters that make up the token.
        /// </summary>
        public string Value { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the numeric value of the token in case it is a number.
        /// </summary>
        public double NumberValue { get; init; }

        /// <summary>
        /// Gets or sets the boolean value of the token in case it is a boolean.
        /// </summary>
        public bool BooleanValue { get; init; }

        /// <summary>
        /// Gets or sets the string value of the token in case it is a string.
        /// </summary>
        public string StringValue { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the line in the code which the token starts in.
        /// </summary>
        public uint Line { get; init; }

        /// <summary>
        /// Gets or sets the character position in the <see cref="Line"/> where the token starts at.
        /// </summary>
        public uint Position { get; init; }
    }
}
