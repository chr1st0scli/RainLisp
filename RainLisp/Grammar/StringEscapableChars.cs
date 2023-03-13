namespace RainLisp.Grammar
{
    /// <summary>
    /// Characters that can be escaped in a string literal, as defined in the lexical grammar of the language.
    /// </summary>
    public static class StringEscapableChars
    {
        public const char ESCAPABLE_NEW_LINE = 'n';
        public const char ESCAPABLE_CARRIAGE_RETURN = 'r';
        public const char ESCAPABLE_TAB = 't';
        public const char ESCAPABLE_DOUBLE_QUOTE = '"';
        public const char ESCAPE = '\\';
    }
}
