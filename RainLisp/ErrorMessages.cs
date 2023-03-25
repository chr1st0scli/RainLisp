namespace RainLisp
{
    /// <summary>
    /// Messages for errors that may occur during code interpretation.
    /// </summary>
    internal static class ErrorMessages
    {
        internal const string CALL_STACK = "Call Stack";
        internal const string DEBUG_INFO = "[{0}] Line {1}, position {2}.";

        internal const string NON_TERMINATED_STRING = "The string is not terminated. Line {0}, position {1}.";
        internal const string INVALID_ESCAPE_SEQUENCE = "Invalid escape sequence \\{0}. Line {1}, position {2}.";
        internal const string INVALID_STRING_CHARACTER = "Invalid string character {0}. Line {1}, position {2}.";
        internal const string INVALID_NUMBER_CHARACTER = "Invalid number character {0}. Line {1}, position {2}.";

        internal const string PARSING_ERROR = "Syntax error. Line {0}, position {1}. Expecting {2}.";
        internal const string SYMBOL_SEPARATOR = " or ";

        internal const string WRONG_NUMBER_OF_ARGUMENTS = "Wrong number of arguments, expecting {0} but got {1}.";
        internal const string WRONG_NUMBER_OF_ARGUMENTS_EXT = "Wrong number of arguments, expecting {0} or more but got {1}.";
        internal const string WRONG_TYPE_OF_ARGUMENT = "Wrong type of argument, expecting {0}, but got {1}.";
        internal const string WRONG_TYPE_OF_ARGUMENT_FOR_MANY = "Wrong type of argument, expecting one of {0}, but got {1}.";
        internal const string UNKNOWN_IDENTIFIER = "Unknown identifier {0}.";
        internal const string NOT_PROCEDURE = "Not a procedure.";
        internal const string USER_ERROR = "User error: {0}";
        internal const string INVALID_VALUE = "Invalid value.";

        internal const string UNKNOWN_ERROR = "Unknown error.";
    }
}
