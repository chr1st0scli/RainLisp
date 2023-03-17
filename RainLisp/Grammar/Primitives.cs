namespace RainLisp.Grammar
{
    /// <summary>
    /// Primitive procedures of the language.
    /// </summary>
    public static class Primitives
    {
        /// <summary>
        /// Numerical addition or string concatenation.
        /// </summary>
        public const string PLUS = "+";

        /// <summary>
        /// Numerical subtraction.
        /// </summary>
        public const string MINUS = "-";

        /// <summary>
        /// Numerical division.
        /// </summary>
        public const string DIVIDE = "/";

        /// <summary>
        /// Numerical modulo.
        /// </summary>
        public const string MODULO = "%";

        /// <summary>
        /// Numerical multiplication.
        /// </summary>
        public const string MULTIPLY = "*";

        /// <summary>
        /// Numerical or date and time, greater than operator.
        /// </summary>
        public const string GREATER = ">";

        /// <summary>
        /// Numerical or date and time, greater than or equal to operator.
        /// </summary>
        public const string GREATER_OR_EQUAL = ">=";

        /// <summary>
        /// Numerical or date and time, less than operator.
        /// </summary>
        public const string LESS = "<";

        /// <summary>
        /// Numerical or date and time, less than or equal to operator.
        /// </summary>
        public const string LESS_OR_EQUAL = "<=";

        /// <summary>
        /// Equality check.
        /// </summary>
        public const string EQUAL = "=";

        /// <summary>
        /// Logical xor.
        /// </summary>
        public const string XOR = "xor";

        /// <summary>
        /// Logical not.
        /// </summary>
        public const string NOT = "not";

        /// <summary>
        /// Pair construction.
        /// </summary>
        public const string CONS = "cons";

        /// <summary>
        /// Pair's first element accessor.
        /// </summary>
        public const string CAR = "car";

        /// <summary>
        /// Pair's second element accessor.
        /// </summary>
        public const string CDR = "cdr";

        /// <summary>
        /// Checks for pair.
        /// </summary>
        public const string IS_PAIR = "pair?";

        /// <summary>
        /// The designated nil construct that marks the end of a list.
        /// </summary>
        public const string NIL = "nil";

        /// <summary>
        /// List construction.
        /// </summary>
        public const string LIST = "list";

        /// <summary>
        /// Checks for nil, also known as empty list.
        /// </summary>
        public const string IS_NULL = "null?";

        /// <summary>
        /// Mutates a pair's first element.
        /// </summary>
        public const string SET_CAR = "set-car!";

        /// <summary>
        /// Mutates a pair's second element.
        /// </summary>
        public const string SET_CDR = "set-cdr!";

        /// <summary>
        /// Returns a string's length.
        /// </summary>
        public const string STRING_LENGTH = "string-length";

        /// <summary>
        /// Returns a substring of a given string.
        /// </summary>
        public const string SUBSTRING = "substring";

        /// <summary>
        /// Returns the index of a string occurence within a given string.
        /// </summary>
        public const string INDEX_OF_STRING = "index-of-string";

        /// <summary>
        /// Returns a new string by replacing a substring within a given string.
        /// </summary>
        public const string REPLACE_STRING = "replace-string";

        /// <summary>
        /// Returns a new string by turning a given one to lower case.
        /// </summary>
        public const string TO_LOWER = "to-lower";

        /// <summary>
        /// Returns a new string by turning a given one to upper case.
        /// </summary>
        public const string TO_UPPER = "to-upper";

        /// <summary>
        /// Writes a primitive value to the standard output.
        /// </summary>
        public const string DISPLAY = "display";

        /// <summary>
        /// Writes a primitive value to the trace listeners in the debug listeners collection.
        /// </summary>
        public const string DEBUG = "debug";

        /// <summary>
        /// Writes a primitive value to the trace listeners in the trace listeners collection.
        /// </summary>
        public const string TRACE = "trace";

        /// <summary>
        /// Writes a new line character to the standard output.
        /// </summary>
        public const string NEW_LINE = "newline";

        /// <summary>
        /// Causes a user exception with a primitive value to be thrown.
        /// </summary>
        public const string ERROR = "error";

        /// <summary>
        /// Returns the current local date and time.
        /// </summary>
        public const string NOW = "now";

        /// <summary>
        /// Returns the current date and time, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public const string UTC_NOW = "utc-now";

        /// <summary>
        /// Date and time construction with year, month and day.
        /// </summary>
        public const string MAKE_DATE = "make-date";

        /// <summary>
        /// 
        /// </summary>
        public const string MAKE_DATETIME = "make-datetime";

        /// <summary>
        /// 
        /// </summary>
        public const string YEAR = "year";

        /// <summary>
        /// 
        /// </summary>
        public const string MONTH = "month";

        /// <summary>
        /// 
        /// </summary>
        public const string DAY = "day";

        /// <summary>
        /// 
        /// </summary>
        public const string HOUR = "hour";

        /// <summary>
        /// 
        /// </summary>
        public const string MINUTE = "minute";

        /// <summary>
        /// 
        /// </summary>
        public const string SECOND = "second";

        /// <summary>
        /// 
        /// </summary>
        public const string MILLISECOND = "millisecond";

        /// <summary>
        /// 
        /// </summary>
        public const string IS_UTC = "utc?";

        /// <summary>
        /// 
        /// </summary>
        public const string TO_LOCAL = "to-local";

        /// <summary>
        /// 
        /// </summary>
        public const string TO_UTC = "to-utc";

        /// <summary>
        /// 
        /// </summary>
        public const string ADD_YEARS = "add-years";

        /// <summary>
        /// 
        /// </summary>
        public const string ADD_MONTHS = "add-months";

        /// <summary>
        /// 
        /// </summary>
        public const string ADD_DAYS = "add-days";

        /// <summary>
        /// 
        /// </summary>
        public const string ADD_HOURS = "add-hours";

        /// <summary>
        /// 
        /// </summary>
        public const string ADD_MINUTES = "add-minutes";

        /// <summary>
        /// 
        /// </summary>
        public const string ADD_SECONDS = "add-seconds";

        /// <summary>
        /// 
        /// </summary>
        public const string ADD_MILLISECONDS = "add-milliseconds";

        /// <summary>
        /// 
        /// </summary>
        public const string DAYS_DIFF = "days-diff";

        /// <summary>
        /// 
        /// </summary>
        public const string HOURS_DIFF = "hours-diff";

        /// <summary>
        /// 
        /// </summary>
        public const string MINUTES_DIFF = "minutes-diff";

        /// <summary>
        /// 
        /// </summary>
        public const string SECONDS_DIFF = "seconds-diff";

        /// <summary>
        /// 
        /// </summary>
        public const string MILLISECONDS_DIFF = "milliseconds-diff";

        /// <summary>
        /// 
        /// </summary>
        public const string PARSE_DATETIME = "parse-datetime";

        /// <summary>
        /// 
        /// </summary>
        public const string DATETIME_TO_STRING = "datetime-to-string";

        /// <summary>
        /// 
        /// </summary>
        public const string NUMBER_TO_STRING = "number-to-string";

        /// <summary>
        /// 
        /// </summary>
        public const string PARSE_NUMBER = "parse-number";

        /// <summary>
        /// 
        /// </summary>
        public const string ROUND = "round";

        /// <summary>
        /// 
        /// </summary>
        public const string EVAL = "eval";
    }
}
