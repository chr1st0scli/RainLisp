using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.DotNetIntegration
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static EvaluationResult EvaluateNow(this IInterpreter interpreter, string? code)
            => interpreter.Evaluate(code).Last();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="program"></param>
        /// <returns></returns>
        public static EvaluationResult EvaluateNow(this IInterpreter interpreter, Program program)
            => interpreter.Evaluate(program).Last();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="code"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static EvaluationResult EvaluateNow(this IInterpreter interpreter, string? code, ref IEvaluationEnvironment? environment)
            => interpreter.Evaluate(code, ref environment).Last();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="program"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static EvaluationResult EvaluateNow(this IInterpreter interpreter, Program program, ref IEvaluationEnvironment? environment)
            => interpreter.Evaluate(program, ref environment).Last();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Bool(this EvaluationResult result)
            => result.Value<BoolDatum, bool>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static double Number(this EvaluationResult result)
            => result.Value<NumberDatum, double>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string String(this EvaluationResult result)
            => result.Value<StringDatum, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static DateTime DateTime(this EvaluationResult result)
            => result.Value<DateTimeDatum, DateTime>();

        private static T Value<TPrimitive, T>(this EvaluationResult result) where TPrimitive : PrimitiveDatum<T> where T : notnull
        {
            if (result is TPrimitive datum)
                return datum.Value;

            throw new InvalidOperationException($"Unexpected runtime type {result.GetType().Name}.");
        }
    }
}
