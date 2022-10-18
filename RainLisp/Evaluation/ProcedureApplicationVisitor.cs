using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public class ProcedureApplicationVisitor : IProcedureApplicationVisitor
    {
        public object ApplyUserProcedure(UserProcedure procedure, object[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor)
        {
            // We extend the procedure environment instead of the given one?
            var extendedEnvironment = procedure.Environment.ExtendEnvironment(procedure.Parameters, evaluatedArguments);

            return evaluatorVisitor.EvaluateBody(procedure.Body, extendedEnvironment);
        }

        public object ApplyPrimitiveProcedure(PrimitiveProcedure procedure, object[] evaluatedArguments)
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
        private static object Add(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<double>(val1) + ValueForPrimitive<double>(val2), values);

        private static object Subtract(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<double>(val1) - ValueForPrimitive<double>(val2), values);

        private static object Multiply(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<double>(val1) * ValueForPrimitive<double>(val2), values);

        private static object Divide(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<double>(val1) / ValueForPrimitive<double>(val2), values);

        private static object Modulo(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<double>(val1) % ValueForPrimitive<double>(val2), values);

        private static object GreaterThan(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) > ValueForPrimitive<double>(val2), values);

        private static object GreaterThanOrEqualTo(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) >= ValueForPrimitive<double>(val2), values);

        private static object LessThan(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) < ValueForPrimitive<double>(val2), values);

        private static object LessThanOrEqualTo(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) <= ValueForPrimitive<double>(val2), values);

        private static object EqualTo(object[] values)
            => ApplyBinaryOperator((val1, val2) => val1.Equals(val2), values);

        private static object LogicalXor(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<bool>(val1) ^ ValueForPrimitive<bool>(val2), values);

        private static object LogicalNot(object[] values)
            => ApplyUnaryOperator(val => !ValueForPrimitive<bool>(val), values);

        private static object Cons(object[] values)
            => ApplyBinaryOperator<object, object>((val1, val2) => new Pair(val1, val2), values);

        private static object Car(object[] values)
            => ApplyUnaryOperator(val => ValueForPrimitive<Pair>(val).First, values);

        private static object Cdr(object[] values)
            => ApplyUnaryOperator(val => ValueForPrimitive<Pair>(val).Second, values);

        private static object List(object[] values)
        {
            if (values == null || values.Length == 0)
                return new Nil();

            return ApplyFoldRightOperator((val1, val2) => new Pair(val1, val2), new Nil(), values);
        }

        private static object IsNull(object[] values)
            => ApplyUnaryOperator(val => val.GetType() == typeof(Nil) , values);

        private static T ApplyMultivalueOperator<T>(Func<T, T, T> primitiveOperator, T[] values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length < 2)
                throw new ArgumentException("Too few arguments.", nameof(values));

            T accumulator = values[0];
            for (int i = 1; i < values.Length; i++)
                accumulator = primitiveOperator(accumulator, values[i]);

            return accumulator;
        }

        private static TResult ApplyBinaryOperator<T, TResult>(Func<T, T, TResult> primitiveOperator, T[] values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 2)
                throw new ArgumentException("Exactly two arguments expected.", nameof(values));

            return primitiveOperator(values[0], values[1]);
        }

        private static T ApplyUnaryOperator<T>(Func<T, T> primitiveOperator, T[] values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 1)
                throw new ArgumentException("Exactly one argument expected.", nameof(values));

            return primitiveOperator(values[0]);
        }

        private static T ValueForPrimitive<T>(object value)
        {
            try
            {
                return (T)value;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException($"{value} must be of type {typeof(T).Name}.", ex);
            }
        }

        private static object ApplyFoldRightOperator(Func<object, object, object> foldOperator, object initial, object[] values, int valueIndex = 0)
        {
            if (valueIndex == values.Length)
                return initial;

            return foldOperator(values[valueIndex], ApplyFoldRightOperator(foldOperator, initial, values, ++valueIndex));
        }
        #endregion
    }
}
