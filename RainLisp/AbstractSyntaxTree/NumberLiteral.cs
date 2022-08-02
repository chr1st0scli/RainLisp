﻿namespace RainLisp.AbstractSyntaxTree
{
    public class NumberLiteral : Expression
    {
        public NumberLiteral(double value) => Value = value;

        public double Value { get; init; }

        public override void AcceptVisitor(IVisitor visitor)
            => visitor.VisitNumberLiteral(this);
    }
}
