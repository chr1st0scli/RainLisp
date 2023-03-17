namespace RainLisp.Grammar
{
    /// <summary>
    /// Characters that can be part of number literals, as defined in the lexical grammar of the language.
    /// </summary>
    public static class NumberSpecialChars
    {
        /// <summary>
        /// Positive sign for numeric literals.
        /// </summary>
        public const char PLUS = '+';

        /// <summary>
        /// Negative sign for numeric literals.
        /// </summary>
        public const char MINUS = '-';

        /// <summary>
        /// Decimal separator for numeric literals.
        /// </summary>
        public const char DOT = '.';
    }
}
