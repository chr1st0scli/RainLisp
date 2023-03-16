using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    /// <summary>
    /// Represents an exception that may occur during the syntax analysis of code.
    /// </summary>
    public class ParsingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the syntax problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the syntax problem refers to.</param>
        /// <param name="missingSymbols">The missing symbols that were expected at the given <paramref name="position"/>.</param>
        public ParsingException(uint line, uint position, TokenType[] missingSymbols)
        {
            Line = line;
            Position = position;
            MissingSymbols = missingSymbols;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the syntax problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the syntax problem refers to.</param>
        /// <param name="missingSymbols">The missing symbols that were expected at the given <paramref name="position"/>.</param>
        /// <param name="message">The message that describes the error.</param>
        public ParsingException(uint line, uint position, TokenType[] missingSymbols, string? message) : base(message)
        {
            Line = line;
            Position = position;
            MissingSymbols = missingSymbols;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the syntax problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the syntax problem refers to.</param>
        /// <param name="missingSymbols">The missing symbols that were expected at the given <paramref name="position"/>.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ParsingException(uint line, uint position, TokenType[] missingSymbols, string? message, Exception? innerException) : base(message, innerException)
        {
            Line = line;
            Position = position;
            MissingSymbols = missingSymbols;
        }

        /// <summary>
        /// Gets or sets the line in the code which the syntax problem refers to.
        /// </summary>
        public uint Line { get; init; }

        /// <summary>
        /// Gets or sets the character position in the <see cref="Line"/> where the syntax problem refers to.
        /// </summary>
        public uint Position { get; init; }

        /// <summary>
        /// Gets or sets the missing symbols that were expected at the given <see cref="Position"/>.
        /// </summary>
        public TokenType[] MissingSymbols { get; init; }
    }
}
