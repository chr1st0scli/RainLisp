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
            // Dispatch to different methods based on enum instead of different procedure runtime types to avoid class explosion for primitive operations.
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
                _ => throw new NotImplementedException()
            };
        }

        #region Primitive Operations
        private static EvaluationResult Add(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 + val2, values);

        private static EvaluationResult Subtract(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 - val2, values);

        private static EvaluationResult Multiply(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 * val2, values);

        private static EvaluationResult Divide(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 / val2, values);

        private static EvaluationResult Modulo(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsDouble, (val1, val2) => val1 % val2, values);

        private static EvaluationResult GreaterThan(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new PrimitiveDatum<bool>(val1 > val2), values);

        private static EvaluationResult GreaterThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new PrimitiveDatum<bool>(val1 >= val2), values);

        private static EvaluationResult LessThan(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new PrimitiveDatum<bool>(val1 < val2), values);

        private static EvaluationResult LessThanOrEqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsDouble, (val1, val2) => new PrimitiveDatum<bool>(val1 <= val2), values);

        private static EvaluationResult EqualTo(EvaluationResult[]? values)
            => ApplyBinaryOperator(AsAnyPrimitive, (val1, val2) => new PrimitiveDatum<bool>(val1.Equals(val2)), values);

        private static EvaluationResult LogicalXor(EvaluationResult[]? values)
            => ApplyMultivalueOperator(AsBool, (val1, val2) => val1 ^ val2, values);

        private static EvaluationResult LogicalNot(EvaluationResult[]? values)
            => ApplyUnaryOperator(AsBool, val => new PrimitiveDatum<bool>(!val), values);

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
            => ApplyUnaryOperator(val => new PrimitiveDatum<bool>(val == Nil.GetNil()), values);
        #endregion

        #region Helpers
        [return: NotNull]
        private delegate T Transform<T>(EvaluationResult value);

        [return: NotNull]
        private delegate T CalculateMultiple<T>(T value1, T value2);

        private delegate EvaluationResult CalculateBinary<T>(T value1, T value2);

        private delegate EvaluationResult CalculateUnary<T>(T value);

        private static EvaluationResult ApplyMultivalueOperator<T>(Transform<T> transform, CalculateMultiple<T> calculate, EvaluationResult[]? values)
        {
            Require(values, 2, true);

            T accumulator = transform(values[0]);
            for (int i = 1; i < values.Length; i++)
                accumulator = calculate(accumulator, transform(values[i]));

            return new PrimitiveDatum<T>(accumulator);
        }

        private static EvaluationResult ApplyBinaryOperator<T>(Transform<T> transform, CalculateBinary<T> calculate, EvaluationResult[]? values)
        {
            Require(values, 2);
            return calculate(transform(values[0]), transform(values[1]));
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
            if (value is PrimitiveDatum<double> datum)
                return datum.Value;
            else
                throw new WrongTypeOfArgumentException(value.GetType(), typeof(PrimitiveDatum<double>));
        }

        private static bool AsBool(EvaluationResult value)
        {
            // All values are true except for false.
            if (value is PrimitiveDatum<bool> primitiveDatum && !primitiveDatum.Value)
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
            if (value is PrimitiveDatum<double> numberDatum)
                return numberDatum.Value;

            else if (value is PrimitiveDatum<bool> boolDatum)
                return boolDatum.Value;

            else if (value is PrimitiveDatum<string> stringDatum)
                return stringDatum.Value;

            throw new WrongTypeOfArgumentException(value.GetType(), typeof(PrimitiveDatum<object>));
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
        #endregion
    }
}
