using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Parsing;
using RainLisp.Tokenization;
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

        public Interpreter(ITokenizer? tokenizer = null, IParser? parser = null, IEvaluatorVisitor? evaluator = null, IEnvironmentFactory? environmentFactory = null, IEvaluationResultVisitor<string>? resultPrinter = null, bool installLispLibraries = true)
        {
            _tokenizer = tokenizer ?? new Tokenizer();
            _parser = parser ?? new Parser();
            _evaluator = evaluator ?? new EvaluatorVisitor(new ProcedureApplicationVisitor());
            _environmentFactory = environmentFactory;
            _resultPrinter = resultPrinter ?? new EvaluationResultPrintVisitor();
            _installLispLibraries = installLispLibraries;
        }

        public EvaluationResult Evaluate(string expression)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(expression, ref environment);
        }

        public EvaluationResult Evaluate(Program program)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(program, ref environment);
        }

        public EvaluationResult Evaluate(string expression, ref IEvaluationEnvironment? environment)
        {
            var tokens = _tokenizer.Tokenize(expression);
            var programAST = _parser.Parse(tokens);

            return Evaluate(programAST, ref environment);
        }

        public EvaluationResult Evaluate(Program program, ref IEvaluationEnvironment? environment)
        {
            environment ??= CreateGlobalEnvironment();

            return _evaluator.EvaluateProgram(program, environment);
        }

        public void ReadEvalPrintLoop(Func<string?> read, Action<string> print, Action<string, Exception> printError)
        {
            ArgumentNullException.ThrowIfNull(read, nameof(read));
            ArgumentNullException.ThrowIfNull(print, nameof(print));

            var environment = CreateGlobalEnvironment();

            while (true)
            {
                try
                {
                    string? expression = read();

                    var tokens = _tokenizer.Tokenize(expression);
                    var programAST = _parser.Parse(tokens);

                    var result = _evaluator.EvaluateProgram(programAST, environment);
                    string resultToPrint = result.AcceptVisitor(_resultPrinter);

                    print(resultToPrint);
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
                catch (ParsingException ex)
                {
                    printError(string.Format(ErrorMessages.PARSING_ERROR, ex.Line, ex.Position), ex);
                }
                catch (WrongNumberOfArgumentsException ex)
                {
                    printError(string.Format(ex.OrMore ? ErrorMessages.WRONG_NUMBER_OF_ARGUMENTS_EXT : ErrorMessages.WRONG_NUMBER_OF_ARGUMENTS, ex.Expected, ex.Actual), ex);
                }
                catch (WrongTypeOfArgumentException ex)
                {
                    printError(string.Format(ErrorMessages.WRONG_TYPE_OF_ARGUMENT, ex.Expected.Name, ex.Actual.Name), ex);
                }
                catch (UnknownIdentifierException ex)
                {
                    printError(string.Format(ErrorMessages.UNKNOWN_IDENTIFIER, ex.IdentifierName), ex);
                }
                catch (NotProcedureException ex)
                {
                    printError(ErrorMessages.NOT_PROCEDURE, ex);
                }
                catch (UserException ex)
                {
                    printError(string.Format(ErrorMessages.USER_ERROR, ex.Message), ex);
                }
                catch (Exception ex)
                {
                    printError(ErrorMessages.UNKNOWN_ERROR, ex);
                }
            }
        }

        private IEvaluationEnvironment CreateGlobalEnvironment()
        {
            var environment = _environmentFactory?.CreateEnvironment() ?? new EvaluationEnvironment();

            // Define primitive procedures.
            environment.DefineIdentifier(PLUS, new PrimitiveProcedure(PrimitiveProcedureType.Add));
            environment.DefineIdentifier(MINUS, new PrimitiveProcedure(PrimitiveProcedureType.Subtract));
            environment.DefineIdentifier(MULTIPLY, new PrimitiveProcedure(PrimitiveProcedureType.Multiply));
            environment.DefineIdentifier(DIVIDE, new PrimitiveProcedure(PrimitiveProcedureType.Divide));
            environment.DefineIdentifier(MODULO, new PrimitiveProcedure(PrimitiveProcedureType.Modulo));
            environment.DefineIdentifier(GREATER, new PrimitiveProcedure(PrimitiveProcedureType.GreaterThan));
            environment.DefineIdentifier(GREATER_OR_EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.GreaterThanOrEqualTo));
            environment.DefineIdentifier(LESS, new PrimitiveProcedure(PrimitiveProcedureType.LessThan));
            environment.DefineIdentifier(LESS_OR_EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.LessThanOrEqualTo));
            environment.DefineIdentifier(EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.EqualTo));
            environment.DefineIdentifier(XOR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalXor));
            environment.DefineIdentifier(NOT, new PrimitiveProcedure(PrimitiveProcedureType.LogicalNot));
            environment.DefineIdentifier(CONS, new PrimitiveProcedure(PrimitiveProcedureType.Cons));
            environment.DefineIdentifier(CAR, new PrimitiveProcedure(PrimitiveProcedureType.Car));
            environment.DefineIdentifier(CDR, new PrimitiveProcedure(PrimitiveProcedureType.Cdr));
            environment.DefineIdentifier(LIST, new PrimitiveProcedure(PrimitiveProcedureType.List));
            environment.DefineIdentifier(IS_NULL, new PrimitiveProcedure(PrimitiveProcedureType.IsNull));
            environment.DefineIdentifier(SET_CAR, new PrimitiveProcedure(PrimitiveProcedureType.SetCar));
            environment.DefineIdentifier(SET_CDR, new PrimitiveProcedure(PrimitiveProcedureType.SetCdr));
            environment.DefineIdentifier(DISPLAY, new PrimitiveProcedure(PrimitiveProcedureType.Display));
            environment.DefineIdentifier(DEBUG, new PrimitiveProcedure(PrimitiveProcedureType.Debug));
            environment.DefineIdentifier(TRACE, new PrimitiveProcedure(PrimitiveProcedureType.Trace));
            environment.DefineIdentifier(NEW_LINE, new PrimitiveProcedure(PrimitiveProcedureType.NewLine));
            environment.DefineIdentifier(ERROR, new PrimitiveProcedure(PrimitiveProcedureType.Error));
            environment.DefineIdentifier(NOW, new PrimitiveProcedure(PrimitiveProcedureType.Now));
            environment.DefineIdentifier(UTC_NOW, new PrimitiveProcedure(PrimitiveProcedureType.UtcNow));
            environment.DefineIdentifier(MAKE_DATE, new PrimitiveProcedure(PrimitiveProcedureType.MakeDate));
            environment.DefineIdentifier(MAKE_DATE_TIME, new PrimitiveProcedure(PrimitiveProcedureType.MakeDateTime));
            environment.DefineIdentifier(YEAR, new PrimitiveProcedure(PrimitiveProcedureType.Year));
            environment.DefineIdentifier(MONTH, new PrimitiveProcedure(PrimitiveProcedureType.Month));
            environment.DefineIdentifier(DAY, new PrimitiveProcedure(PrimitiveProcedureType.Day));
            environment.DefineIdentifier(HOUR, new PrimitiveProcedure(PrimitiveProcedureType.Hour));
            environment.DefineIdentifier(MINUTE, new PrimitiveProcedure(PrimitiveProcedureType.Minute));
            environment.DefineIdentifier(SECOND, new PrimitiveProcedure(PrimitiveProcedureType.Second));
            environment.DefineIdentifier(MILLISECOND, new PrimitiveProcedure(PrimitiveProcedureType.Millisecond));
            environment.DefineIdentifier(IS_UTC, new PrimitiveProcedure(PrimitiveProcedureType.IsUtc));
            environment.DefineIdentifier(TO_LOCAL, new PrimitiveProcedure(PrimitiveProcedureType.ToLocal));
            environment.DefineIdentifier(TO_UTC, new PrimitiveProcedure(PrimitiveProcedureType.ToUtc));
            environment.DefineIdentifier(ADD_YEARS, new PrimitiveProcedure(PrimitiveProcedureType.AddYears));
            environment.DefineIdentifier(ADD_MONTHS, new PrimitiveProcedure(PrimitiveProcedureType.AddMonths));
            environment.DefineIdentifier(ADD_DAYS, new PrimitiveProcedure(PrimitiveProcedureType.AddDays));
            environment.DefineIdentifier(ADD_HOURS, new PrimitiveProcedure(PrimitiveProcedureType.AddHours));
            environment.DefineIdentifier(ADD_MINUTES, new PrimitiveProcedure(PrimitiveProcedureType.AddMinutes));
            environment.DefineIdentifier(ADD_SECONDS, new PrimitiveProcedure(PrimitiveProcedureType.AddSeconds));
            environment.DefineIdentifier(ADD_MILLISECONDS, new PrimitiveProcedure(PrimitiveProcedureType.AddMilliseconds));
            environment.DefineIdentifier(DAYS_DIFF, new PrimitiveProcedure(PrimitiveProcedureType.DaysDiff));
            environment.DefineIdentifier(HOURS_DIFF, new PrimitiveProcedure(PrimitiveProcedureType.HoursDiff));
            environment.DefineIdentifier(MINUTES_DIFF, new PrimitiveProcedure(PrimitiveProcedureType.MinutesDiff));
            environment.DefineIdentifier(SECONDS_DIFF, new PrimitiveProcedure(PrimitiveProcedureType.SecondsDiff));
            environment.DefineIdentifier(MILLISECONDS_DIFF, new PrimitiveProcedure(PrimitiveProcedureType.MillisecondsDiff));
            environment.DefineIdentifier(PARSE_DATE_TIME, new PrimitiveProcedure(PrimitiveProcedureType.ParseDateTime));
            environment.DefineIdentifier(DATE_TIME_TO_STRING, new PrimitiveProcedure(PrimitiveProcedureType.DateTimeToString));

            environment.DefineIdentifier(NIL, Nil.GetNil());

            if (_installLispLibraries)
            {
                // Install common libraries in the environment.
                IEvaluationEnvironment? env = environment;
                Evaluate(CommonLibraries.LIBS, ref env);
            }

            return environment;
        }
    }
}
