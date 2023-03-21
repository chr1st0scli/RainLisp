namespace RainLisp.Grammar
{
    /// <summary>
    /// Keywords, i.e. special forms and derived expressions, as defined in the lexical grammar of the language.
    /// </summary>
    public static class Keywords
    {
        /// <summary>
        /// Literal for the true boolean value.
        /// </summary>
        public const string TRUE = "true";

        /// <summary>
        /// Literal for the false boolean value.
        /// </summary>
        public const string FALSE = "false";

        /// <summary>
        /// Quote.
        /// </summary>
        public const string QUOTE = "quote";

        /// <summary>
        /// Assignment.
        /// </summary>
        public const string SET = "set!";

        /// <summary>
        /// Definition.
        /// </summary>
        public const string DEFINE = "define";

        /// <summary>
        /// If special form.
        /// </summary>
        public const string IF = "if";

        /// <summary>
        /// Conditional.
        /// </summary>
        public const string COND = "cond";

        /// <summary>
        /// Conditional's else.
        /// </summary>
        public const string ELSE = "else";

        /// <summary>
        /// Begin code block.
        /// </summary>
        public const string BEGIN = "begin";

        /// <summary>
        /// Lambda.
        /// </summary>
        public const string LAMBDA = "lambda";

        /// <summary>
        /// Logical and.
        /// </summary>
        public const string AND = "and";

        /// <summary>
        /// Logical or.
        /// </summary>
        public const string OR = "or";

        /// <summary>
        /// Let expression.
        /// </summary>
        public const string LET = "let";
    }
}
