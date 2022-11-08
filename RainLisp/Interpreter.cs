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

        public void ReadEvalPrintLoop(Func<string?> read, Action<string> print)
        {
            ArgumentNullException.ThrowIfNull(read, nameof(read));
            ArgumentNullException.ThrowIfNull(print, nameof(print));

            void PrintError(string message, Exception ex)
            {
                print(message);
                // Print the exception's message if one provided.
                if (!string.IsNullOrWhiteSpace(ex.Message) && !ex.Message.StartsWith("Exception of type"))
                    print(ex.Message);
            }

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
                    PrintError(string.Format(ErrorMessages.NON_TERMINATED_STRING, ex.Line, ex.Position), ex);
                }
                catch (InvalidEscapeSequenceException ex)
                {
                    PrintError(string.Format(ErrorMessages.INVALID_ESCAPE_SEQUENCE, ex.Character, ex.Line, ex.Position), ex);
                }
                catch (InvalidStringCharacterException ex)
                {
                    PrintError(string.Format(ErrorMessages.INVALID_STRING_CHARACTER, ex.Character, ex.Line, ex.Position), ex);
                }
                catch (ParsingException ex)
                {
                    PrintError(string.Format(ErrorMessages.PARSING_ERROR, ex.Line, ex.Position), ex);
                }
                catch (WrongNumberOfArgumentsException ex)
                {
                    PrintError(string.Format(ex.OrMore ? ErrorMessages.WRONG_NUMBER_OF_ARGUMENTS_EXT : ErrorMessages.WRONG_NUMBER_OF_ARGUMENTS, ex.Expected, ex.Actual), ex);
                }
                catch (WrongTypeOfArgumentException ex)
                {
                    PrintError(string.Format(ErrorMessages.WRONG_TYPE_OF_ARGUMENT, ex.Expected.Name, ex.Actual.Name), ex);
                }
                catch (UnknownIdentifierException ex)
                {
                    PrintError(string.Format(ErrorMessages.UNKNOWN_IDENTIFIER, ex.IdentifierName), ex);
                }
                catch (NotProcedureException ex)
                {
                    PrintError(ErrorMessages.NOT_PROCEDURE, ex);
                }
                catch (UserException ex)
                {
                    PrintError(string.Format(ErrorMessages.USER_ERROR, ex.Message), ex);
                }
                catch (Exception ex)
                {
                    print(ex.ToString());
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
