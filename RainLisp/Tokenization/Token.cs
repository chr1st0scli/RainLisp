namespace RainLisp.Tokenization
{
    public class Token
    {
        public TokenType Type { get; set; }

        public string Value { get; set; } = string.Empty;

        public uint LineNumber { get; set; }

        public uint Position { get; set; }
    }
}
