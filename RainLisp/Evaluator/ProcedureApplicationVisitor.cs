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

        public object VisitPrimitiveProcedure(PrimitiveProcedure procedure, object[]? evaluatedArguments)
        {
            // Dispatch to different methods based on enum instead of different procedure runtime types to avoid class explosion for primitive operations.
            return procedure.ProcedureType switch
            {
                PrimitiveProcedureType.Add => Add(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.Subtract => Subtract(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.Multiply => Multiply(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.Divide => Divide(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.Remainder => Remainder(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.GreaterThan => GreaterThan(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.GreaterThanOrEqualTo => GreaterThanOrEqualTo(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.LessThan => LessThan(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.LessThanOrEqualTo => LessThanOrEqualTo(GetPrimitiveTypeSafeValues<double>(evaluatedArguments)),
                PrimitiveProcedureType.LogicalAnd => LogicalAnd(GetPrimitiveTypeSafeValues<bool>(evaluatedArguments)),
                PrimitiveProcedureType.LogicalOr => LogicalOr(GetPrimitiveTypeSafeValues<bool>(evaluatedArguments)),
                PrimitiveProcedureType.LogicalNot => LogicalNot(GetPrimitiveTypeSafeValues<bool>(evaluatedArguments)),
                _ => throw new NotImplementedException()
            };
        }

        private static T[] GetPrimitiveTypeSafeValues<T>(object[]? values)
        {
            try
            {
                return values?.Cast<T>().ToArray() ?? Array.Empty<T>();
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException($"All arguments must be of type {typeof(T).Name}.", ex);
            }
        }

        private static object Add(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 + val2, values);

        private static object Subtract(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 - val2, values);

        private static object Multiply(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 * val2, values);

        private static object Divide(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 / val2, values);

        private static object Remainder(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 % val2, values);

        private static object GreaterThan(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 > val2, values);

        private static object GreaterThanOrEqualTo(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 >= val2, values);

        private static object LessThan(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 < val2, values);

        private static object LessThanOrEqualTo(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 <= val2, values);

        private static object LogicalAnd(params bool[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 && val2, values);

        private static object LogicalOr(params bool[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 || val2, values);

        private static object LogicalNot(params bool[] values)
            => ApplyUnaryOperator(val => !val, values);

        private static T ApplyMultivalueOperator<T>(Func<T, T, T> primitiveOperator, params T[] values)
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

        private static bool ApplyBinaryOperator(Func<double, double, bool> primitiveOperator, params double[] values)
        {
            ArgumentNullException.ThrowIfNull(primitiveOperator, nameof(primitiveOperator));
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 2)
                throw new ArgumentException("Exactly two arguments expected.", nameof(values));

            return primitiveOperator(values[0], values[1]);
        }

        private static bool ApplyUnaryOperator(Func<bool, bool> primitiveOperator, params bool[] values)
        {
            ArgumentNullException.ThrowIfNull(primitiveOperator, nameof(primitiveOperator));
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 1)
                throw new ArgumentException("Exactly one argument expected.", nameof(values));

            return primitiveOperator(values[0]);
        }
    }
}
