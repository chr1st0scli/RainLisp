﻿namespace RainLisp.Tokenization
{
    /// <summary>
    /// Represents an exception that may occur during the lexical analysis of code.
    /// </summary>
    public class TokenizationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizationException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        protected TokenizationException(uint line, uint position)
        {
            Line = line;
            Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizationException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="message">The message that describes the error.</param>
        protected TokenizationException(uint line, uint position, string? message) : base(message)
        {
            Line = line;
            Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizationException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        protected TokenizationException(uint line, uint position, string? message, Exception? innerException) : base(message, innerException)
        {
            Line = line;
            Position = position;
        }

        /// <summary>
        /// Gets or sets the line in the code which the tokenization problem refers to.
        /// </summary>
        public uint Line { get; init; }

        /// <summary>
        /// Gets or sets the character position in the <see cref="Line"/> where the tokenization problem refers to.
        /// </summary>
        public uint Position { get; init; }
    }

    /// <summary>
    /// Represents an exception that may occur during the lexical analysis of code, that relates to a specific character.
    /// </summary>
    public class InvalidCharacterException : TokenizationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        public InvalidCharacterException(uint line, uint position, char character) : base(line, position)
        {
            Character = character;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public InvalidCharacterException(uint line, uint position, char character, string? message) : base(line, position, message)
        {
            Character = character;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidCharacterException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, message, innerException)
        {
            Character = character;
        }

        /// <summary>
        /// Gets or sets the invalid character causing the error.
        /// </summary>
        public char Character { get; init; }
    }

    /// <summary>
    /// Represents an exception that may occur during the lexical analysis of code, that relates to a string literal that is not terminated properly.
    /// </summary>
    public class NonTerminatedStringException : TokenizationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonTerminatedStringException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        public NonTerminatedStringException(uint line, uint position) : base(line, position)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonTerminatedStringException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="message">The message that describes the error.</param>
        public NonTerminatedStringException(uint line, uint position, string? message) : base(line, position, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonTerminatedStringException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public NonTerminatedStringException(uint line, uint position, string? message, Exception? innerException) : base(line, position, message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents an exception that may occur during the lexical analysis of code, that relates to an invalid escape sequence in a string literal.
    /// </summary>
    public class InvalidEscapeSequenceException : InvalidCharacterException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEscapeSequenceException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        public InvalidEscapeSequenceException(uint line, uint position, char character) : base(line, position, character)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEscapeSequenceException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public InvalidEscapeSequenceException(uint line, uint position, char character, string? message) : base(line, position, character, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEscapeSequenceException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidEscapeSequenceException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, character, message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents an exception that may occur during the lexical analysis of code, that relates to an invalid character in a string literal.
    /// </summary>
    public class InvalidStringCharacterException : InvalidCharacterException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStringCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        public InvalidStringCharacterException(uint line, uint position, char character) : base(line, position, character)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStringCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public InvalidStringCharacterException(uint line, uint position, char character, string? message) : base(line, position, character, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStringCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidStringCharacterException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, character, message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents an exception that may occur during the lexical analysis of code, that relates to an invalid character in a numeric literal.
    /// </summary>
    public class InvalidNumberCharacterException : InvalidCharacterException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNumberCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        public InvalidNumberCharacterException(uint line, uint position, char character) : base(line, position, character)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNumberCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public InvalidNumberCharacterException(uint line, uint position, char character, string? message) : base(line, position, character, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNumberCharacterException"/> class.
        /// </summary>
        /// <param name="line">The line in the code which the tokenization problem refers to.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where the tokenization problem refers to.</param>
        /// <param name="character">The invalid character causing the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidNumberCharacterException(uint line, uint position, char character, string? message, Exception? innerException) : base(line, position, character, message, innerException)
        {
        }
    }
}
