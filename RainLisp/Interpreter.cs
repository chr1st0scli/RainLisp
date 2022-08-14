using RainLisp.Evaluation;
using RainLisp.Parsing;
using RainLisp.Tokenization;

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
            EvaluationEnvironment.ResetGlobalEnvironment();

            var tokens = tokenizer.Tokenize(expression);
            var programAST = parser.Parse(tokens);

            return evaluator.EvaluateProgram(programAST);
        }
    }
}
