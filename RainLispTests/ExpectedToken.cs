using RainLisp.Tokenization;
using Xunit.Abstractions;

namespace RainLispTests
{
    public class ExpectedToken(TokenType tokenType, string value, uint position, uint line = 1, double numberValue = 0, bool booleanValue = false, string stringValue = "") : IXunitSerializable
    {
        public ExpectedToken() : this(TokenType.EOF, null!, 0)
        {    
        }

        public TokenType TokenType { get; set; } = tokenType;

        public string Value { get; set; } = value;

        public uint Position { get; set; } = position;

        public uint Line { get; set; } = line;

        public double NumberValue { get; set; } = numberValue;
        
        public bool BooleanValue { get; set; } = booleanValue;
        
        public string StringValue { get; set; } = stringValue;

        public void Deserialize(IXunitSerializationInfo info)
        {
            TokenType = info.GetValue<TokenType>(nameof(TokenType));
            Value = info.GetValue<string>(nameof(Value));
            Position = info.GetValue<uint>(nameof(Position));
            Line = info.GetValue<uint>(nameof(Line));
            NumberValue = info.GetValue<double>(nameof(NumberValue));
            BooleanValue = info.GetValue<bool>(nameof(BooleanValue));
            StringValue = info.GetValue<string>(nameof(StringValue));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(TokenType), TokenType);
            info.AddValue(nameof(Value), Value);
            info.AddValue(nameof(Position), Position);
            info.AddValue(nameof(Line), Line);
            info.AddValue(nameof(NumberValue), NumberValue);
            info.AddValue(nameof(BooleanValue), BooleanValue);
            info.AddValue(nameof(StringValue), StringValue);
        }
    }
}
