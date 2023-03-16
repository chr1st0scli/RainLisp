namespace RainLisp.Tokenization
{
    /// <summary>
    /// Represents a tokenizer capable of performing lexical analysis on the code, based on the language's lexical grammar.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Performs lexical analysis on code and produces tokens as a result.
        /// </summary>
        /// <param name="code">The code to lexically analyze.</param>
        /// <returns>A list of tokens.</returns>
        /// <exception cref="NonTerminatedStringException">A string literal is not properly terminated.</exception>
        /// <exception cref="InvalidEscapeSequenceException">An invalid string literal escape sequence is provided.</exception>
        /// <exception cref="InvalidStringCharacterException">An invalid string literal character is provided.</exception>
        /// <exception cref="InvalidNumberCharacterException">An invalid character for a numeric literal is provided.</exception>
        IList<Token> Tokenize(string? code);
    }
}
