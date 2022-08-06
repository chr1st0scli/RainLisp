﻿namespace RainLisp.Evaluator
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
                //"and" => LogicalAnd,
                _ => null
            };
        }

        public static object Add(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 + val2, values);

        public static object Subtract(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 - val2, values);

        public static object Multiply(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 * val2, values);

        public static object Divide(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 / val2, values);

        public static object Remainder(params double[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 % val2, values);

        public static object GreaterThan(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 > val2, values);

        public static object GreaterThanOrEqualTo(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 >= val2, values);

        public static object LessThan(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 < val2, values);

        public static object LessThanOrEqualTo(params double[] values)
            => ApplyBinaryOperator((val1, val2) => val1 <= val2, values);

        public static object LogicalAnd(params bool[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 && val2, values);

        public static object LogicalOr(params bool[] values)
            => ApplyMultivalueOperator((val1, val2) => val1 || val2, values);

        public static object LogicalNot(params bool[] values)
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
