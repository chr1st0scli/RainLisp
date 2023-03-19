using RainLisp.Evaluation.Results;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Implementation of primitive procedures and constructs of the language.
    /// </summary>
    public static class PrimitiveOperation
    {
        /// <summary>
        /// Returns the sum of numeric or the concatenation of string values. It accepts two or more values.
        /// </summary>
        /// <param name="values">The numbers to add or strings to concatenate.</param>
        /// <returns>The sum of numeric or the concatenation of string values.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two or more.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers or strings.</exception>
        public static EvaluationResult Add(EvaluationResult[]? values)
            => ApplyMultivalueOperatorOnEitherPrimitive(AsDouble, AsString,
                (val1, val2) => val1 + val2,
                (val1, val2) => val1 + val2,
                result => new NumberDatum(result),
                result => new StringDatum(result),
                values);

        /// <summary>
        /// Returns the result of subtracting two or more numeric values. The subtraction accumulates from left to right.
        /// </summary>
        /// <param name="values">The numeric values to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two or more.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers.</exception>
        public static EvaluationResult Subtract(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 - val2, result => new NumberDatum(result), values);

        /// <summary>
        /// Returns the result of multiplying two or more numeric values.
        /// </summary>
        /// <param name="values">The numeric values to multiply.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two or more.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers.</exception>
        public static EvaluationResult Multiply(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 * val2, result => new NumberDatum(result), values);

        /// <summary>
        /// Returns the result of dividing two or more numeric values. The division accumulates from left to right.
        /// </summary>
        /// <param name="values">The numeric values to divide.</param>
        /// <returns>The result of the division.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two or more.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers.</exception>
        public static EvaluationResult Divide(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 / val2, result => new NumberDatum(result), values);

        /// <summary>
        /// Returns the result of calculating the modulo of two or more numeric values. The operation accumulates from left to right.
        /// </summary>
        /// <param name="values">The numeric values to calculate their modulo.</param>
        /// <returns>The result of the modulo calculation.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two or more.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers.</exception>
        public static EvaluationResult Modulo(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 % val2, result => new NumberDatum(result), values);

        /// <summary>
        /// Determines if the first numeric value is greater than the second one, or the first datetime is later than the second one.
        /// </summary>
        /// <param name="values">The numeric or datetime values to compare.</param>
        /// <returns>true if the first value is greater or later than the second one; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers or datetimes.</exception>
        public static EvaluationResult GreaterThan(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value > val2.Value),
                (val1, val2) => new BoolDatum(val1.Value > val2.Value), values);

        /// <summary>
        /// Determines if the first numeric value is greater than or equal to the second one, or the first datetime is the same as or later than the second one.
        /// </summary>
        /// <param name="values">The numeric or datetime values to compare.</param>
        /// <returns>true if the first value is equal to or greater/later than the second one; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers or datetimes.</exception>
        public static EvaluationResult GreaterThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value >= val2.Value),
                (val1, val2) => new BoolDatum(val1.Value >= val2.Value), values);

        /// <summary>
        /// Determines if the first numeric value is less than the second one, or the first datetime is earlier than the second one.
        /// </summary>
        /// <param name="values">The numeric or datetime values to compare.</param>
        /// <returns>true if the first value is less or earlier than the second one; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers or datetimes.</exception>
        public static EvaluationResult LessThan(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value < val2.Value),
                (val1, val2) => new BoolDatum(val1.Value < val2.Value), values);

        /// <summary>
        /// Determines if the first numeric value is less than or equal to the second one, or the first datetime is the same as or earlier than the second one.
        /// </summary>
        /// <param name="values">The numeric or datetime values to compare.</param>
        /// <returns>true if the first value is equal to or less/earlier than the second one; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given arguments are not all numbers or datetimes.</exception>
        public static EvaluationResult LessThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value <= val2.Value),
                (val1, val2) => new BoolDatum(val1.Value <= val2.Value), values);

        /// <summary>
        /// Determines if two values are equal. Primitive values like numbers, strings, booleans and datetimes are compared by value. All others are compared by reference.
        /// </summary>
        /// <param name="values">The values to compare.</param>
        /// <returns>true if the two values are equal; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        public static EvaluationResult EqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator((val1, val2) => new BoolDatum(val1.Equals(val2)), values);

        /// <summary>
        /// Returns the result of calculating the logical xor of two or more values. The operation accumulates from left to right.
        /// </summary>
        /// <param name="values">The values to calculate their logical xor.</param>
        /// <returns>The result of the logical xor calculation.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two or more.</exception>
        public static EvaluationResult LogicalXor(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsBool, (val1, val2) => val1 ^ val2, result => new BoolDatum(result), values);

        /// <summary>
        /// Returns the logical negation of a value. Every value is true except false itself.
        /// </summary>
        /// <param name="values">The value to calculate its logical negation.</param>
        /// <returns>true if the given value is false; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        public static EvaluationResult LogicalNot(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsBool, val => new BoolDatum(!val), values);

        /// <summary>
        /// Returns a pair made of the two given values.
        /// </summary>
        /// <param name="values">The two values to make the pair from.</param>
        /// <returns>A new pair.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        public static EvaluationResult Cons(EvaluationResult[]? values)
            => ApplyBinaryOperator((val1, val2) => new Pair(val1, val2), values);

        /// <summary>
        /// Returns the first element of a pair.
        /// </summary>
        /// <param name="values">A pair.</param>
        /// <returns>The first element of the pair.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given argument is not a pair.</exception>
        public static EvaluationResult Car(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsPair, val => val.First, values);

        /// <summary>
        /// Returns the second element of a pair.
        /// </summary>
        /// <param name="values">A pair.</param>
        /// <returns>The second element of the pair.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The given argument is not a pair.</exception>
        public static EvaluationResult Cdr(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsPair, val => val.Second, values);

        /// <summary>
        /// Determines if the given value is a pair.
        /// </summary>
        /// <param name="values">The value to check.</param>
        /// <returns>true if the value is a pair; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        public static EvaluationResult IsPair(EvaluationResult[]? values)
            => ApplyUnaryOperator(val => new BoolDatum(val is Pair), values);

        /// <summary>
        /// Returns a new list made of the values provided, or nil if none is given.
        /// </summary>
        /// <param name="values">The values the list will consist of.</param>
        /// <returns>A new list made of the values provided, or nil if none is given.</returns>
        public static EvaluationResult List(EvaluationResult[]? values)
        {
            if (values == null || values.Length == 0)
                return Nil.GetNil();

            return ApplyFoldRightOperator((val1, val2) => new Pair(val1, val2), Nil.GetNil(), values);
        }

        /// <summary>
        /// Determines if the given value is nil, i.e. an empty list.
        /// </summary>
        /// <param name="values">The value to check.</param>
        /// <returns>true if the given value is nil; otherwise, false;</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        public static EvaluationResult IsNull(EvaluationResult[]? values)
            => ApplyUnaryOperator(val => new BoolDatum(val == Nil.GetNil()), values);

        /// <summary>
        /// Sets the first part of a pair to the value provided.
        /// </summary>
        /// <param name="values">A pair and a value to set its first part to.</param>
        /// <returns>The unspecified result.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a pair.</exception>
        public static EvaluationResult SetCar(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsPair, (Pair val1, EvaluationResult val2) =>
            {
                val1.First = val2;
                return Unspecified.GetUnspecified();
            }, values);

        /// <summary>
        /// Sets the second part of a pair to the value provided.
        /// </summary>
        /// <param name="values">A pair and a value to set its second part to.</param>
        /// <returns>The unspecified result.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a pair.</exception>
        public static EvaluationResult SetCdr(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsPair, (Pair val1, EvaluationResult val2) =>
            {
                val1.Second = val2;
                return Unspecified.GetUnspecified();
            }, values);

        /// <summary>
        /// Returns the length of a given string.
        /// </summary>
        /// <param name="values">A string value.</param>
        /// <returns>The length of the given string.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a string value.</exception>
        public static EvaluationResult StringLength(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => new NumberDatum(val.Length), values);

        /// <summary>
        /// Returns a substring value of a given string. The substring starts at a specified character position and has a specified length.
        /// </summary>
        /// <param name="values">The string to get a substring from, the zero-based start index and the length of the substring.</param>
        /// <returns>A substring starting at the specified character position and with the given length, or an empty string if the start index is equal to the length of the string and the desired length is zero.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not three.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a string value or the other two are not all numeric ones.</exception>
        public static EvaluationResult Substring(EvaluationResult[]? values)
            => ApplyTripleOperator(AsString, AsDouble, AsDouble,
                (value, startIndex, length) => ValueOrThrowInvalid(() => new StringDatum(value.Substring((int)startIndex, (int)length))),
                values);

        /// <summary>
        /// Returns the zero-based index of the first occurence of a string withing another string. The search starts at a specified character position.
        /// </summary>
        /// <param name="values">The string to search in, the string to look for and the search starting position.</param>
        /// <returns>The zero-based index of the first occurence of a string withing another string if it is found, or -1 if it is not. If the string to look for is empty, the return value is the start index.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not three.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first two arguments are not all string values or the third one is not a numeric one.</exception>
        public static EvaluationResult IndexOfString(EvaluationResult[]? values)
            => ApplyTripleOperator(AsString, AsString, AsDouble,
                (value, searchValue, startIndex) => ValueOrThrowInvalid(() => new NumberDatum(value.IndexOf(searchValue, (int)startIndex))),
                values);

        /// <summary>
        /// Returns a new string in which all occurrences of a substring within a given string are replaced by another one.
        /// </summary>
        /// <param name="values">The string to search in, the string to be replaced and the string to replace all occurrences of the old value.</param>
        /// <returns>A new string in which all occurrences of a substring within a given string are replaced by another one. If the string to be replaced is not found, the original string is returned unchanged.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not three.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are string values.</exception>
        public static EvaluationResult ReplaceString(EvaluationResult[]? values)
            => ApplyTripleOperator(AsString, AsString, AsString,
                (value, oldValue, newValue) => ValueOrThrowInvalid(() => new StringDatum(value.Replace(oldValue, newValue))),
                values);

        /// <summary>
        /// Returns a copy of a string value converted to lower case using the casing rules of the invariant culture.
        /// </summary>
        /// <param name="values">The string value to convert.</param>
        /// <returns>A copy of the string value converted to lower case.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a string value.</exception>
        public static EvaluationResult ToLower(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => new StringDatum(val.ToLowerInvariant()), values);

        /// <summary>
        /// Returns a copy of a string value converted to upper case using the casing rules of the invariant culture.
        /// </summary>
        /// <param name="values">The string value to convert.</param>
        /// <returns>A copy of the string value converted to upper case.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a string value.</exception>
        public static EvaluationResult ToUpper(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => new StringDatum(val.ToUpperInvariant()), values);

        /// <summary>
        /// Writes a primitive value to the standard output. The format of the output is determined by the local culture.
        /// </summary>
        /// <param name="values">A primitive value such as a boolean, a number, a string or a datetime.</param>
        /// <returns>The unspecified result.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a primitive value.</exception>
        public static EvaluationResult Display(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                Console.Write(ToCurrentCultureString(val));
                return Unspecified.GetUnspecified();
            }, values);

        /// <summary>
        /// Writes a primitive value to the trace listeners in the debug listeners collection. The format of the output is determined by the local culture.
        /// </summary>
        /// <param name="values">A primitive value such as a boolean, a number, a string or a datetime.</param>
        /// <returns>The unspecified result.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a primitive value.</exception>
        public static EvaluationResult Debug(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                System.Diagnostics.Debug.Write(ToCurrentCultureString(val));
                return Unspecified.GetUnspecified();
            }, values);

        /// <summary>
        /// Writes a primitive value to the trace listeners in the trace listeners collection. The format of the output is determined by the local culture.
        /// </summary>
        /// <param name="values">A primitive value such as a boolean, a number, a string or a datetime.</param>
        /// <returns>The unspecified result.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a primitive value.</exception>
        public static EvaluationResult Trace(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                System.Diagnostics.Trace.Write(ToCurrentCultureString(val));
                return Unspecified.GetUnspecified();
            }, values);

        /// <summary>
        /// Writes a new line to the standard output.
        /// </summary>
        /// <param name="values">No arguments are expected.</param>
        /// <returns>The unspecified result.</returns>
        /// <exception cref="WrongNumberOfArgumentsException"><paramref name="values"/> is neither null nor empty.</exception>
        public static EvaluationResult NewLine(EvaluationResult[]? values)
        {
            RequireZero(values);
            Console.WriteLine();
            return Unspecified.GetUnspecified();
        }

        /// <summary>
        /// Causes a user exception carrying a primitive value to be thrown.
        /// A number primitive value is formatted using the invariant culture but all other primitives use the local culture.
        /// </summary>
        /// <param name="values">A primitive value such as a boolean, a number, a string or a datetime.</param>
        /// <returns>Not applicable.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a primitive value.</exception>
        /// <exception cref="UserException">Always throws a user exception.</exception>
        public static EvaluationResult Error(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                string? message;
                if (val is double doubleVal)
                    message = doubleVal.ToString(CultureInfo.InvariantCulture);
                else
                    message = ToCurrentCultureString(val);

                throw new UserException(message);
            }, values);

        /// <summary>
        /// Returns the current date and time on this computer, expressed as the local time.
        /// </summary>
        /// <param name="values">No arguments are expected.</param>
        /// <returns>The current local date and time.</returns>
        /// <exception cref="WrongNumberOfArgumentsException"><paramref name="values"/> is neither null nor empty.</exception>
        public static EvaluationResult Now(EvaluationResult[]? values)
        {
            RequireZero(values);
            return new DateTimeDatum(DateTime.Now);
        }

        /// <summary>
        /// Returns the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        /// <param name="values">No arguments are expected.</param>
        /// <returns>The current UTC date and time.</returns>
        /// <exception cref="WrongNumberOfArgumentsException"><paramref name="values"/> is neither null nor empty.</exception>
        public static EvaluationResult UtcNow(EvaluationResult[]? values)
        {
            RequireZero(values);
            return new DateTimeDatum(DateTime.UtcNow);
        }

        /// <summary>
        /// Returns a new datetime value in an unspecified time zone, made of a year, month and day of the month.
        /// </summary>
        /// <param name="values">The year (1 through 9999), the month (1 through 12) and the day (1 through the number of days in month).</param>
        /// <returns>A datetime value.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not three.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are numeric values.</exception>
        public static EvaluationResult MakeDate(EvaluationResult[]? values)
            => ApplyTripleOperator(AsDouble, AsDouble, AsDouble,
                (year, month, day) => ValueOrThrowInvalid(() => new DateTimeDatum(new DateTime((int)year, (int)month, (int)day))),
                values);

        /// <summary>
        /// Returns a new datetime value in an unspecified time zone, made of a year, month, day of the month, hour, minute, second and millisecond.
        /// </summary>
        /// <param name="values">
        /// The year (1 through 9999), the month (1 through 12), the day (1 through the number of days in month),
        /// The hours (0 through 23), the minutes (0 through 59), the seconds (0 through 59) and the milliseconds (0 through 999).
        /// </param>
        /// <returns>A datetime value.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not seven.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are numeric values.</exception>
        public static EvaluationResult MakeDateTime(EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 7);
            int year = (int)AsDouble(values[0]);
            int month = (int)AsDouble(values[1]);
            int day = (int)AsDouble(values[2]);
            int hour = (int)AsDouble(values[3]);
            int minute = (int)AsDouble(values[4]);
            int second = (int)AsDouble(values[5]);
            int millisecond = (int)AsDouble(values[6]);

            return ValueOrThrowInvalid(() => new DateTimeDatum(new DateTime(year, month, day, hour, minute, second, millisecond)));
        }

        /// <summary>
        /// Returns the year of a given datetime value.
        /// </summary>
        /// <param name="values">The datetime to get the year from.</param>
        /// <returns>The year, expressed as a value between 1 and 9999.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult Year(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Year), values);

        /// <summary>
        /// Returns the month of a given datetime value.
        /// </summary>
        /// <param name="values">The datetime to get the month from.</param>
        /// <returns>The month, expressed as a value between 1 and 12.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult Month(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Month), values);

        /// <summary>
        /// Returns the day of the month of a given datetime value.
        /// </summary>
        /// <param name="values">The datetime to get the day from.</param>
        /// <returns>The day, expressed as a value between 1 and 31.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult Day(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Day), values);

        /// <summary>
        /// Returns the hour of a given datetime value.
        /// </summary>
        /// <param name="values">The datetime to get the hour from.</param>
        /// <returns>The hour, expressed as a value between 0 and 23.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult Hour(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Hour), values);

        /// <summary>
        /// Returns the minute of a given datetime value.
        /// </summary>
        /// <param name="values">The datetime to get the minute from.</param>
        /// <returns>The minute, expressed as a value between 0 and 59.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult Minute(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Minute), values);

        /// <summary>
        /// Returns the second of a given datetime value.
        /// </summary>
        /// <param name="values">The datetime to get the second from.</param>
        /// <returns>The second, expressed as a value between 0 and 59.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult Second(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Second), values);

        /// <summary>
        /// Returns the millisecond of a given datetime value.
        /// </summary>
        /// <param name="values">The datetime to get the millisecond from.</param>
        /// <returns>The millisecond, expressed as a value between 0 and 999.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult Millisecond(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Millisecond), values);

        /// <summary>
        /// Determines if the given datetime is Coordinated Universal Time (UTC).
        /// </summary>
        /// <param name="values">The datetime that is checked.</param>
        /// <returns>true if the given datetime is UTC; otherwise, false.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult IsUtc(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new BoolDatum(val.Kind == DateTimeKind.Utc), values);

        /// <summary>
        /// Converts a datetime value to local.
        /// </summary>
        /// <param name="values">The datetime value to convert. It must be a UTC or unspecified datetime.</param>
        /// <returns>The datetime value expressed in local time.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult ToLocal(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => ValueOrThrowInvalid(() => new DateTimeDatum(TimeZoneInfo.ConvertTime(val, TimeZoneInfo.Utc, TimeZoneInfo.Local))), values);

        /// <summary>
        /// Converts a datetime value to Universal Coordinated Time (UTC).
        /// </summary>
        /// <param name="values">The datetime value to convert. It must be a local or unspecified datetime.</param>
        /// <returns>The datetime value expressed in UTC.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a datetime value.</exception>
        public static EvaluationResult ToUtc(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => ValueOrThrowInvalid(() => new DateTimeDatum(TimeZoneInfo.ConvertTime(val, TimeZoneInfo.Local, TimeZoneInfo.Utc))), values);

        /// <summary>
        /// Returns a new datetime that adds the specified number of years to the specified datetime.
        /// </summary>
        /// <param name="values">The datetime to add years to, the number of years to add which can be positive or negative.</param>
        /// <returns>A new datetime having added the given number of years.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a numeric one.</exception>
        public static EvaluationResult AddYears(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddYears((int)value))), values);

        /// <summary>
        /// Returns a new datetime that adds the specified number of months to the specified datetime.
        /// </summary>
        /// <param name="values">The datetime to add months to, the number of months to add which can be positive or negative.</param>
        /// <returns>A new datetime having added the given number of months.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a numeric one.</exception>
        public static EvaluationResult AddMonths(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddMonths((int)value))), values);

        /// <summary>
        /// Returns a new datetime that adds the specified number of days to the specified datetime.
        /// </summary>
        /// <param name="values">The datetime to add days to, the whole and fractional number of days to add, which can be positive or negative.</param>
        /// <returns>A new datetime having added the given number of days.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a numeric one.</exception>
        public static EvaluationResult AddDays(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddDays(value))), values);

        /// <summary>
        /// Returns a new datetime that adds the specified number of hours to the specified datetime.
        /// </summary>
        /// <param name="values">The datetime to add hours to, the whole and fractional number of hours to add, which can be positive or negative.</param>
        /// <returns>A new datetime having added the given number of hours.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a numeric one.</exception>
        public static EvaluationResult AddHours(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddHours(value))), values);

        /// <summary>
        /// Returns a new datetime that adds the specified number of minutes to the specified datetime.
        /// </summary>
        /// <param name="values">The datetime to add minutes to, the whole and fractional number of minutes to add, which can be positive or negative.</param>
        /// <returns>A new datetime having added the given number of minutes.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a numeric one.</exception>
        public static EvaluationResult AddMinutes(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddMinutes(value))), values);

        /// <summary>
        /// Returns a new datetime that adds the specified number of seconds to the specified datetime.
        /// </summary>
        /// <param name="values">The datetime to add seconds to, the whole and fractional number of seconds to add, which can be positive or negative.</param>
        /// <returns>A new datetime having added the given number of seconds.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a numeric one.</exception>
        public static EvaluationResult AddSeconds(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddSeconds(value))), values);

        /// <summary>
        /// Returns a new datetime that adds the specified number of milliseconds to the specified datetime.
        /// </summary>
        /// <param name="values">
        /// The datetime to add milliseconds to, the whole and fractional number of milliseconds to add, which can be positive or negative.
        /// Note that milliseconds is rounded to the nearest integer.
        /// </param>
        /// <returns>A new datetime having added the given number of milliseconds.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a numeric one.</exception>
        public static EvaluationResult AddMilliseconds(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddMilliseconds(value))), values);

        /// <summary>
        /// Returns the number of days between two datetimes.
        /// </summary>
        /// <param name="values">The datetime to subtract the other one from, the datetime to subtract.</param>
        /// <returns>The number of days which can be positive or negative.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are datetime values.</exception>
        public static EvaluationResult DaysDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Days)), values);

        /// <summary>
        /// Returns the number of hours between two datetimes.
        /// </summary>
        /// <param name="values">The datetime to subtract the other one from, the datetime to subtract.</param>
        /// <returns>The number of hours ranging from -23 through 23.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are datetime values.</exception>
        public static EvaluationResult HoursDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Hours)), values);

        /// <summary>
        /// Returns the number of minutes between two datetimes.
        /// </summary>
        /// <param name="values">The datetime to subtract the other one from, the datetime to subtract.</param>
        /// <returns>The number of minutes ranging from -59 through 59.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are datetime values.</exception>
        public static EvaluationResult MinutesDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Minutes)), values);

        /// <summary>
        /// Returns the number of seconds between two datetimes.
        /// </summary>
        /// <param name="values">The datetime to subtract the other one from, the datetime to subtract.</param>
        /// <returns>The number of seconds ranging from -59 through 59.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are datetime values.</exception>
        public static EvaluationResult SecondsDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Seconds)), values);

        /// <summary>
        /// Returns the number of milliseconds between two datetimes.
        /// </summary>
        /// <param name="values">The datetime to subtract the other one from, the datetime to subtract.</param>
        /// <returns>The number of milliseconds ranging from -999 through 999.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are datetime values.</exception>
        public static EvaluationResult MillisecondsDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Milliseconds)), values);

        /// <summary>
        /// Converts a string representation of a datetime in invariant culture to its datetime equivalent, using a specified format.
        /// </summary>
        /// <param name="values">A string containing the datetime info, a string specifying the exact format of the datetime info.</param>
        /// <returns>The equivalent datetime value.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are string values.</exception>
        public static EvaluationResult ParseDateTime(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsString, (dt, format) => ValueOrThrowInvalid(() => new DateTimeDatum(DateTime.ParseExact(dt, format, CultureInfo.InvariantCulture))), values);

        /// <summary>
        /// Converts a datetime to its equivalent string representation, using a specified format in invariant culture.
        /// </summary>
        /// <param name="values">A datetime to convert, a standard or custom date and time format string.</param>
        /// <returns>The string representation of the datetime value.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a datetime value or the second is not a string one.</exception>
        public static EvaluationResult DateTimeToString(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsString, (dt, format) => ValueOrThrowInvalid(() => new StringDatum(dt.ToString(format, CultureInfo.InvariantCulture))), values);

        /// <summary>
        /// Converts a numeric value to its equivalent string representation, using a specified format in invariant culture.
        /// </summary>
        /// <param name="values">A numeric value to convert, a numeric format string value.</param>
        /// <returns>The string representation of the numeric value.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The first argument is not a numeric value or the second is not a string one.</exception>
        public static EvaluationResult NumberToString(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, AsString, (num, format) => ValueOrThrowInvalid(() => new StringDatum(num.ToString(format, CultureInfo.InvariantCulture))), values);

        /// <summary>
        /// Converts a string representation of a numeric value in invariant culture to its number equivalent.
        /// </summary>
        /// <param name="values">A string containing the numeric info.</param>
        /// <returns>The equivalent numeric value.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is not a string value.</exception>
        public static EvaluationResult ParseNumber(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => ValueOrThrowInvalid(() => new NumberDatum(double.Parse(val, CultureInfo.InvariantCulture))), values);

        /// <summary>
        /// Rounds a numeric value to a specified number of fractional digits, using the away from zero rounding convention.
        /// </summary>
        /// <param name="values">
        /// A numeric value to round, a numeric value that specifies the number of decimal places in the return value.
        /// Note that only the integral part of decimal places is considered.
        /// </param>
        /// <returns>The rounded numeric value. If given numeric value to round has fewer fractional digits than the one specified, it is returned unchanged.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not two.</exception>
        /// <exception cref="WrongTypeOfArgumentException">Not all arguments are numeric values.</exception>
        public static EvaluationResult Round(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val, decimals) => ValueOrThrowInvalid(() => new NumberDatum((double)Math.Round((decimal)val, (int)decimals, MidpointRounding.AwayFromZero))), values);

        /// <summary>
        /// Returns a result by evaluating a quote symbol or a non-empty list of quote symbols as user code.
        /// </summary>
        /// <param name="values">A quote symbol or a non-empty list of quote symbols.</param>
        /// <param name="EvalCallback">A callback that is capable of performing the evaluation.</param>
        /// <returns>The result of the evaluation.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">The given arguments are not one.</exception>
        /// <exception cref="WrongTypeOfArgumentException">The argument is neither a quote symbol nor a non-empty list of quote symbols.</exception>
        public static EvaluationResult Eval(EvaluationResult[]? values, Func<EvaluationResult, EvaluationResult> EvalCallback)
        {
            RequireMoreThanZero(values, 1);
            var value = values[0];

            // Ensure that value is either a quote symbol or a non-empty list of quote symbols.
            if (value is QuoteSymbol)
                return EvalCallback(value);
            else if (value is Pair pair)
            {
                RequireListOf<QuoteSymbol>(pair);
                return EvalCallback(pair);
            }
            else
                throw new WrongTypeOfArgumentException(value.GetType(), new[] { typeof(QuoteSymbol), typeof(Pair) });
        }

        #region Helpers
        private delegate T Transform<T>(EvaluationResult value);

        private delegate EvaluationResult TransformBack<T>(T value);

        private delegate TResult TransformBackTo<T, TResult>(T value) where TResult : EvaluationResult;

        private delegate T CalculateMultiple<T>(T value1, T value2);

        private delegate EvaluationResult CalculateTriple<T1, T2, T3>(T1 value1, T2 value2, T3 value3);

        private delegate EvaluationResult CalculateBinary<T>(T value1, T value2);

        private delegate EvaluationResult CalculateBinary<T1, T2>(T1 value1, T2 value2);

        private delegate EvaluationResult CalculateBinaryWithResult<T>(T value1, EvaluationResult value2);

        private delegate EvaluationResult CalculateUnary<T>(T value);

        private static EvaluationResult ApplyMultivalueOperator<T>(Transform<T> transform, CalculateMultiple<T> calculate, TransformBack<T> resultTransform, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 2, true);

            T accumulator = transform(values[0]);
            accumulator = AccumulateRest(transform, calculate, accumulator, values);

            return resultTransform(accumulator);
        }

        private static EvaluationResult ApplyMultivalueOperatorOnEitherPrimitive<T1, T2, T3, T4>(Transform<T3> transform, Transform<T4> transformAlt, CalculateMultiple<T3> calculate, CalculateMultiple<T4> calculateAlt, TransformBackTo<T3, T1> resultTransform, TransformBackTo<T4, T2> resultTransformAlt, EvaluationResult[]? values)
            where T1 : PrimitiveDatum<T3> where T2 : PrimitiveDatum<T4> where T3 : notnull where T4 : notnull
        {
            RequireMoreThanZero(values, 2, true);

            if (values[0] is T1 t1)
            {
                T3 accumulator = AccumulateRest(transform, calculate, t1.Value, values);

                return resultTransform(accumulator);
            }
            else if (values[0] is T2 t2)
            {
                T4 accumulator = AccumulateRest(transformAlt, calculateAlt, t2.Value, values);

                return resultTransformAlt(accumulator);
            }

            throw new WrongTypeOfArgumentException(values[0].GetType(), new[] { typeof(T1), typeof(T2) });
        }

        private static T AccumulateRest<T>(Transform<T> transform, CalculateMultiple<T> calculate, T initial, EvaluationResult[] values)
        {
            T accumulator = initial;
            for (int i = 1; i < values.Length; i++)
                accumulator = calculate(accumulator, transform(values[i]));

            return accumulator;
        }

        private static EvaluationResult ApplyTripleOperator<T1, T2, T3>(Transform<T1> transform1, Transform<T2> transform2, Transform<T3> transform3, CalculateTriple<T1, T2, T3> calculate, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 3);

            T1 value1 = transform1(values[0]);
            T2 value2 = transform2(values[1]);
            T3 value3 = transform3(values[2]);

            return calculate(value1, value2, value3);
        }

        private static EvaluationResult ApplyBinaryOperator<T>(Transform<T> transform, CalculateBinary<T> calculate, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 2);
            return calculate(transform(values[0]), transform(values[1]));
        }

        private static EvaluationResult ApplyBinaryOperator<T1, T2>(Transform<T1> transform1, Transform<T2> transform2, CalculateBinary<T1, T2> calculate, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 2);
            return calculate(transform1(values[0]), transform2(values[1]));
        }

        private static EvaluationResult ApplyBinaryOperator<T>(Transform<T> transform, CalculateBinaryWithResult<T> calculate, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 2);
            return calculate(transform(values[0]), values[1]);
        }

        private static EvaluationResult ApplyBinaryOperator(CalculateBinary<EvaluationResult> calculate, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 2);
            return calculate(values[0], values[1]);
        }

        private static EvaluationResult ApplyBinaryOperatorOnEither<T1, T2>(Transform<T1> transform, Transform<T2> transformAlt, CalculateBinary<T1> calculate, CalculateBinary<T2> calculateAlt, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 2);

            if (values[0] is T1 t1)
                return calculate(t1, transform(values[1]));

            else if (values[0] is T2 t2)
                return calculateAlt(t2, transformAlt(values[1]));

            throw new WrongTypeOfArgumentException(values[0].GetType(), new[] { typeof(T1), typeof(T2) });
        }

        private static EvaluationResult ApplyUnaryOperator<T>(Transform<T> transform, CalculateUnary<T> calculate, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 1);
            return calculate(transform(values[0]));
        }

        private static EvaluationResult ApplyUnaryOperator(CalculateUnary<EvaluationResult> calculate, EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 1);
            return calculate(values[0]);
        }

        private static double AsDouble(EvaluationResult value)
            => As<NumberDatum>(value).Value;

        private static string AsString(EvaluationResult value)
            => As<StringDatum>(value).Value;

        private static DateTime AsDateTime(EvaluationResult value)
            => As<DateTimeDatum>(value).Value;

        private static bool AsBool(EvaluationResult value)
        {
            // All values are true except for false.
            if (value is BoolDatum datum && !datum.Value)
                return false;

            return true;
        }

        private static Pair AsPair(EvaluationResult value)
            => As<Pair>(value);

        private static object AsAnyPrimitive(EvaluationResult value)
            => As<IPrimitiveDatum>(value).GetValueAsObject();

        private static T As<T>(EvaluationResult value)
        {
            if (value is T t)
                return t;

            throw new WrongTypeOfArgumentException(value.GetType(), new[] { typeof(T) });
        }

        private static EvaluationResult ApplyFoldRightOperator(CalculateMultiple<EvaluationResult> foldOperator, EvaluationResult initial, EvaluationResult[] values, int valueIndex = 0)
        {
            if (valueIndex == values.Length)
                return initial;

            return foldOperator(values[valueIndex], ApplyFoldRightOperator(foldOperator, initial, values, ++valueIndex));
        }

        private static void RequireMoreThanZero([NotNull] EvaluationResult[]? values, int expected, bool orMore = false)
        {
            if (expected <= 0)
                throw new ArgumentOutOfRangeException(nameof(expected));

            if (values == null)
                throw new WrongNumberOfArgumentsException(0, expected, orMore);

            if (orMore)
            {
                if (values.Length < expected)
                    throw new WrongNumberOfArgumentsException(values.Length, expected, true);
            }
            else if (values.Length != expected)
                throw new WrongNumberOfArgumentsException(values.Length, expected);
        }

        private static void RequireZero(EvaluationResult[]? values)
        {
            if (values != null && values.Length > 0)
                throw new WrongNumberOfArgumentsException(values.Length, 0);
        }

        private static T ValueOrThrowInvalid<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                throw new InvalidValueException(ex.Message, ex);
            }
        }

        private static string? ToCurrentCultureString(object value)
        {
            string? message = value.ToString();
            if (value is bool)
                message = message?.ToLower();

            return message;
        }

        private static void RequireListOf<T>(Pair pair)
        {
            if (pair.First is Nil)
                return;
            else if (pair.First is Pair firstInnerPair)
                RequireListOf<T>(firstInnerPair);
            else
                As<T>(pair.First);

            if (pair.Second is Nil)
                return;
            else if (pair.Second is Pair secondInnerPair)
                RequireListOf<T>(secondInnerPair);
            // Ensure we are dealing with a list and not any pair.
            else
                throw new WrongTypeOfArgumentException(pair.Second.GetType(), new[] { typeof(Pair), typeof(Nil) });
        }
        #endregion
    }
}
