﻿using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Quote : Expression
    {
        public Quote(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; init; } = string.Empty;

        public override object AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateQuote(this);
    }
}
