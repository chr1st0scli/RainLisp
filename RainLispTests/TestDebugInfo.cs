using RainLisp;

namespace RainLispTests
{
    internal class TestDebugInfo(uint line, uint position) : IDebugInfo
    {
        public uint Line { get; set; } = line;

        public uint Position { get; set; } = position;

        public bool HasDebugInfo { get; set; }
    }
}
