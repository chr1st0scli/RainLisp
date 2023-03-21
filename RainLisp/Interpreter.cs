using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Parsing;
using RainLisp.Tokenization;
using System.Reflection;
using static RainLisp.Grammar.Primitives;

namespace RainLisp
{
    /// <summary>
    /// Represents an interpreter that can evaluate code.
    /// </summary>
    public class Interpreter : IInterpreter
    {
        private readonly ITokenizer _tokenizer;
        private readonly IParser _parser;
        private readonly IEvaluatorVisitor _evaluator;
        private readonly IEvaluationResultVisitor<string> _resultPrinter;
        private readonly bool _installLispLibraries;

        private IEvaluationEnvironment? _mostRecentGlobalEnvironment;

        private static Type[]? _primitiveTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interpreter"/> class.
        /// </summary>
        /// <param name="tokenizer">A tokenizer capable of performing lexical analysis and produce tokens. If null, the default tokenizer is used.</param>
        /// <param name="parser">A parser capable of performing syntax analysis and produce an abstract syntax tree. If null, the default parser is used.</param>
        /// <param name="evaluator">An evaluator capable of evaluating an abstract syntax tree and produce a result. If null, the default evaluator is used.</param>
        /// <param name="resultPrinter">A result printer that transforms different evaluation results to their string representation. If null, the default result printer is used.</param>
        /// <param name="installLispLibraries">Determines whether common LISP libraries should be installed in the global environment. Default value is true.</param>
        public Interpreter(ITokenizer? tokenizer = null, IParser? parser = null, IEvaluatorVisitor? evaluator = null, IEvaluationResultVisitor<string>? resultPrinter = null, bool installLispLibraries = true)
        {
            _tokenizer = tokenizer ?? new Tokenizer();
            _parser = parser ?? new Parser();
            _evaluator = evaluator ?? new EvaluatorVisitor(new ProcedureApplicationVisitor());
            _resultPrinter = resultPrinter ?? new EvaluationResultPrintVisitor();
            _installLispLibraries = installLispLibraries;
            LoadPrimitiveTypes();
        }

        /// <summary>
        /// Gets or sets an environment factory that is capable of creating a custom <see cref="IEvaluationEnvironment"/>.
        /// If not set, the default evaluation environment is created when appropriate.
        /// </summary>
        public IEnvironmentFactory? EnvironmentFactory { get; set; }

        /// <summary>
        /// Evaluates <paramref name="code"/> and returns the results.
        /// </summary>
        /// <param name="code">The LISP code to evaluate.</param>
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
        public IEnumerable<EvaluationResult> Evaluate(string? code)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(code, ref environment);
        }

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
        public IEnumerable<EvaluationResult> Evaluate(Program program)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(program, ref environment);
        }

        /// <summary>
        /// Evaluates <paramref name="code"/> in the given evaluation <paramref name="environment"/> and returns the results.
        /// Typically, the first time it is called, null is passed to <paramref name="environment"/> which creates one and returns it by reference.
        /// Subsequent calls should use that environment to progressively add more definitions to it.
        /// </summary>
        /// <param name="code">The LISP code to evaluate.</param>
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
        public IEnumerable<EvaluationResult> Evaluate(string? code, ref IEvaluationEnvironment? environment)
        {
            var tokens = _tokenizer.Tokenize(code);
            var programAST = _parser.Parse(tokens);

            return Evaluate(programAST, ref environment);
        }

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
        public IEnumerable<EvaluationResult> Evaluate(Program program, ref IEvaluationEnvironment? environment)
        {
            environment ??= CreateGlobalEnvironment();

            return _evaluator.EvaluateProgram(program, environment);
        }

        /// <summary>
        /// Reads, evaluates and prints the results safely and indefinitely. It facilitates a common REPL (Read Eval Print Loop) mechanism.
        /// </summary>
        /// <param name="read">A function that reads code from user input.</param>
        /// <param name="print">A function that prints an evaluation result.</param>
        /// <param name="printError">A function that prints an evaluation error.</param>
        /// <exception cref="ArgumentNullException">Any of the function arguments is null.</exception>
        public void ReadEvalPrintLoop(Func<string?> read, PrintResult print, PrintError printError)
        {
            ArgumentNullException.ThrowIfNull(read, nameof(read));
            ArgumentNullException.ThrowIfNull(print, nameof(print));
            ArgumentNullException.ThrowIfNull(printError, nameof(printError));

            var environment = CreateGlobalEnvironment();

            while (true)
            {
                EvaluateAndPrint(read(), ref environment, print, printError);
            }
        }

        /// <summary>
        /// Evaluates <paramref name="code"/> safely in the given evaluation <paramref name="environment"/> and forwards the results and possible errors to the given functions.
        /// Typically, the first time it is called, null is passed to <paramref name="environment"/> which creates one and returns it by reference.
        /// Subsequent calls should use that environment to progressively add more definitions to it.
        /// </summary>
        /// <param name="code">The LISP code to evaluate.</param>
        /// <param name="environment">The environment which the evaluation occurs in. If null a global environment is created and returned for subsequent evaluations.</param>
        /// <param name="print">A function that prints an evaluation result.</param>
        /// <param name="printError">A function that prints an evaluation error.</param>
        /// <exception cref="ArgumentNullException"><paramref name="print"/> and/or <paramref name="printError"/> is null.</exception>
        public void EvaluateAndPrint(string? code, ref IEvaluationEnvironment? environment, PrintResult print, PrintError printError)
        {
            ArgumentNullException.ThrowIfNull(print, nameof(print));
            ArgumentNullException.ThrowIfNull(printError, nameof(printError));

            environment ??= CreateGlobalEnvironment();

            try
            {
                var tokens = _tokenizer.Tokenize(code);
                var programAST = _parser.Parse(tokens);

                var results = _evaluator.EvaluateProgram(programAST, environment);
                foreach (var result in results)
                {
                    string resultToPrint = result.AcceptVisitor(_resultPrinter);
                    print(resultToPrint);
                }
            }
            catch (NonTerminatedStringException ex)
            {
                printError(string.Format(ErrorMessages.NON_TERMINATED_STRING, ex.Line, ex.Position), ex);
            }
            catch (InvalidEscapeSequenceException ex)
            {
                printError(string.Format(ErrorMessages.INVALID_ESCAPE_SEQUENCE, ex.Character, ex.Line, ex.Position), ex);
            }
            catch (InvalidStringCharacterException ex)
            {
                printError(string.Format(ErrorMessages.INVALID_STRING_CHARACTER, ex.Character, ex.Line, ex.Position), ex);
            }
            catch (InvalidNumberCharacterException ex)
            {
                printError(string.Format(ErrorMessages.INVALID_NUMBER_CHARACTER, ex.Character, ex.Line, ex.Position), ex);
            }
            catch (ParsingException ex)
            {
                string expectedSymbols = string.Join(ErrorMessages.SYMBOL_SEPARATOR, ex.MissingSymbols.Select(t => t.ToWord()));
                string message = string.Format(ErrorMessages.PARSING_ERROR, ex.Line, ex.Position, expectedSymbols);

                printError(message, ex);
            }
            catch (WrongNumberOfArgumentsException ex)
            {
                string message = string.Format(ex.OrMore ? ErrorMessages.WRONG_NUMBER_OF_ARGUMENTS_EXT : ErrorMessages.WRONG_NUMBER_OF_ARGUMENTS, ex.Expected, ex.Actual);
                printError(AppendDebugInfo(message, ex), ex);
            }
            catch (WrongTypeOfArgumentException ex)
            {
                string message;

                // It does not help the user if IPrimitiveDatum is reported. Report its concrete classes instead.
                var expectedTypes = ex.Expected;
                if (_primitiveTypes != null && _primitiveTypes.Length > 0)
                    expectedTypes = expectedTypes.SelectMany(t => t == typeof(IPrimitiveDatum) ? _primitiveTypes : Enumerable.Repeat(t, 1)).ToArray();

                if (expectedTypes.Length > 1)
                    message = string.Format(ErrorMessages.WRONG_TYPE_OF_ARGUMENT_FOR_MANY, string.Join(", ", expectedTypes.Select(t => t.Name)), ex.Actual.Name);
                else
                    message = string.Format(ErrorMessages.WRONG_TYPE_OF_ARGUMENT, expectedTypes[0].Name, ex.Actual.Name);

                printError(AppendDebugInfo(message, ex), ex);
            }
            catch (UnknownIdentifierException ex)
            {
                string message = string.Format(ErrorMessages.UNKNOWN_IDENTIFIER, ex.IdentifierName);
                printError(AppendDebugInfo(message, ex), ex);
            }
            catch (NotProcedureException ex)
            {
                printError(AppendDebugInfo(ErrorMessages.NOT_PROCEDURE, ex), ex);
            }
            catch (UserException ex)
            {
                string message = string.Format(ErrorMessages.USER_ERROR, ex.Message);
                printError(AppendDebugInfo(message, ex), ex);
            }
            catch (InvalidValueException ex)
            {
                printError(AppendDebugInfo(ErrorMessages.INVALID_VALUE, ex), ex);
            }
            catch (Exception ex)
            {
                printError(ErrorMessages.UNKNOWN_ERROR, ex, true);
            }
        }

        private IEvaluationEnvironment CreateGlobalEnvironment()
        {
            _mostRecentGlobalEnvironment = EnvironmentFactory?.CreateEnvironment() ?? new EvaluationEnvironment();

            void Install(string procedureName, Func<EvaluationResult[]?, EvaluationResult> implementation)
                => _mostRecentGlobalEnvironment.DefineIdentifier(procedureName, new PrimitiveProcedure(procedureName, implementation));

            // Define primitive procedures.
            Install(PLUS, PrimitiveOperation.Add);
            Install(MINUS, PrimitiveOperation.Subtract);
            Install(MULTIPLY, PrimitiveOperation.Multiply);
            Install(DIVIDE, PrimitiveOperation.Divide);
            Install(MODULO, PrimitiveOperation.Modulo);
            Install(GREATER, PrimitiveOperation.GreaterThan);
            Install(GREATER_OR_EQUAL, PrimitiveOperation.GreaterThanOrEqualTo);
            Install(LESS, PrimitiveOperation.LessThan);
            Install(LESS_OR_EQUAL, PrimitiveOperation.LessThanOrEqualTo);
            Install(EQUAL, PrimitiveOperation.EqualTo);
            Install(XOR, PrimitiveOperation.LogicalXor);
            Install(NOT, PrimitiveOperation.LogicalNot);
            Install(CONS, PrimitiveOperation.Cons);
            Install(CAR, PrimitiveOperation.Car);
            Install(CDR, PrimitiveOperation.Cdr);
            Install(IS_PAIR, PrimitiveOperation.IsPair);
            Install(LIST, PrimitiveOperation.List);
            Install(IS_NULL, PrimitiveOperation.IsNull);
            Install(SET_CAR, PrimitiveOperation.SetCar);
            Install(SET_CDR, PrimitiveOperation.SetCdr);
            Install(STRING_LENGTH, PrimitiveOperation.StringLength);
            Install(SUBSTRING, PrimitiveOperation.Substring);
            Install(INDEX_OF_STRING, PrimitiveOperation.IndexOfString);
            Install(REPLACE_STRING, PrimitiveOperation.ReplaceString);
            Install(TO_LOWER, PrimitiveOperation.ToLower);
            Install(TO_UPPER, PrimitiveOperation.ToUpper);
            Install(DISPLAY, PrimitiveOperation.Display);
            Install(DEBUG, PrimitiveOperation.Debug);
            Install(TRACE, PrimitiveOperation.Trace);
            Install(NEW_LINE, PrimitiveOperation.NewLine);
            Install(ERROR, PrimitiveOperation.Error);
            Install(NOW, PrimitiveOperation.Now);
            Install(UTC_NOW, PrimitiveOperation.UtcNow);
            Install(MAKE_DATE, PrimitiveOperation.MakeDate);
            Install(MAKE_DATETIME, PrimitiveOperation.MakeDateTime);
            Install(YEAR, PrimitiveOperation.Year);
            Install(MONTH, PrimitiveOperation.Month);
            Install(DAY, PrimitiveOperation.Day);
            Install(HOUR, PrimitiveOperation.Hour);
            Install(MINUTE, PrimitiveOperation.Minute);
            Install(SECOND, PrimitiveOperation.Second);
            Install(MILLISECOND, PrimitiveOperation.Millisecond);
            Install(IS_UTC, PrimitiveOperation.IsUtc);
            Install(TO_LOCAL, PrimitiveOperation.ToLocal);
            Install(TO_UTC, PrimitiveOperation.ToUtc);
            Install(ADD_YEARS, PrimitiveOperation.AddYears);
            Install(ADD_MONTHS, PrimitiveOperation.AddMonths);
            Install(ADD_DAYS, PrimitiveOperation.AddDays);
            Install(ADD_HOURS, PrimitiveOperation.AddHours);
            Install(ADD_MINUTES, PrimitiveOperation.AddMinutes);
            Install(ADD_SECONDS, PrimitiveOperation.AddSeconds);
            Install(ADD_MILLISECONDS, PrimitiveOperation.AddMilliseconds);
            Install(DAYS_DIFF, PrimitiveOperation.DaysDiff);
            Install(HOURS_DIFF, PrimitiveOperation.HoursDiff);
            Install(MINUTES_DIFF, PrimitiveOperation.MinutesDiff);
            Install(SECONDS_DIFF, PrimitiveOperation.SecondsDiff);
            Install(MILLISECONDS_DIFF, PrimitiveOperation.MillisecondsDiff);
            Install(PARSE_DATETIME, PrimitiveOperation.ParseDateTime);
            Install(DATETIME_TO_STRING, PrimitiveOperation.DateTimeToString);
            Install(NUMBER_TO_STRING, PrimitiveOperation.NumberToString);
            Install(PARSE_NUMBER, PrimitiveOperation.ParseNumber);
            Install(ROUND, PrimitiveOperation.Round);

            _mostRecentGlobalEnvironment.DefineIdentifier(EVAL, new PrimitiveProcedure(EVAL, values => PrimitiveOperation.Eval(values, EvalPrimitiveCallback)));
            _mostRecentGlobalEnvironment.DefineIdentifier(NIL, Nil.GetNil());

            // Install common libraries in the environment.
            if (_installLispLibraries)
            {
                IEvaluationEnvironment? env = _mostRecentGlobalEnvironment;
                // Force enumeration, so that all definitions are installed.
                _ = Evaluate(LispLibraries.LIBS, ref env).LastOrDefault();
            }

            return _mostRecentGlobalEnvironment;
        }

        private EvaluationResult EvalPrimitiveCallback(EvaluationResult result)
        {
            string code = result.AcceptVisitor(_resultPrinter);
            return Evaluate(code, ref _mostRecentGlobalEnvironment).Last();
        }

        private static string AppendDebugInfo(string message, EvaluationException exception)
        {
            if (exception.CallStack == null || exception.CallStack.Count == 0)
                return message;

            var callStackLines = exception.CallStack.Select(dbg => string.Format(ErrorMessages.DEBUG_INFO, dbg, dbg.Line, dbg.Position));
            string debugInfoText = string.Join(Environment.NewLine, callStackLines);

            return $"{message}{Environment.NewLine}{ErrorMessages.CALL_STACK}{Environment.NewLine}{debugInfoText}";
        }

        private static void LoadPrimitiveTypes()
        {
            // Load this information just once using reflection to avoid future maintenance.
            if (_primitiveTypes != null)
                return;

            Type primitiveBaseType = typeof(IPrimitiveDatum);
            _primitiveTypes = Assembly.GetAssembly(primitiveBaseType)
                ?.GetTypes()
                .Where(t => primitiveBaseType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
        }
    }
}
