﻿using RainLisp.Environment;
using RainLisp.Evaluation;

namespace RainLisp.AbstractSyntaxTree
{
    public class Begin : Expression
    {
        public Begin(IList<Expression> expressions)
            => Expressions = expressions;

        public IList<Expression> Expressions { get; init; }

        public override EvaluationResult AcceptVisitor(IEvaluatorVisitor visitor, IEvaluationEnvironment environment)
            => visitor.EvaluateBegin(this, environment);
    }
}
