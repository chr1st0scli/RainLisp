using RainLisp.AbstractSyntaxTree;
using RainLisp.Environment;
using RainLisp.Evaluation;
using RainLisp.Parsing;
using RainLisp.Tokenization;
using static RainLisp.Grammar.Primitives;

namespace RainLisp
{
    public class Interpreter
    {
        private readonly ITokenizer tokenizer;
        private readonly IParser parser;
        private readonly IEvaluatorVisitor evaluator;
        private readonly IEnvironmentFactory? environmentFactory;

        public Interpreter(ITokenizer? tokenizer = null, IParser? parser = null, IEvaluatorVisitor? evaluator = null, IEnvironmentFactory? environmentFactory = null)
        {
            this.tokenizer = tokenizer ?? new Tokenizer();
            this.parser = parser ?? new Parser();
            this.evaluator = evaluator ?? new EvaluatorVisitor(new ProcedureApplicationVisitor());
            this.environmentFactory = environmentFactory;
        }

        public object Evaluate(string expression)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(expression, ref environment);
        }

        public object Evaluate(Program program)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(program, ref environment);
        }

        public object Evaluate(string expression, ref IEvaluationEnvironment? environment)
        {
            var tokens = tokenizer.Tokenize(expression);
            var programAST = parser.Parse(tokens);

            return Evaluate(programAST, ref environment);
        }

        public object Evaluate(Program program, ref IEvaluationEnvironment? environment)
        {
            environment ??= CreateGlobalEnvironment();

            return evaluator.EvaluateProgram(program, environment);
        }

        public void ReadEvalPrintLoop(Func<string> read, Action<string> print)
        {
            ArgumentNullException.ThrowIfNull(read, nameof(read));
            ArgumentNullException.ThrowIfNull(print, nameof(print));

            var environment = CreateGlobalEnvironment();

            while (true)
            {
                try
                {
                    string expression = read();

                    var tokens = tokenizer.Tokenize(expression);
                    var programAST = parser.Parse(tokens);

                    var result = evaluator.EvaluateProgram(programAST, environment);

                    print(result.ToString()!);
                }
                catch (Exception ex)
                {
                    print(ex.ToString());
                }
            }
        }

        private IEvaluationEnvironment CreateGlobalEnvironment()
        {
            var environment = environmentFactory?.CreateEnvironment() ?? new EvaluationEnvironment();

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
            environment.DefineIdentifier(AND, new PrimitiveProcedure(PrimitiveProcedureType.LogicalAnd));
            environment.DefineIdentifier(OR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalOr));
            environment.DefineIdentifier(XOR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalXor));
            environment.DefineIdentifier(NOT, new PrimitiveProcedure(PrimitiveProcedureType.LogicalNot));

            return environment;
        }
    }
}
