using RainLisp.Environment;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLisp.AbstractSyntaxTree
{
    public class Quote : Expression
    {
        public Quote(string text)
            => Text = text;

        public string Text { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateQuote(this);
    }
}
