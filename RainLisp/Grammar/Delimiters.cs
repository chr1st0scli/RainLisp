namespace RainLisp.Grammar
{
    /// <summary>
    /// Delimeters as defined in the lexical grammar of the language.
    /// </summary>
    public static class Delimiters
    {
        /// <summary>
        /// Opening parenthesis.
        /// </summary>
        public const char LPAREN = '(';

        /// <summary>
        /// Closing parenthesis.
        /// </summary>
        public const char RPAREN = ')';

        /// <summary>
        /// Space.
        /// </summary>
        public const char SPACE = ' ';

        /// <summary>
        /// Carriage return.
        /// </summary>
        public const char CARRIAGE_RETURN = '\r';

        /// <summary>
        /// New line.
        /// </summary>
        public const char NEW_LINE = '\n';

        /// <summary>
        /// Tab.
        /// </summary>
        public const char TAB = '\t';

        /// <summary>
        /// Double quote.
        /// </summary>
        public const char DOUBLE_QUOTE = '"';

        /// <summary>
        /// Single quote.
        /// </summary>
        public const char SINGLE_QUOTE = '\'';

        /// <summary>
        /// Start of inline comment.
        /// </summary>
        public const char COMMENT = ';';
    }
}
