namespace RainLispConsole.CodeTextView
{
    internal readonly struct LineInfo
    {
        public string Text { get; init; }

        public int CommentStart { get; init; }

        public WordInfo[] Words { get; init; }

        public StringInfo[] Strings { get; init; }
    }
}
