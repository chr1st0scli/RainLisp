namespace RainLisp
{
    public interface IDebugInfo
    {
        public uint Line { get; set; }

        public uint Position { get; set; }

        public bool HasDebugInfo { get; set; }
    }
}
