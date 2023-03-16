using RainLisp.AbstractSyntaxTree;
using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    /// <summary>
    /// Represents a parser that is capable of performing syntax analysis on tokens, based on the language's syntax grammar.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Returns an abstract syntax tree by syntactically analyzing tokens that are produced by a tokenizer.
        /// </summary>
        /// <param name="tokens">The tokens to syntactically analyze.</param>
        /// <returns>An abstract syntax tree to evaluate.</returns>
        /// <exception cref="ParsingException">The token sequence is syntactically incorrect.</exception>
        Program Parse(IList<Token> tokens);
    }
}
