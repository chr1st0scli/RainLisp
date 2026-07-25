using RainLisp;
using Xunit.Abstractions;

namespace RainLispTests
{
    public class TestDebugInfo(uint line, uint position) : IDebugInfo, IXunitSerializable
    {
        public TestDebugInfo() : this(0, 0)
        {
        }

        public uint Line { get; set; } = line;

        public uint Position { get; set; } = position;

        public bool HasDebugInfo { get; set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Line = info.GetValue<uint>(nameof(Line));
            Position = info.GetValue<uint>(nameof(Position));
            HasDebugInfo = info.GetValue<bool>(nameof(HasDebugInfo));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Line), Line);
            info.AddValue(nameof(Position), Position);
            info.AddValue(nameof(HasDebugInfo), HasDebugInfo);
        }
    }
}
