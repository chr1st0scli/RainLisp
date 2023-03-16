using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Parsing;
using RainLisp.Tokenization;
using System.Reflection;
using static RainLisp.Grammar.Primitives;

namespace RainLisp
{
    public class Interpreter
    {
        private readonly ITokenizer _tokenizer;
        private readonly IParser _parser;
        private readonly IEvaluatorVisitor _evaluator;
        private readonly IEvaluationResultVisitor<string> _resultPrinter;
        private readonly IEnvironmentFactory? _environmentFactory;
        private readonly bool _installLispLibraries;

        private IEvaluationEnvironment? _mostRecentGlobalEnvironment;

        private static Type[]? _primitiveTypes;

        public delegate void PrintResult(string result);
        public delegate void PrintError(string message, Exception exception, bool unknownError = false);

        public Interpreter(ITokenizer? tokenizer = null, IParser? parser = null, IEvaluatorVisitor? evaluator = null, IEnvironmentFactory? environmentFactory = null, IEvaluationResultVisitor<string>? resultPrinter = null, bool installLispLibraries = true)
        {
            _tokenizer = tokenizer ?? new Tokenizer();
            _parser = parser ?? new Parser();
            _evaluator = evaluator ?? new EvaluatorVisitor(new ProcedureApplicationVisitor());
            _environmentFactory = environmentFactory;
            _resultPrinter = resultPrinter ?? new EvaluationResultPrintVisitor();
            _installLispLibraries = installLispLibraries;
            LoadPrimitiveTypes();
        }

        public IEnumerable<EvaluationResult> Evaluate(string? code)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(code, ref environment);
        }

        public IEnumerable<EvaluationResult> Evaluate(Program program)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(program, ref environment);
        }

        public IEnumerable<EvaluationResult> Evaluate(string? code, ref IEvaluationEnvironment? environment)
        {
            var tokens = _tokenizer.Tokenize(code);
            var programAST = _parser.Parse(tokens);

            return Evaluate(programAST, ref environment);
        }

        public IEnumerable<EvaluationResult> Evaluate(Program program, ref IEvaluationEnvironment? environment)
        {
            environment ??= CreateGlobalEnvironment();

            return _evaluator.EvaluateProgram(program, environment);
        }

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
            _mostRecentGlobalEnvironment = _environmentFactory?.CreateEnvironment() ?? new EvaluationEnvironment();

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
