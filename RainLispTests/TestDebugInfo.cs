using RainLisp;

namespace RainLispTests
{
    internal class TestDebugInfo : IDebugInfo
    {
        public TestDebugInfo(uint line, uint position)
        {
            Line = line;
            Position = position;
        }

        public uint Line { get; set; }

        public uint Position { get; set; }

        public bool HasDebugInfo { get; set; }
    }
}
