namespace RainLisp.Evaluator
{
    public static class PrimitiveProcedures
    {
        public static Func<double[], object>? GetPrimitiveProcedure(string identifierName)
        {
            return identifierName switch
            {
                "+" => Add,
                "-" => Subtract,
                "*" => Multiply,
                "/" => Divide,
                "%" => Remainder,
                ">" => GreaterThan,
                ">=" => GreaterThanOrEqualTo,
                "<" => LessThan,
                "<=" => LessThanOrEqualTo,
                _ => null
            };
        }

        public static object Add(params double[] values)
            => ApplyOnValues((val1, val2) => val1 + val2, values);

        public static object Subtract(params double[] values)
            => ApplyOnValues((val1, val2) => val1 - val2, values);

        public static object Multiply(params double[] values)
            => ApplyOnValues((val1, val2) => val1 * val2, values);

        public static object Divide(params double[] values)
            => ApplyOnValues((val1, val2) => val1 / val2, values);

        public static object Remainder(params double[] values)
            => ApplyOnValues((val1, val2) => val1 % val2, values);

        public static object GreaterThan(params double[] values)
            => ApplyOnValues((val1, val2) => val1 > val2, values);

        public static object GreaterThanOrEqualTo(params double[] values)
            => ApplyOnValues((val1, val2) => val1 >= val2, values);

        public static object LessThan(params double[] values)
            => ApplyOnValues((val1, val2) => val1 < val2, values);

        public static object LessThanOrEqualTo(params double[] values)
            => ApplyOnValues((val1, val2) => val1 <= val2, values);

        private static double ApplyOnValues(Func<double, double, double> primitiveOperator, params double[] values)
        {
            ArgumentNullException.ThrowIfNull(primitiveOperator, nameof(primitiveOperator));
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length < 2)
                throw new ArgumentException("Too few arguments.", nameof(values));

            double accumulator = values[0];
            for (int i = 1; i < values.Length; i++)
                accumulator = primitiveOperator(accumulator, values[i]);

            return accumulator;
        }

        private static bool ApplyOnValues(Func<double, double, bool> primitiveOperator, params double[] values)
        {
            ArgumentNullException.ThrowIfNull(primitiveOperator, nameof(primitiveOperator));
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Length != 2)
                throw new ArgumentException("Exactly two arguments expected.", nameof(values));

            return primitiveOperator(values[0], values[1]);
        }
    }
}
