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

        public Interpreter(ITokenizer? tokenizer = null, IParser? parser = null, IEvaluatorVisitor? evaluator = null)
        {
            this.tokenizer = tokenizer ?? new Tokenizer();
            this.parser = parser ?? new Parser();
            this.evaluator = evaluator ?? new EvaluatorVisitor(new ProcedureApplicationVisitor());
        }

        public object Evaluate(string expression)
            => Evaluate(expression, out EvaluationEnvironment _);

        public object Evaluate(string expression, out EvaluationEnvironment environment)
        {
            var tokens = tokenizer.Tokenize(expression);
            var programAST = parser.Parse(tokens);

            environment = CreateGlobalEnvironment();
            return evaluator.EvaluateProgram(programAST, environment);
        }

        private static EvaluationEnvironment CreateGlobalEnvironment()
        {
            var environment = new EvaluationEnvironment();

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
            environment.DefineIdentifier(AND, new PrimitiveProcedure(PrimitiveProcedureType.LogicalAnd));
            environment.DefineIdentifier(OR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalOr));
            environment.DefineIdentifier(XOR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalXor));
            environment.DefineIdentifier(NOT, new PrimitiveProcedure(PrimitiveProcedureType.LogicalNot));

            return environment;
        }
    }
}
