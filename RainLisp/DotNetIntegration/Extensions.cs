using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Parsing;
using RainLisp.Tokenization;

namespace RainLisp.DotNetIntegration
{
    /// <summary>
    /// Defines extension methods that ease the usage of the interpreter by other .NET code.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Executes <paramref name="code"/> immediately and returns the last expression's result.
        /// </summary>
        /// <param name="interpreter">The interpreter instance to perform the evaluation.</param>
        /// <param name="code">The code to evaluate.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the code's evaluation.</returns>
        /// <exception cref="NonTerminatedStringException">A string literal is not terminated properly; occurs during the lexical analysis of code.</exception>
        /// <exception cref="InvalidEscapeSequenceException">There is an invalid escape sequence in a string literal; occurs during the lexical analysis of code.</exception>
        /// <exception cref="InvalidStringCharacterException">There is an invalid character in a string literal; occurs during the lexical analysis of code.</exception>
        /// <exception cref="InvalidNumberCharacterException">There is an invalid character in a numeric literal; occurs during the lexical analysis of code.</exception>
        /// <exception cref="ParsingException">There is a syntax error; occurs during the syntax analysis of code.</exception>
        /// <exception cref="WrongNumberOfArgumentsException">A procedure is called with the wrong number of arguments; occurs during evaluation.</exception>
        /// <exception cref="WrongTypeOfArgumentException">A procedure is called with the wrong type of argument; occurs during evaluation.</exception>
        /// <exception cref="UnknownIdentifierException">An undefined identifier is evaluated; occurs during evaluation.</exception>
        /// <exception cref="NotProcedureException">A procedure application is evaluated on a value that is not a procedure; occurs during evaluation.</exception>
        /// <exception cref="UserException">User code explicitly caused an error; occurs during evaluation.</exception>
        /// <exception cref="InvalidValueException">A procedure is called with a wrong argument value; occurs during evaluation.</exception>
        public static EvaluationResult Execute(this IInterpreter interpreter, string? code)
            => interpreter.Evaluate(code).Last();

        /// <summary>
        /// Executes an abstract syntax tree immediately and returns the last expression's result. Lexical and syntax analysis are omitted,
        /// which can be useful for scenarios where an abstract syntax tree can be cached and reused for improved performance.
        /// </summary>
        /// <param name="interpreter">The interpreter instance to perform the evaluation.</param>
        /// <param name="program">The abstract syntax tree to evaluate.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the <paramref name="program"/>'s evaluation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="program"/> is null.</exception>
        /// <exception cref="WrongNumberOfArgumentsException">A procedure is called with the wrong number of arguments; occurs during evaluation.</exception>
        /// <exception cref="WrongTypeOfArgumentException">A procedure is called with the wrong type of argument; occurs during evaluation.</exception>
        /// <exception cref="UnknownIdentifierException">An undefined identifier is evaluated; occurs during evaluation.</exception>
        /// <exception cref="NotProcedureException">A procedure application is evaluated on a value that is not a procedure; occurs during evaluation.</exception>
        /// <exception cref="UserException">User code explicitly caused an error; occurs during evaluation.</exception>
        /// <exception cref="InvalidValueException">A procedure is called with a wrong argument value; occurs during evaluation.</exception>
        public static EvaluationResult Execute(this IInterpreter interpreter, Program program)
            => interpreter.Evaluate(program).Last();

        /// <summary>
        /// Executes <paramref name="code"/> in the given evaluation <paramref name="environment"/> immediately and returns the last expression's result.
        /// Typically, the first time it is called, null is passed to <paramref name="environment"/> which creates one and returns it by reference.
        /// Subsequent calls should use that environment to progressively add more definitions to it.
        /// </summary>
        /// <param name="interpreter">The interpreter instance to perform the evaluation.</param>
        /// <param name="code">The code to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in. If null a global environment is created and returned for subsequent evaluations.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the code's evaluation.</returns>
        /// <exception cref="NonTerminatedStringException">A string literal is not terminated properly; occurs during the lexical analysis of code.</exception>
        /// <exception cref="InvalidEscapeSequenceException">There is an invalid escape sequence in a string literal; occurs during the lexical analysis of code.</exception>
        /// <exception cref="InvalidStringCharacterException">There is an invalid character in a string literal; occurs during the lexical analysis of code.</exception>
        /// <exception cref="InvalidNumberCharacterException">There is an invalid character in a numeric literal; occurs during the lexical analysis of code.</exception>
        /// <exception cref="ParsingException">There is a syntax error; occurs during the syntax analysis of code.</exception>
        /// <exception cref="WrongNumberOfArgumentsException">A procedure is called with the wrong number of arguments; occurs during evaluation.</exception>
        /// <exception cref="WrongTypeOfArgumentException">A procedure is called with the wrong type of argument; occurs during evaluation.</exception>
        /// <exception cref="UnknownIdentifierException">An undefined identifier is evaluated; occurs during evaluation.</exception>
        /// <exception cref="NotProcedureException">A procedure application is evaluated on a value that is not a procedure; occurs during evaluation.</exception>
        /// <exception cref="UserException">User code explicitly caused an error; occurs during evaluation.</exception>
        /// <exception cref="InvalidValueException">A procedure is called with a wrong argument value; occurs during evaluation.</exception>
        public static EvaluationResult Execute(this IInterpreter interpreter, string? code, ref IEvaluationEnvironment? environment)
            => interpreter.Evaluate(code, ref environment).Last();

        /// <summary>
        /// Executes an abstract syntax tree in the given evaluation <paramref name="environment"/> immediately and returns the last expression's result.
        /// Lexical and syntax analysis are omitted, which can be useful for scenarios where an abstract syntax tree can be cached and reused for improved performance.
        /// Typically, the first time it is called, null is passed to <paramref name="environment"/> which creates one and returns it by reference.
        /// Subsequent calls should use that environment to progressively add more definitions to it.
        /// </summary>
        /// <param name="interpreter">The interpreter instance to perform the evaluation.</param>
        /// <param name="program">The abstract syntax tree to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in. If null a global environment is created and returned for subsequent evaluations.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the <paramref name="program"/>'s evaluation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="program"/> is null.</exception>
        /// <exception cref="WrongNumberOfArgumentsException">A procedure is called with the wrong number of arguments; occurs during evaluation.</exception>
        /// <exception cref="WrongTypeOfArgumentException">A procedure is called with the wrong type of argument; occurs during evaluation.</exception>
        /// <exception cref="UnknownIdentifierException">An undefined identifier is evaluated; occurs during evaluation.</exception>
        /// <exception cref="NotProcedureException">A procedure application is evaluated on a value that is not a procedure; occurs during evaluation.</exception>
        /// <exception cref="UserException">User code explicitly caused an error; occurs during evaluation.</exception>
        /// <exception cref="InvalidValueException">A procedure is called with a wrong argument value; occurs during evaluation.</exception>
        public static EvaluationResult Execute(this IInterpreter interpreter, Program program, ref IEvaluationEnvironment? environment)
            => interpreter.Evaluate(program, ref environment).Last();

        /// <summary>
        /// Returns a boolean as long as <paramref name="result"/> is a <see cref="BoolDatum"/>.
        /// </summary>
        /// <param name="result">The evaluation result to extract the boolean from.</param>
        /// <returns>The encapsulated boolean value.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="result"/> is not a <see cref="BoolDatum"/>.</exception>
        public static bool Bool(this EvaluationResult result)
            => result.Value<BoolDatum, bool>();

        /// <summary>
        /// Returns a double as long as <paramref name="result"/> is a <see cref="NumberDatum"/>.
        /// </summary>
        /// <param name="result">The evaluation result to extract the double from.</param>
        /// <returns>The encapsulated double value.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="result"/> is not a <see cref="NumberDatum"/>.</exception>
        public static double Number(this EvaluationResult result)
            => result.Value<NumberDatum, double>();

        /// <summary>
        /// Returns a string as long as <paramref name="result"/> is a <see cref="StringDatum"/>.
        /// </summary>
        /// <param name="result">The evaluation result to extract the string from.</param>
        /// <returns>The encapsulated string value.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="result"/> is not a <see cref="StringDatum"/>.</exception>
        public static string String(this EvaluationResult result)
            => result.Value<StringDatum, string>();

        /// <summary>
        /// Returns a date and time as long as <paramref name="result"/> is a <see cref="DateTimeDatum"/>.
        /// </summary>
        /// <param name="result">The evaluation result to extract the date and time from.</param>
        /// <returns>The encapsulated date and time value.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="result"/> is not a <see cref="DateTimeDatum"/>.</exception>
        public static DateTime DateTime(this EvaluationResult result)
            => result.Value<DateTimeDatum, DateTime>();

        private static T Value<TPrimitive, T>(this EvaluationResult result) where TPrimitive : PrimitiveDatum<T> where T : notnull
        {
            if (result is TPrimitive datum)
                return datum.Value;

            throw new InvalidOperationException($"Unexpected type {result.GetType().Name}. {typeof(TPrimitive).Name} was expected.");
        }
    }
}
