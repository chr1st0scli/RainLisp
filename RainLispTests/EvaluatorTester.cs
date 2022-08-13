using RainLisp.Evaluation;
using RainLisp.Parsing;
using RainLisp.Tokenization;

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
            EvaluationEnvironment.ResetGlobalEnvironment();
            var tokens = Tokenizer.TokenizeExt(expression);
            var ast = _parser.Parse(tokens);
            return _evaluator.EvaluateProgram(ast);
        }
    }
}
