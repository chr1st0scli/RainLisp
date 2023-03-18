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
        public static EvaluationResult Add(EvaluationResult[]? values)
            => ApplyMultivalueOperatorOnEitherPrimitive(AsDouble, AsString,
                (val1, val2) => val1 + val2,
                (val1, val2) => val1 + val2,
                result => new NumberDatum(result),
                result => new StringDatum(result),
                values);

        public static EvaluationResult Subtract(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 - val2, result => new NumberDatum(result), values);

        public static EvaluationResult Multiply(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 * val2, result => new NumberDatum(result), values);

        public static EvaluationResult Divide(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 / val2, result => new NumberDatum(result), values);

        public static EvaluationResult Modulo(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 % val2, result => new NumberDatum(result), values);

        public static EvaluationResult GreaterThan(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value > val2.Value),
                (val1, val2) => new BoolDatum(val1.Value > val2.Value), values);

        public static EvaluationResult GreaterThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value >= val2.Value),
                (val1, val2) => new BoolDatum(val1.Value >= val2.Value), values);

        public static EvaluationResult LessThan(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value < val2.Value),
                (val1, val2) => new BoolDatum(val1.Value < val2.Value), values);

        public static EvaluationResult LessThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value <= val2.Value),
                (val1, val2) => new BoolDatum(val1.Value <= val2.Value), values);

        public static EvaluationResult EqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator((val1, val2) => new BoolDatum(val1.Equals(val2)), values);

        public static EvaluationResult LogicalXor(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsBool, (val1, val2) => val1 ^ val2, result => new BoolDatum(result), values);

        public static EvaluationResult LogicalNot(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsBool, val => new BoolDatum(!val), values);

        public static EvaluationResult Cons(EvaluationResult[]? values)
            => ApplyBinaryOperator((val1, val2) => new Pair(val1, val2), values);

        public static EvaluationResult Car(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsPair, val => val.First, values);

        public static EvaluationResult Cdr(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsPair, val => val.Second, values);

        public static EvaluationResult IsPair(EvaluationResult[]? values)
            => ApplyUnaryOperator(val => new BoolDatum(val is Pair), values);

        public static EvaluationResult List(EvaluationResult[]? values)
        {
            if (values == null || values.Length == 0)
                return Nil.GetNil();

            return ApplyFoldRightOperator((val1, val2) => new Pair(val1, val2), Nil.GetNil(), values);
        }

        public static EvaluationResult IsNull(EvaluationResult[]? values)
            => ApplyUnaryOperator(val => new BoolDatum(val == Nil.GetNil()), values);

        public static EvaluationResult SetCar(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsPair, (Pair val1, EvaluationResult val2) =>
            {
                val1.First = val2;
                return Unspecified.GetUnspecified();
            }, values);

        public static EvaluationResult SetCdr(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsPair, (Pair val1, EvaluationResult val2) =>
            {
                val1.Second = val2;
                return Unspecified.GetUnspecified();
            }, values);

        public static EvaluationResult StringLength(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => new NumberDatum(val.Length), values);

        public static EvaluationResult Substring(EvaluationResult[]? values)
            => ApplyTripleOperator(AsString, AsDouble, AsDouble,
                (value, startIndex, length) => ValueOrThrowInvalid(() => new StringDatum(value.Substring((int)startIndex, (int)length))),
                values);

        public static EvaluationResult IndexOfString(EvaluationResult[]? values)
            => ApplyTripleOperator(AsString, AsString, AsDouble,
                (value, searchValue, startIndex) => ValueOrThrowInvalid(() => new NumberDatum(value.IndexOf(searchValue, (int)startIndex))),
                values);

        public static EvaluationResult ReplaceString(EvaluationResult[]? values)
            => ApplyTripleOperator(AsString, AsString, AsString,
                (value, oldValue, newValue) => ValueOrThrowInvalid(() => new StringDatum(value.Replace(oldValue, newValue))),
                values);

        public static EvaluationResult ToLower(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => new StringDatum(val.ToLowerInvariant()), values);

        public static EvaluationResult ToUpper(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => new StringDatum(val.ToUpperInvariant()), values);

        public static EvaluationResult Display(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                Console.Write(ToCurrentCultureString(val));
                return Unspecified.GetUnspecified();
            }, values);

        public static EvaluationResult Debug(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                System.Diagnostics.Debug.Write(ToCurrentCultureString(val));
                return Unspecified.GetUnspecified();
            }, values);

        public static EvaluationResult Trace(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                System.Diagnostics.Trace.Write(ToCurrentCultureString(val));
                return Unspecified.GetUnspecified();
            }, values);

        public static EvaluationResult NewLine(EvaluationResult[]? values)
        {
            RequireZero(values);
            Console.WriteLine();
            return Unspecified.GetUnspecified();
        }

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

        public static EvaluationResult Now(EvaluationResult[]? values)
        {
            RequireZero(values);
            return new DateTimeDatum(DateTime.Now);
        }

        public static EvaluationResult UtcNow(EvaluationResult[]? values)
        {
            RequireZero(values);
            return new DateTimeDatum(DateTime.UtcNow);
        }

        public static EvaluationResult MakeDate(EvaluationResult[]? values)
            => ApplyTripleOperator(AsDouble, AsDouble, AsDouble,
                (year, month, day) => ValueOrThrowInvalid(() => new DateTimeDatum(new DateTime((int)year, (int)month, (int)day))),
                values);

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

        public static EvaluationResult Year(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Year), values);

        public static EvaluationResult Month(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Month), values);

        public static EvaluationResult Day(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Day), values);

        public static EvaluationResult Hour(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Hour), values);

        public static EvaluationResult Minute(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Minute), values);

        public static EvaluationResult Second(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Second), values);

        public static EvaluationResult Millisecond(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Millisecond), values);

        public static EvaluationResult IsUtc(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new BoolDatum(val.Kind == DateTimeKind.Utc), values);

        public static EvaluationResult ToLocal(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => ValueOrThrowInvalid(() => new DateTimeDatum(TimeZoneInfo.ConvertTime(val, TimeZoneInfo.Utc, TimeZoneInfo.Local))), values);

        public static EvaluationResult ToUtc(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => ValueOrThrowInvalid(() => new DateTimeDatum(TimeZoneInfo.ConvertTime(val, TimeZoneInfo.Local, TimeZoneInfo.Utc))), values);

        public static EvaluationResult AddYears(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddYears((int)value))), values);

        public static EvaluationResult AddMonths(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddMonths((int)value))), values);

        public static EvaluationResult AddDays(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddDays(value))), values);

        public static EvaluationResult AddHours(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddHours(value))), values);

        public static EvaluationResult AddMinutes(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddMinutes(value))), values);

        public static EvaluationResult AddSeconds(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddSeconds(value))), values);

        public static EvaluationResult AddMilliseconds(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => ValueOrThrowInvalid(() => new DateTimeDatum(dt.AddMilliseconds(value))), values);

        public static EvaluationResult DaysDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Days)), values);

        public static EvaluationResult HoursDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Hours)), values);

        public static EvaluationResult MinutesDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Minutes)), values);

        public static EvaluationResult SecondsDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Seconds)), values);

        public static EvaluationResult MillisecondsDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => ValueOrThrowInvalid(() => new NumberDatum(val2.Subtract(val1).Milliseconds)), values);

        public static EvaluationResult ParseDateTime(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsString, (dt, format) => ValueOrThrowInvalid(() => new DateTimeDatum(DateTime.ParseExact(dt, format, CultureInfo.InvariantCulture))), values);

        public static EvaluationResult DateTimeToString(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsString, (dt, format) => ValueOrThrowInvalid(() => new StringDatum(dt.ToString(format, CultureInfo.InvariantCulture))), values);

        public static EvaluationResult NumberToString(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, AsString, (num, format) => ValueOrThrowInvalid(() => new StringDatum(num.ToString(format, CultureInfo.InvariantCulture))), values);

        public static EvaluationResult ParseNumber(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => ValueOrThrowInvalid(() => new NumberDatum(double.Parse(val, CultureInfo.InvariantCulture))), values);

        public static EvaluationResult Round(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val, decimals) => ValueOrThrowInvalid(() => new NumberDatum((double)Math.Round((decimal)val, (int)decimals, MidpointRounding.AwayFromZero))), values);

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
