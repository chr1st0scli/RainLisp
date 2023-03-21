namespace RainLisp.Grammar
{
    /// <summary>
    /// Characters that can be combined with a starting backslash \ character to form a valid escape sequence in a string literal, as defined in the lexical grammar of the language.
    /// </summary>
    public static class StringEscapableChars
    {
        /// <summary>
        /// Denotes the new line character if escaped.
        /// </summary>
        public const char ESCAPABLE_NEW_LINE = 'n';

        /// <summary>
        /// Denotes the carriage return character if escaped.
        /// </summary>
        public const char ESCAPABLE_CARRIAGE_RETURN = 'r';

        /// <summary>
        /// Denotes the tab character if escaped.
        /// </summary>
        public const char ESCAPABLE_TAB = 't';

        /// <summary>
        /// Denotes the double quote character if escaped.
        /// </summary>
        public const char ESCAPABLE_DOUBLE_QUOTE = '"';

        /// <summary>
        /// Denotes the backslash character if escaped.
        /// </summary>
        public const char ESCAPE = '\\';
    }
}
