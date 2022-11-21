using RainLisp.Evaluation.Results;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RainLisp.Evaluation
{
    public class ProcedureApplicationVisitor : IProcedureApplicationVisitor
    {
        public EvaluationResult ApplyUserProcedure(UserProcedure procedure, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
        {
            var extendedEnvironment = procedure.Environment.ExtendEnvironment(procedure.Parameters, evaluatedArguments);

            return evaluatorVisitor.EvaluateBody(procedure.Body, extendedEnvironment);
        }

        public EvaluationResult ApplyPrimitiveProcedure(PrimitiveProcedure procedure, EvaluationResult[]? evaluatedArguments)
        {
            // Dispatch to different methods based on enum (message-passing style) instead of different procedure runtime types to avoid class explosion for primitive operations.
            return procedure.ProcedureType switch
            {
                PrimitiveProcedureType.Add => Add(evaluatedArguments),
                PrimitiveProcedureType.Subtract => Subtract(evaluatedArguments),
                PrimitiveProcedureType.Multiply => Multiply(evaluatedArguments),
                PrimitiveProcedureType.Divide => Divide(evaluatedArguments),
                PrimitiveProcedureType.Modulo => Modulo(evaluatedArguments),
                PrimitiveProcedureType.GreaterThan => GreaterThan(evaluatedArguments),
                PrimitiveProcedureType.GreaterThanOrEqualTo => GreaterThanOrEqualTo(evaluatedArguments),
                PrimitiveProcedureType.LessThan => LessThan(evaluatedArguments),
                PrimitiveProcedureType.LessThanOrEqualTo => LessThanOrEqualTo(evaluatedArguments),
                PrimitiveProcedureType.EqualTo => EqualTo(evaluatedArguments),
                PrimitiveProcedureType.LogicalXor => LogicalXor(evaluatedArguments),
                PrimitiveProcedureType.LogicalNot => LogicalNot(evaluatedArguments),
                PrimitiveProcedureType.Cons => Cons(evaluatedArguments),
                PrimitiveProcedureType.Car => Car(evaluatedArguments),
                PrimitiveProcedureType.Cdr => Cdr(evaluatedArguments),
                PrimitiveProcedureType.List => List(evaluatedArguments),
                PrimitiveProcedureType.IsNull => IsNull(evaluatedArguments),
                PrimitiveProcedureType.SetCar => SetCar(evaluatedArguments),
                PrimitiveProcedureType.SetCdr => SetCdr(evaluatedArguments),
                PrimitiveProcedureType.StringLength => StringLength(evaluatedArguments),
                PrimitiveProcedureType.Display => Display(evaluatedArguments),
                PrimitiveProcedureType.Debug => Debug(evaluatedArguments),
                PrimitiveProcedureType.Trace => Trace(evaluatedArguments),
                PrimitiveProcedureType.NewLine => NewLine(evaluatedArguments),
                PrimitiveProcedureType.Error => Error(evaluatedArguments),
                PrimitiveProcedureType.Now => Now(evaluatedArguments),
                PrimitiveProcedureType.UtcNow => UtcNow(evaluatedArguments),
                PrimitiveProcedureType.MakeDate => MakeDate(evaluatedArguments),
                PrimitiveProcedureType.MakeDateTime => MakeDateTime(evaluatedArguments),
                PrimitiveProcedureType.Year => Year(evaluatedArguments),
                PrimitiveProcedureType.Month => Month(evaluatedArguments),
                PrimitiveProcedureType.Day => Day(evaluatedArguments),
                PrimitiveProcedureType.Hour => Hour(evaluatedArguments),
                PrimitiveProcedureType.Minute => Minute(evaluatedArguments),
                PrimitiveProcedureType.Second => Second(evaluatedArguments),
                PrimitiveProcedureType.Millisecond => Millisecond(evaluatedArguments),
                PrimitiveProcedureType.IsUtc => IsUtc(evaluatedArguments),
                PrimitiveProcedureType.ToLocal => ToLocal(evaluatedArguments),
                PrimitiveProcedureType.ToUtc => ToUtc(evaluatedArguments),
                PrimitiveProcedureType.AddYears => AddYears(evaluatedArguments),
                PrimitiveProcedureType.AddMonths => AddMonths(evaluatedArguments),
                PrimitiveProcedureType.AddDays => AddDays(evaluatedArguments),
                PrimitiveProcedureType.AddHours => AddHours(evaluatedArguments),
                PrimitiveProcedureType.AddMinutes => AddMinutes(evaluatedArguments),
                PrimitiveProcedureType.AddSeconds => AddSeconds(evaluatedArguments),
                PrimitiveProcedureType.AddMilliseconds => AddMilliseconds(evaluatedArguments),
                PrimitiveProcedureType.DaysDiff => DaysDiff(evaluatedArguments),
                PrimitiveProcedureType.HoursDiff => HoursDiff(evaluatedArguments),
                PrimitiveProcedureType.MinutesDiff => MinutesDiff(evaluatedArguments),
                PrimitiveProcedureType.SecondsDiff => SecondsDiff(evaluatedArguments),
                PrimitiveProcedureType.MillisecondsDiff => MillisecondsDiff(evaluatedArguments),
                PrimitiveProcedureType.ParseDateTime => ParseDateTime(evaluatedArguments),
                PrimitiveProcedureType.DateTimeToString => DateTimeToString(evaluatedArguments),
                _ => throw new NotImplementedException()
            };
        }

        #region Primitive Operations
        private static EvaluationResult Add(EvaluationResult[]? values)
            => ApplyMultivalueOperatorOnEitherPrimitive(AsDouble, AsString,
                (val1, val2) => val1 + val2,
                (val1, val2) => val1 + val2,
                result => new NumberDatum(result),
                result => new StringDatum(result),
                values);

        private static EvaluationResult Subtract(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 - val2, result => new NumberDatum(result), values);

        private static EvaluationResult Multiply(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 * val2, result => new NumberDatum(result), values);

        private static EvaluationResult Divide(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 / val2, result => new NumberDatum(result), values);

        private static EvaluationResult Modulo(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 % val2, result => new NumberDatum(result), values);

        private static EvaluationResult GreaterThan(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value > val2.Value),
                (val1, val2) => new BoolDatum(val1.Value > val2.Value), values);

        private static EvaluationResult GreaterThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value >= val2.Value),
                (val1, val2) => new BoolDatum(val1.Value >= val2.Value), values);

        private static EvaluationResult LessThan(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value < val2.Value),
                (val1, val2) => new BoolDatum(val1.Value < val2.Value), values);

        private static EvaluationResult LessThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperatorOnEither(As<NumberDatum>, As<DateTimeDatum>,
                (val1, val2) => new BoolDatum(val1.Value <= val2.Value),
                (val1, val2) => new BoolDatum(val1.Value <= val2.Value), values);

        private static EvaluationResult EqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsAnyPrimitive, (object val1, object val2) => new BoolDatum(val1.Equals(val2)), values);

        private static EvaluationResult LogicalXor(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsBool, (val1, val2) => val1 ^ val2, result => new BoolDatum(result), values);

        private static EvaluationResult LogicalNot(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsBool, val => new BoolDatum(!val), values);

        private static EvaluationResult Cons(EvaluationResult[]? values)
            => ApplyBinaryOperator((val1, val2) => new Pair(val1, val2), values);

        private static EvaluationResult Car(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsPair, val => val.First, values);

        private static EvaluationResult Cdr(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsPair, val => val.Second, values);

        private static EvaluationResult List(EvaluationResult[]? values)
        {
            if (values == null || values.Length == 0)
                return Nil.GetNil();

            return ApplyFoldRightOperator((val1, val2) => new Pair(val1, val2), Nil.GetNil(), values);
        }

        private static EvaluationResult IsNull(EvaluationResult[]? values)
            => ApplyUnaryOperator(val => new BoolDatum(val == Nil.GetNil()), values);

        private static EvaluationResult SetCar(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsPair, (Pair val1, EvaluationResult val2) =>
            {
                val1.First = val2;
                return Unspecified.GetUnspecified();
            }, values);

        private static EvaluationResult SetCdr(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsPair, (Pair val1, EvaluationResult val2) =>
            {
                val1.Second = val2;
                return Unspecified.GetUnspecified();
            }, values);

        private static EvaluationResult StringLength(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsString, val => new NumberDatum(val.Length), values);

        private static EvaluationResult Display(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                Console.Write(val);
                return Unspecified.GetUnspecified();
            }, values);

        private static EvaluationResult Debug(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                System.Diagnostics.Debug.Write(val);
                return Unspecified.GetUnspecified();
            }, values);

        private static EvaluationResult Trace(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                System.Diagnostics.Trace.Write(val);
                return Unspecified.GetUnspecified();
            }, values);

        private static EvaluationResult NewLine(EvaluationResult[]? values)
        {
            RequireZero(values);
            Console.WriteLine();
            return Unspecified.GetUnspecified();
        }

        private static EvaluationResult Error(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsAnyPrimitive, val =>
            {
                string? message;
                if (val is double doubleVal)
                    message = doubleVal.ToString(CultureInfo.InvariantCulture);
                else
                {
                    message = val.ToString();
                    if (val is bool boolVal)
                        message = message?.ToLower();
                }

                throw new UserException(message);
            }, values);

        private static EvaluationResult Now(EvaluationResult[]? values)
        {
            RequireZero(values);
            return new DateTimeDatum(DateTime.Now);
        }

        private static EvaluationResult UtcNow(EvaluationResult[]? values)
        {
            RequireZero(values);
            return new DateTimeDatum(DateTime.UtcNow);
        }

        private static EvaluationResult MakeDate(EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 3);
            int year = (int)AsDouble(values[0]);
            int month = (int)AsDouble(values[1]);
            int day = (int)AsDouble(values[2]);

            return new DateTimeDatum(ValueOrThrowInvalid(() => new DateTime(year, month, day)));
        }

        private static EvaluationResult MakeDateTime(EvaluationResult[]? values)
        {
            RequireMoreThanZero(values, 7);
            int year = (int)AsDouble(values[0]);
            int month = (int)AsDouble(values[1]);
            int day = (int)AsDouble(values[2]);
            int hour = (int)AsDouble(values[3]);
            int minute = (int)AsDouble(values[4]);
            int second = (int)AsDouble(values[5]);
            int millisecond = (int)AsDouble(values[6]);

            return new DateTimeDatum(ValueOrThrowInvalid(() => new DateTime(year, month, day, hour, minute, second, millisecond)));
        }

        private static EvaluationResult Year(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Year), values);

        private static EvaluationResult Month(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Month), values);

        private static EvaluationResult Day(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Day), values);

        private static EvaluationResult Hour(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Hour), values);

        private static EvaluationResult Minute(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Minute), values);

        private static EvaluationResult Second(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Second), values);

        private static EvaluationResult Millisecond(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new NumberDatum(val.Millisecond), values);

        private static EvaluationResult IsUtc(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new BoolDatum(val.Kind == DateTimeKind.Utc), values);

        private static EvaluationResult ToLocal(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new DateTimeDatum(ValueOrThrowInvalid(() => TimeZoneInfo.ConvertTime(val, TimeZoneInfo.Utc, TimeZoneInfo.Local))), values);

        private static EvaluationResult ToUtc(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsDateTime, val => new DateTimeDatum(ValueOrThrowInvalid(() => TimeZoneInfo.ConvertTime(val, TimeZoneInfo.Local, TimeZoneInfo.Utc))), values);

        private static EvaluationResult AddYears(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => new DateTimeDatum(ValueOrThrowInvalid(() => dt.AddYears((int)value))), values);

        private static EvaluationResult AddMonths(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => new DateTimeDatum(ValueOrThrowInvalid(() => dt.AddMonths((int)value))), values);

        private static EvaluationResult AddDays(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => new DateTimeDatum(ValueOrThrowInvalid(() => dt.AddDays(value))), values);

        private static EvaluationResult AddHours(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => new DateTimeDatum(ValueOrThrowInvalid(() => dt.AddHours(value))), values);

        private static EvaluationResult AddMinutes(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => new DateTimeDatum(ValueOrThrowInvalid(() => dt.AddMinutes(value))), values);

        private static EvaluationResult AddSeconds(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => new DateTimeDatum(ValueOrThrowInvalid(() => dt.AddSeconds(value))), values);

        private static EvaluationResult AddMilliseconds(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsDouble, (dt, value) => new DateTimeDatum(ValueOrThrowInvalid(() => dt.AddMilliseconds(value))), values);

        private static EvaluationResult DaysDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => new NumberDatum(ValueOrThrowInvalid(() => val2.Subtract(val1)).Days), values);

        private static EvaluationResult HoursDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => new NumberDatum(ValueOrThrowInvalid(() => val2.Subtract(val1)).Hours), values);

        private static EvaluationResult MinutesDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => new NumberDatum(ValueOrThrowInvalid(() => val2.Subtract(val1)).Minutes), values);

        private static EvaluationResult SecondsDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => new NumberDatum(ValueOrThrowInvalid(() => val2.Subtract(val1)).Seconds), values);

        private static EvaluationResult MillisecondsDiff(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, (val1, val2) => new NumberDatum(ValueOrThrowInvalid(() => val2.Subtract(val1)).Milliseconds), values);

        private static EvaluationResult ParseDateTime(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsString, (dt, format) => new DateTimeDatum(ValueOrThrowInvalid(() => DateTime.ParseExact(dt, format, CultureInfo.InvariantCulture))), values);

        private static EvaluationResult DateTimeToString(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDateTime, AsString, (dt, format) => new StringDatum(ValueOrThrowInvalid(() => dt.ToString(format, CultureInfo.InvariantCulture))), values);
        #endregion

        #region Helpers
        private delegate T Transform<T>(EvaluationResult value);

        private delegate EvaluationResult TransformBack<T>(T value);

        private delegate TResult TransformBackTo<T, TResult>(T value) where TResult : EvaluationResult;

        private delegate T CalculateMultiple<T>(T value1, T value2);

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
        #endregion
    }
}
