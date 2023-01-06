namespace RainLisp.Tokenization
{
    public class TokenizationException : Exception
    {
        public TokenizationException(uint line, uint position)
        {
            Line = line;
            Position = position;
        }

        public TokenizationException(uint line, uint position, string? message) : base(message)
        {
            Line = line;
            Position = position;
        }

        public TokenizationException(uint line, uint position, string? message, Exception? innerException) : base(message, innerException)
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

        public NonTerminatedStringException(uint line, uint position, string? message) : base(line, position, message)
        {
        }

        public NonTerminatedStringException(uint line, uint position, string? message, Exception? innerException) : base(line, position, message, innerException)
        {
        }
    }

    public class StringCharacterException : TokenizationException
    {
        public StringCharacterException(uint line, uint position, char character) : base(line, position)
        {
            Character = character;
        }

        public StringCharacterException(uint line, uint position, char character, string? message) : base(line, position, message)
        {
            Character = character;
        }

        public StringCharacterException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, message, innerException)
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

        public InvalidEscapeSequenceException(uint line, uint position, char character, string? message) : base(line, position, character, message)
        {
        }

        public InvalidEscapeSequenceException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, character, message, innerException)
        {
        }
    }

    public class InvalidStringCharacterException : StringCharacterException
    {
        public InvalidStringCharacterException(uint line, uint position, char character) : base(line, position, character)
        {
        }

        public InvalidStringCharacterException(uint line, uint position, char character, string? message) : base(line, position, character, message)
        {
        }

        public InvalidStringCharacterException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, character, message, innerException)
        {
        }
    }

    public class InvalidNumberCharacterException : TokenizationException
    {
        public InvalidNumberCharacterException(uint line, uint position, char character) : base(line, position)
        {
            Character = character;
        }

        public InvalidNumberCharacterException(uint line, uint position, char character, string? message) : base(line, position, message)
        {
            Character = character;
        }

        public InvalidNumberCharacterException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, message, innerException)
        {
            Character = character;
        }

        public char Character { get; init; }
    }
}
