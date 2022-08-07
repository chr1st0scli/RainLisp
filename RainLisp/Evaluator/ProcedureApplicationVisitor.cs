namespace RainLisp.Evaluator
{
    public class ProcedureApplicationVisitor : IProcedureApplicationVisitor
    {
        public object VisitUserProcedure(UserProcedure procedure, object[]? evaluatedArguments, Environment environment, IEvaluatorVisitor evaluatorVisitor)
        {
            if (procedure.Parameters?.Count != evaluatedArguments?.Length)
                throw new InvalidOperationException("Wrong number of arguments.");

            // We extend the procedure environment instead of the given one?
            var extendedEnvironment = procedure.Environment.ExtendEnvironment();

            if (procedure.Parameters?.Count > 0 && evaluatedArguments?.Length > 0)
            {
                for (int i = 0; i < procedure.Parameters.Count; i++)
                    extendedEnvironment.SetIdentifier(procedure.Parameters[i], evaluatedArguments[i]);
            }

            return evaluatorVisitor.VisitBody(procedure.Body, extendedEnvironment);
        }

        public object VisitPrimitiveProcedure(PrimitiveProcedure procedure, object[] evaluatedArguments)
        {
            // Dispatch to different methods based on enum instead of different procedure runtime types to avoid class explosion for primitive operations.
            return procedure.ProcedureType switch
            {
                PrimitiveProcedureType.Add => Add(evaluatedArguments),
                PrimitiveProcedureType.Subtract => Subtract(evaluatedArguments),
                PrimitiveProcedureType.Multiply => Multiply(evaluatedArguments),
                PrimitiveProcedureType.Divide => Divide(evaluatedArguments),
                PrimitiveProcedureType.Remainder => Remainder(evaluatedArguments),
                PrimitiveProcedureType.GreaterThan => GreaterThan(evaluatedArguments),
                PrimitiveProcedureType.GreaterThanOrEqualTo => GreaterThanOrEqualTo(evaluatedArguments),
                PrimitiveProcedureType.LessThan => LessThan(evaluatedArguments),
                PrimitiveProcedureType.LessThanOrEqualTo => LessThanOrEqualTo(evaluatedArguments),
                PrimitiveProcedureType.LogicalAnd => LogicalAnd(evaluatedArguments),
                PrimitiveProcedureType.LogicalOr => LogicalOr(evaluatedArguments),
                PrimitiveProcedureType.LogicalNot => LogicalNot(evaluatedArguments),
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

        private static object Remainder(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<double>(val1) % ValueForPrimitive<double>(val2), values);

        private static object GreaterThan(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) > ValueForPrimitive<double>(val2), values);

        private static object GreaterThanOrEqualTo(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) >= ValueForPrimitive<double>(val2), values);

        private static object LessThan(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) < ValueForPrimitive<double>(val2), values);

        private static object LessThanOrEqualTo(object[] values)
            => ApplyBinaryOperator((val1, val2) => ValueForPrimitive<double>(val1) <= ValueForPrimitive<double>(val2), values);

        private static object LogicalAnd(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<bool>(val1) && ValueForPrimitive<bool>(val2), values);

        private static object LogicalOr(object[] values)
            => ApplyMultivalueOperator((val1, val2) => ValueForPrimitive<bool>(val1) || ValueForPrimitive<bool>(val2), values);

        private static object LogicalNot(object[] values)
            => ApplyUnaryOperator(val => !ValueForPrimitive<bool>(val), values);

        private static T ApplyMultivalueOperator<T>(Func<T, T, T> primitiveOperator, T[] values)
        {
            ArgumentNullException.ThrowIfNull(primitiveOperator, nameof(primitiveOperator));
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
            ArgumentNullException.ThrowIfNull(primitiveOperator, nameof(primitiveOperator));
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 2)
                throw new ArgumentException("Exactly two arguments expected.", nameof(values));

            return primitiveOperator(values[0], values[1]);
        }

        private static T ApplyUnaryOperator<T>(Func<T, T> primitiveOperator, T[] values)
        {
            ArgumentNullException.ThrowIfNull(primitiveOperator, nameof(primitiveOperator));
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
        #endregion
    }
}
