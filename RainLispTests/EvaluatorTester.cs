using RainLisp;
using RainLisp.Evaluator;

namespace RainLispTests
{
    public class EvaluatorTester
    {
        private readonly IEvaluatorVisitor _evaluator;
        private readonly Parser _parser;

        public EvaluatorTester()
        {
            _evaluator = new EvaluatorVisitor(new ProcedureApplicationVisitor());
            _parser = new Parser();
        }

        protected object Evaluate(string expression)
        {
            RainLisp.Environment.ResetGlobalEnvironment();
            var tokens = Tokenizer.TokenizeExt(expression);
            var ast = _parser.Parse(tokens);
            return _evaluator.EvaluateProgram(ast);
        }
    }
}
