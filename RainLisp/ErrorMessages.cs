namespace RainLisp
{
    public static class ErrorMessages
    {
        public const string NON_TERMINATED_STRING = "The string is not terminated, line {0}, position {1}.";
        public const string INVALID_ESCAPE_SEQUENCE = "Invalid escape sequence \\{0}, line {1}, position {2}.";
        public const string INVALID_STRING_CHARACTER = "Invalid string character {0}, line {1}, position {2}.";
        public const string INVALID_NUMBER_CHARACTER = "Invalid number character {0}, line {1}, position {2}.";

        public const string PARSING_ERROR = "Wrong syntax, line {0}, position {1}.";

        public const string WRONG_NUMBER_OF_ARGUMENTS = "Wrong number of arguments, expecting {0} but got {1}.";
        public const string WRONG_NUMBER_OF_ARGUMENTS_EXT = "Wrong number of arguments, expecting {0} or more but got {1}.";
        public const string WRONG_TYPE_OF_ARGUMENT = "Wrong type of argument, expecting {0}, but got {1}.";
        public const string WRONG_TYPE_OF_ARGUMENT_FOR_MANY = "Wrong type of argument, expecting one of {0}, but got {1}.";
        public const string UNKNOWN_IDENTIFIER = "Unknown identifier {0}.";
        public const string NOT_PROCEDURE = "Not a procedure.";
        public const string USER_ERROR = "User error: {0}";
        public const string INVALID_VALUE = "Invalid value.";

        public const string UNKNOWN_ERROR = "Unknown error.";
    }
}
