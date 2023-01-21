﻿namespace RainLisp.AbstractSyntaxTree
{
    public abstract class Expression : Node, IDebugInfo
    {
        public uint Line { get; set; }

        public uint Position { get; set; }

        public bool HasDebugInfo { get; set; }
    }
}
