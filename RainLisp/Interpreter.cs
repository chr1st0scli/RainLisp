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

        public Interpreter(ITokenizer? tokenizer = null, IParser? parser = null, IEvaluatorVisitor? evaluator = null, IEnvironmentFactory? environmentFactory = null, IEvaluationResultVisitor<string>? resultPrinter = null, bool installLispLibraries = true)
        {
            _tokenizer = tokenizer ?? new Tokenizer();
            _parser = parser ?? new Parser();
            _evaluator = evaluator ?? new EvaluatorVisitor(new ProcedureApplicationVisitor(EvalPrimitiveCallback));
            _environmentFactory = environmentFactory;
            _resultPrinter = resultPrinter ?? new EvaluationResultPrintVisitor();
            _installLispLibraries = installLispLibraries;
            LoadPrimitiveTypes();
        }

        public IEnumerable<EvaluationResult> Evaluate(string? expression)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(expression, ref environment);
        }

        public IEnumerable<EvaluationResult> Evaluate(Program program)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(program, ref environment);
        }

        public IEnumerable<EvaluationResult> Evaluate(string? expression, ref IEvaluationEnvironment? environment)
        {
            var tokens = _tokenizer.Tokenize(expression);
            var programAST = _parser.Parse(tokens);

            return Evaluate(programAST, ref environment);
        }

        public IEnumerable<EvaluationResult> Evaluate(Program program, ref IEvaluationEnvironment? environment)
        {
            environment ??= CreateGlobalEnvironment();

            return _evaluator.EvaluateProgram(program, environment);
        }

        public void ReadEvalPrintLoop(Func<string?> read, Action<string> print, Action<string, Exception> printError)
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

        public void EvaluateAndPrint(string? expression, ref IEvaluationEnvironment? environment, Action<string> print, Action<string, Exception> printError)
        {
            ArgumentNullException.ThrowIfNull(print, nameof(print));
            ArgumentNullException.ThrowIfNull(printError, nameof(printError));

            environment ??= CreateGlobalEnvironment();

            try
            {
                var tokens = _tokenizer.Tokenize(expression);
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
                printError(ErrorMessages.UNKNOWN_ERROR, ex);
            }
        }

        private IEvaluationEnvironment CreateGlobalEnvironment()
        {
            _mostRecentGlobalEnvironment = _environmentFactory?.CreateEnvironment() ?? new EvaluationEnvironment();

            void Install(string procedureName, PrimitiveProcedureType procedureType)
                => _mostRecentGlobalEnvironment.DefineIdentifier(procedureName, new PrimitiveProcedure(procedureType));

            // Define primitive procedures.
            Install(PLUS, PrimitiveProcedureType.Add);
            Install(MINUS, PrimitiveProcedureType.Subtract);
            Install(MULTIPLY, PrimitiveProcedureType.Multiply);
            Install(DIVIDE, PrimitiveProcedureType.Divide);
            Install(MODULO, PrimitiveProcedureType.Modulo);
            Install(GREATER, PrimitiveProcedureType.GreaterThan);
            Install(GREATER_OR_EQUAL, PrimitiveProcedureType.GreaterThanOrEqualTo);
            Install(LESS, PrimitiveProcedureType.LessThan);
            Install(LESS_OR_EQUAL, PrimitiveProcedureType.LessThanOrEqualTo);
            Install(EQUAL, PrimitiveProcedureType.EqualTo);
            Install(XOR, PrimitiveProcedureType.LogicalXor);
            Install(NOT, PrimitiveProcedureType.LogicalNot);
            Install(CONS, PrimitiveProcedureType.Cons);
            Install(CAR, PrimitiveProcedureType.Car);
            Install(CDR, PrimitiveProcedureType.Cdr);
            Install(IS_PAIR, PrimitiveProcedureType.IsPair);
            Install(LIST, PrimitiveProcedureType.List);
            Install(IS_NULL, PrimitiveProcedureType.IsNull);
            Install(SET_CAR, PrimitiveProcedureType.SetCar);
            Install(SET_CDR, PrimitiveProcedureType.SetCdr);
            Install(STRING_LENGTH, PrimitiveProcedureType.StringLength);
            Install(SUBSTRING, PrimitiveProcedureType.Substring);
            Install(INDEX_OF_STRING, PrimitiveProcedureType.IndexOfString);
            Install(REPLACE_STRING, PrimitiveProcedureType.ReplaceString);
            Install(TO_LOWER, PrimitiveProcedureType.ToLower);
            Install(TO_UPPER, PrimitiveProcedureType.ToUpper);
            Install(DISPLAY, PrimitiveProcedureType.Display);
            Install(DEBUG, PrimitiveProcedureType.Debug);
            Install(TRACE, PrimitiveProcedureType.Trace);
            Install(NEW_LINE, PrimitiveProcedureType.NewLine);
            Install(ERROR, PrimitiveProcedureType.Error);
            Install(NOW, PrimitiveProcedureType.Now);
            Install(UTC_NOW, PrimitiveProcedureType.UtcNow);
            Install(MAKE_DATE, PrimitiveProcedureType.MakeDate);
            Install(MAKE_DATETIME, PrimitiveProcedureType.MakeDateTime);
            Install(YEAR, PrimitiveProcedureType.Year);
            Install(MONTH, PrimitiveProcedureType.Month);
            Install(DAY, PrimitiveProcedureType.Day);
            Install(HOUR, PrimitiveProcedureType.Hour);
            Install(MINUTE, PrimitiveProcedureType.Minute);
            Install(SECOND, PrimitiveProcedureType.Second);
            Install(MILLISECOND, PrimitiveProcedureType.Millisecond);
            Install(IS_UTC, PrimitiveProcedureType.IsUtc);
            Install(TO_LOCAL, PrimitiveProcedureType.ToLocal);
            Install(TO_UTC, PrimitiveProcedureType.ToUtc);
            Install(ADD_YEARS, PrimitiveProcedureType.AddYears);
            Install(ADD_MONTHS, PrimitiveProcedureType.AddMonths);
            Install(ADD_DAYS, PrimitiveProcedureType.AddDays);
            Install(ADD_HOURS, PrimitiveProcedureType.AddHours);
            Install(ADD_MINUTES, PrimitiveProcedureType.AddMinutes);
            Install(ADD_SECONDS, PrimitiveProcedureType.AddSeconds);
            Install(ADD_MILLISECONDS, PrimitiveProcedureType.AddMilliseconds);
            Install(DAYS_DIFF, PrimitiveProcedureType.DaysDiff);
            Install(HOURS_DIFF, PrimitiveProcedureType.HoursDiff);
            Install(MINUTES_DIFF, PrimitiveProcedureType.MinutesDiff);
            Install(SECONDS_DIFF, PrimitiveProcedureType.SecondsDiff);
            Install(MILLISECONDS_DIFF, PrimitiveProcedureType.MillisecondsDiff);
            Install(PARSE_DATETIME, PrimitiveProcedureType.ParseDateTime);
            Install(DATETIME_TO_STRING, PrimitiveProcedureType.DateTimeToString);
            Install(NUMBER_TO_STRING, PrimitiveProcedureType.NumberToString);
            Install(PARSE_NUMBER, PrimitiveProcedureType.ParseNumber);
            Install(ROUND, PrimitiveProcedureType.Round);
            Install(EVAL, PrimitiveProcedureType.Eval);

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
