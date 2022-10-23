using RainLisp.Environment;

namespace RainLisp.Evaluation
{
    public class ProcedureApplicationVisitor : IProcedureApplicationVisitor
    {
        public EvaluationResult ApplyUserProcedure(UserProcedure procedure, EvaluationResult[]? evaluatedArguments, IEvaluationEnvironment environment, IEvaluatorVisitor evaluatorVisitor)
        {
            // We extend the procedure environment instead of the given one?
            var extendedEnvironment = procedure.Environment.ExtendEnvironment(procedure.Parameters, evaluatedArguments);

            return evaluatorVisitor.EvaluateBody(procedure.Body, extendedEnvironment);
        }

        public EvaluationResult ApplyPrimitiveProcedure(PrimitiveProcedure procedure, EvaluationResult[] evaluatedArguments)
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
        private static EvaluationResult Add(EvaluationResult[] values)
            => ApplyMultivalueOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => val1 + val2, values);

        private static EvaluationResult Subtract(EvaluationResult[] values)
            => ApplyMultivalueOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => val1 - val2, values);

        private static EvaluationResult Multiply(EvaluationResult[] values)
            => ApplyMultivalueOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => val1 * val2, values);

        private static EvaluationResult Divide(EvaluationResult[] values)
            => ApplyMultivalueOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => val1 / val2, values);

        private static EvaluationResult Modulo(EvaluationResult[] values)
            => ApplyMultivalueOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => val1 % val2, values);

        private static EvaluationResult GreaterThan(EvaluationResult[] values)
            => ApplyBinaryOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => new PrimitiveDatum(val1 > val2), values);

        private static EvaluationResult GreaterThanOrEqualTo(EvaluationResult[] values)
            => ApplyBinaryOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => new PrimitiveDatum(val1 >= val2), values);

        private static EvaluationResult LessThan(EvaluationResult[] values)
            => ApplyBinaryOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => new PrimitiveDatum(val1 < val2), values);

        private static EvaluationResult LessThanOrEqualTo(EvaluationResult[] values)
            => ApplyBinaryOperator(val => ValueAsPrimitive<double>(val), (val1, val2) => new PrimitiveDatum(val1 <= val2), values);

        private static EvaluationResult EqualTo(EvaluationResult[] values)
            => ApplyBinaryOperator(val => ValueAs<PrimitiveDatum>(val), (val1, val2) => new PrimitiveDatum(val1.Value.Equals(val2.Value)), values);

        private static EvaluationResult LogicalXor(EvaluationResult[] values)
            => ApplyMultivalueOperator(val => ValueAsPrimitive<bool>(val), (val1, val2) => val1 ^ val2, values);

        private static EvaluationResult LogicalNot(EvaluationResult[] values)
            => ApplyUnaryOperator(val => ValueAsPrimitive<bool>(val), val => new PrimitiveDatum(!val), values);

        private static EvaluationResult Cons(EvaluationResult[] values)
            => ApplyBinaryOperator(val => val, (val1, val2) => new Pair(val1, val2), values);

        private static EvaluationResult Car(EvaluationResult[] values)
            => ApplyUnaryOperator(val => ValueAs<Pair>(val), val => val.First, values);

        private static EvaluationResult Cdr(EvaluationResult[] values)
            => ApplyUnaryOperator(val => ValueAs<Pair>(val), val => val.Second, values);

        private static EvaluationResult List(EvaluationResult[] values)
        {
            if (values == null || values.Length == 0)
                return Nil.GetNil();

            return ApplyFoldRightOperator((val1, val2) => new Pair(val1, val2), Nil.GetNil(), values);
        }

        private static EvaluationResult IsNull(EvaluationResult[] values)
            => ApplyUnaryOperator(val => val, val => new PrimitiveDatum(val == Nil.GetNil()), values);

        private static EvaluationResult ApplyMultivalueOperator<T>(Func<EvaluationResult, T> toPrimitive, Func<T, T, T> primitiveOperator, EvaluationResult[] values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length < 2)
                throw new ArgumentException("Too few arguments.", nameof(values));

            T accumulator = toPrimitive(values[0]);
            for (int i = 1; i < values.Length; i++)
                accumulator = primitiveOperator(accumulator, toPrimitive(values[i]));

            return new PrimitiveDatum(accumulator!);
        }

        private static EvaluationResult ApplyBinaryOperator<T>(Func<EvaluationResult, T> toPrimitive, Func<T, T, EvaluationResult> primitiveOperator, EvaluationResult[] values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 2)
                throw new ArgumentException("Exactly two arguments expected.", nameof(values));

            return primitiveOperator(toPrimitive(values[0]), toPrimitive(values[1]));
        }

        private static EvaluationResult ApplyUnaryOperator<T>(Func<EvaluationResult, T> toPrimitive, Func<T, EvaluationResult> primitiveOperator, EvaluationResult[] values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 1)
                throw new ArgumentException("Exactly one argument expected.", nameof(values));

            return primitiveOperator(toPrimitive(values[0]));
        }

        private static T ValueAsPrimitive<T>(EvaluationResult value)
        {
            if (value is PrimitiveDatum datum)
                return ValueAs<T>(datum.Value);
            else
                throw new InvalidOperationException($"{value} must be of type {typeof(T).Name}.");
        }

        private static T ValueAs<T>(object value)
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

        private static EvaluationResult ApplyFoldRightOperator(Func<EvaluationResult, EvaluationResult, EvaluationResult> foldOperator, EvaluationResult initial, EvaluationResult[] values, int valueIndex = 0)
        {
            if (valueIndex == values.Length)
                return initial;

            return foldOperator(values[valueIndex], ApplyFoldRightOperator(foldOperator, initial, values, ++valueIndex));
        }
        #endregion
    }
}
