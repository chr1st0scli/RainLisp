﻿namespace RainLisp.AbstractSyntaxTree
{
    public class Begin : Expression
    {
        public Begin(IList<Expression> expressions)
        {
            Expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
        }

        public IList<Expression> Expressions { get; init; }

        public override object AcceptVisitor(IVisitor visitor, Environment environment)
            => visitor.VisitBegin(this, environment);
    }
}
