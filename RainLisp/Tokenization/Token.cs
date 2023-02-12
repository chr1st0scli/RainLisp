namespace RainLisp.Tokenization
{
    public class Token
    {
        public TokenType Type { get; init; }

        public string Value { get; init; } = string.Empty;

        public double NumberValue { get; init; }

        public bool BooleanValue { get; init; }

        public string StringValue { get; init; } = string.Empty;

        public uint Line { get; init; }

        public uint Position { get; init; }
    }
}
