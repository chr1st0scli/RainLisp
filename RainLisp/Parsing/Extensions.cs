using RainLisp.AbstractSyntaxTree;
using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    /// <summary>
    /// Extension methods for syntax analysis.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Attaches debugging information to the abstract syntax tree <paramref name="expression"/> from <paramref name="token"/>.
        /// </summary>
        /// <param name="expression">The expression to copy debugging info to.</param>
        /// <param name="token">The token to copy debugging info from.</param>
        /// <returns>The <paramref name="expression"/> with debugging info.</returns>
        public static Expression WithDebugInfo(this Expression expression, Token token)
        {
            expression.Line = token.Line;
            expression.Position = token.Position;
            expression.HasDebugInfo = true;

            return expression;
        }

        /// <summary>
        /// Requires that the current token is an identifier. If it is, it advances the
        /// current position and returns the identifier's name; otherwise, it throws an exception.
        /// </summary>
        /// <param name="tokenConsumer">The token consumer to request the identifier from.</param>
        /// <returns>The identifier's name.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The required token is the last one, so the token position cannot be advanced.</exception>
        /// <exception cref="ParsingException">The current token's type is not an identifier.</exception>
        public static string RequireIdentifierName(this TokenConsumer tokenConsumer)
        {
            var currentToken = tokenConsumer.CurrentToken();
            tokenConsumer.Require(TokenType.Identifier);

            return currentToken.Value;
        }
    }
}
