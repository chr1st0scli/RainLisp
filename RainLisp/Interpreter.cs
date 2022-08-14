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
        {
            ResetGlobalEnvironment();

            var tokens = tokenizer.Tokenize(expression);
            var programAST = parser.Parse(tokens);

            return evaluator.EvaluateProgram(programAST);
        }

        private void ResetGlobalEnvironment()
        {
            EvaluationEnvironment.ResetGlobalEnvironment();

            var globalEnvironment = EvaluationEnvironment.GlobalEnvironment;
            // Define primitive procedures.
            globalEnvironment.DefineIdentifier(PLUS, new PrimitiveProcedure(PrimitiveProcedureType.Add));
            globalEnvironment.DefineIdentifier(MINUS, new PrimitiveProcedure(PrimitiveProcedureType.Subtract));
            globalEnvironment.DefineIdentifier(MULTIPLY, new PrimitiveProcedure(PrimitiveProcedureType.Multiply));
            globalEnvironment.DefineIdentifier(DIVIDE, new PrimitiveProcedure(PrimitiveProcedureType.Divide));
            globalEnvironment.DefineIdentifier(MODULO, new PrimitiveProcedure(PrimitiveProcedureType.Modulo));
            globalEnvironment.DefineIdentifier(GREATER, new PrimitiveProcedure(PrimitiveProcedureType.GreaterThan));
            globalEnvironment.DefineIdentifier(GREATER_OR_EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.GreaterThanOrEqualTo));
            globalEnvironment.DefineIdentifier(LESS, new PrimitiveProcedure(PrimitiveProcedureType.LessThan));
            globalEnvironment.DefineIdentifier(LESS_OR_EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.LessThanOrEqualTo));
            globalEnvironment.DefineIdentifier(AND, new PrimitiveProcedure(PrimitiveProcedureType.LogicalAnd));
            globalEnvironment.DefineIdentifier(OR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalOr));
            globalEnvironment.DefineIdentifier(XOR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalXor));
            globalEnvironment.DefineIdentifier(NOT, new PrimitiveProcedure(PrimitiveProcedureType.LogicalNot));
        }
    }
}
