namespace RainLisp.Tokenization
{
    public class TokenizationException : Exception
    {
        public TokenizationException(uint line, uint position)
        {
            Line = line;
            Position = position;
        }

        public uint Line { get; init; }

        public uint Position { get; init; }
    }

    public class NonTerminatedStringException : TokenizationException
    {
        public NonTerminatedStringException(uint line, uint position) : base(line, position)
        {
        }
    }

    public class StringCharacterException : TokenizationException
    {
        public StringCharacterException(uint line, uint position, char character) : base(line, position)
        {
            Character = character;
        }

        public char Character { get; init; }
    }

    public class InvalidEscapeSequenceException : StringCharacterException
    {
        public InvalidEscapeSequenceException(uint line, uint position, char character) : base(line, position, character)
        {
        }
    }

    public class InvalidStringCharacterException : StringCharacterException
    {
        public InvalidStringCharacterException(uint line, uint position, char character) : base(line, position, character)
        {
        }
    }
}
