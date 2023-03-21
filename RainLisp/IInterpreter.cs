using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Parsing;
using RainLisp.Tokenization;

namespace RainLisp
{
    /// <summary>
    /// Represents an interpreter that can evaluate code.
    /// </summary>
    public interface IInterpreter
    {
        /// <summary>
        /// Gets or sets an environment factory that is capable of creating a custom <see cref="IEvaluationEnvironment"/>.
        /// If not set, the default evaluation environment is created when appropriate.
        /// </summary>
        public IEnvironmentFactory? EnvironmentFactory { get; set; }

        /// <summary>
        /// Evaluates <paramref name="code"/> and returns the results.
        /// </summary>
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
        IEnumerable<EvaluationResult> Evaluate(string? code);

        /// <summary>
        /// Evaluates an abstract syntax tree. Lexical and syntax analysis are omitted, which can be useful for
        /// scenarios where an abstract syntax tree can be cached and reused for improved performance.
        /// </summary>
        /// <param name="program">The abstract syntax tree to evaluate.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the <paramref name="program"/>'s evaluation.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">A procedure is called with the wrong number of arguments; occurs during evaluation.</exception>
        /// <exception cref="WrongTypeOfArgumentException">A procedure is called with the wrong type of argument; occurs during evaluation.</exception>
        /// <exception cref="UnknownIdentifierException">An undefined identifier is evaluated; occurs during evaluation.</exception>
        /// <exception cref="NotProcedureException">A procedure application is evaluated on a value that is not a procedure; occurs during evaluation.</exception>
        /// <exception cref="UserException">User code explicitly caused an error; occurs during evaluation.</exception>
        /// <exception cref="InvalidValueException">A procedure is called with a wrong argument value; occurs during evaluation.</exception>
        IEnumerable<EvaluationResult> Evaluate(Program program);

        /// <summary>
        /// Evaluates <paramref name="code"/> in the given evaluation <paramref name="environment"/> and returns the results.
        /// Typically, the first time it is called, null is passed to <paramref name="environment"/> which creates one and returns it by reference.
        /// Subsequent calls should use that environment to progressively add more definitions to it.
        /// </summary>
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
        IEnumerable<EvaluationResult> Evaluate(string? code, ref IEvaluationEnvironment? environment);

        /// <summary>
        /// Evaluates an abstract syntax tree in the given evaluation <paramref name="environment"/>. Lexical and syntax analysis
        /// are omitted, which can be useful for scenarios where an abstract syntax tree can be cached and reused for improved performance.
        /// Typically, the first time it is called, null is passed to <paramref name="environment"/> which creates one and returns it by reference.
        /// Subsequent calls should use that environment to progressively add more definitions to it.
        /// </summary>
        /// <param name="program">The abstract syntax tree to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in. If null a global environment is created and returned for subsequent evaluations.</param>
        /// <returns>An <see cref="IEnumerable{EvaluationResult}"/> whose elements are the results of the <paramref name="program"/>'s evaluation.</returns>
        /// <exception cref="WrongNumberOfArgumentsException">A procedure is called with the wrong number of arguments; occurs during evaluation.</exception>
        /// <exception cref="WrongTypeOfArgumentException">A procedure is called with the wrong type of argument; occurs during evaluation.</exception>
        /// <exception cref="UnknownIdentifierException">An undefined identifier is evaluated; occurs during evaluation.</exception>
        /// <exception cref="NotProcedureException">A procedure application is evaluated on a value that is not a procedure; occurs during evaluation.</exception>
        /// <exception cref="UserException">User code explicitly caused an error; occurs during evaluation.</exception>
        /// <exception cref="InvalidValueException">A procedure is called with a wrong argument value; occurs during evaluation.</exception>
        IEnumerable<EvaluationResult> Evaluate(Program program, ref IEvaluationEnvironment? environment);

        /// <summary>
        /// Reads, evaluates and prints the results safely and indefinitely. It facilitates a common REPL (Read Eval Print Loop) mechanism.
        /// </summary>
        /// <param name="read">A function that reads code from user input.</param>
        /// <param name="print">A function that prints an evaluation result.</param>
        /// <param name="printError">A function that prints an evaluation error.</param>
        /// <exception cref="ArgumentNullException">Any of the function arguments is null.</exception>
        void ReadEvalPrintLoop(Func<string?> read, PrintResult print, PrintError printError);

        /// <summary>
        /// Evaluates <paramref name="code"/> safely in the given evaluation <paramref name="environment"/> and forwards the results and possible errors to the given functions.
        /// Typically, the first time it is called, null is passed to <paramref name="environment"/> which creates one and returns it by reference.
        /// Subsequent calls should use that environment to progressively add more definitions to it.
        /// </summary>
        /// <param name="code">The code to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in. If null a global environment is created and returned for subsequent evaluations.</param>
        /// <param name="print">A function that prints an evaluation result.</param>
        /// <param name="printError">A function that prints an evaluation error.</param>
        /// <exception cref="ArgumentNullException"><paramref name="print"/> and/or <paramref name="printError"/> is null.</exception>
        void EvaluateAndPrint(string? code, ref IEvaluationEnvironment? environment, PrintResult print, PrintError printError);
    }
}
