﻿namespace RainLispConsole.CodeTextView
{
    internal readonly struct LineInfo
    {
        public string Text { get; init; }

        public WordInfo[] Words { get; init; }
    }
}
