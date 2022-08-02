﻿namespace RainLisp.AbstractSyntaxTree
{
    public class Identifier : Expression
    {
        public Identifier(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; init; }

        public override void AcceptVisitor(IVisitor visitor)
            => visitor.VisitIdentifier(this);
    }
}
