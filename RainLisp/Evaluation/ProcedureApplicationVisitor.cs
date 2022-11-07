using RainLisp.Evaluation.Results;
using System.Diagnostics.CodeAnalysis;

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
            // Dispatch to different methods based on enum (data-directed style) instead of different procedure runtime types to avoid class explosion for primitive operations.
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
                PrimitiveProcedureType.Display => Display(evaluatedArguments),
                PrimitiveProcedureType.Debug => Debug(evaluatedArguments),
                PrimitiveProcedureType.Trace => Trace(evaluatedArguments),
                PrimitiveProcedureType.NewLine => NewLine(evaluatedArguments),
                PrimitiveProcedureType.Error => Error(evaluatedArguments),
                _ => throw new NotImplementedException()
            };
        }

        #region Primitive Operations
        private static EvaluationResult Add(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 + val2, values, result => new NumberDatum(result));

        private static EvaluationResult Subtract(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 - val2, values, result => new NumberDatum(result));

        private static EvaluationResult Multiply(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 * val2, values, result => new NumberDatum(result));

        private static EvaluationResult Divide(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 / val2, values, result => new NumberDatum(result));

        private static EvaluationResult Modulo(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 % val2, values, result => new NumberDatum(result));

        private static EvaluationResult GreaterThan(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new BoolDatum(val1 > val2), values);

        private static EvaluationResult GreaterThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new BoolDatum(val1 >= val2), values);

        private static EvaluationResult LessThan(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new BoolDatum(val1 < val2), values);

        private static EvaluationResult LessThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new BoolDatum(val1 <= val2), values);

        private static EvaluationResult EqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsAnyPrimitive, (object val1, object val2) => new BoolDatum(val1.Equals(val2)), values);

        private static EvaluationResult LogicalXor(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsBool, (val1, val2) => val1 ^ val2, values, result => new BoolDatum(result));

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
            => ApplyUnaryOperator(AsAnyPrimitive, val => throw new UserException(val.ToString()), values);
        #endregion

        #region Helpers
        private delegate T Transform<T>(EvaluationResult value);

        private delegate EvaluationResult TransformBack<T>(T value);

        private delegate T CalculateMultiple<T>(T value1, T value2);

        private delegate EvaluationResult CalculateBinary<T>(T value1, T value2);

        private delegate EvaluationResult CalculateBinaryWithResult<T>(T value1, EvaluationResult value2);

        private delegate EvaluationResult CalculateUnary<T>(T value);

        private static EvaluationResult ApplyMultivalueOperator<T>(Transform<T> transform, CalculateMultiple<T> calculate, EvaluationResult[]? values, TransformBack<T> resultTransform) where T : notnull
        {
            Require(values, 2, true);

            T accumulator = transform(values[0]);
            for (int i = 1; i < values.Length; i++)
                accumulator = calculate(accumulator, transform(values[i]));

            return resultTransform(accumulator);
        }

        private static EvaluationResult ApplyBinaryOperator<T>(Transform<T> transform, CalculateBinary<T> calculate, EvaluationResult[]? values)
        {
            Require(values, 2);
            return calculate(transform(values[0]), transform(values[1]));
        }

        private static EvaluationResult ApplyBinaryOperator<T>(Transform<T> transform, CalculateBinaryWithResult<T> calculate, EvaluationResult[]? values)
        {
            Require(values, 2);
            return calculate(transform(values[0]), values[1]);
        }

        private static EvaluationResult ApplyBinaryOperator(CalculateBinary<EvaluationResult> calculate, EvaluationResult[]? values)
        {
            Require(values, 2);
            return calculate(values[0], values[1]);
        }

        private static EvaluationResult ApplyUnaryOperator<T>(Transform<T> transform, CalculateUnary<T> calculate, EvaluationResult[]? values)
        {
            Require(values, 1);
            return calculate(transform(values[0]));
        }

        private static EvaluationResult ApplyUnaryOperator(CalculateUnary<EvaluationResult> calculate, EvaluationResult[]? values)
        {
            Require(values, 1);
            return calculate(values[0]);
        }

        private static double AsDouble(EvaluationResult value)
        {
            if (value is NumberDatum datum)
                return datum.Value;
            else
                throw new WrongTypeOfArgumentException(value.GetType(), typeof(NumberDatum));
        }

        private static bool AsBool(EvaluationResult value)
        {
            // All values are true except for false.
            if (value is BoolDatum datum && !datum.Value)
                return false;

            return true;
        }

        private static Pair AsPair(EvaluationResult value)
        {
            if (value is Pair pair)
                return pair;

            throw new WrongTypeOfArgumentException(value.GetType(), typeof(Pair));
        }

        private static object AsAnyPrimitive(EvaluationResult value)
        {
            if (value is IPrimitiveDatum primitiveDatum)
                return primitiveDatum.GetValueAsObject();

            throw new WrongTypeOfArgumentException(value.GetType(), typeof(IPrimitiveDatum));
        }

        private static EvaluationResult ApplyFoldRightOperator(CalculateMultiple<EvaluationResult> foldOperator, EvaluationResult initial, EvaluationResult[] values, int valueIndex = 0)
        {
            if (valueIndex == values.Length)
                return initial;

            return foldOperator(values[valueIndex], ApplyFoldRightOperator(foldOperator, initial, values, ++valueIndex));
        }

        private static void Require([NotNull] EvaluationResult[]? values, int expected, bool orMore = false)
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
        #endregion
    }
}
