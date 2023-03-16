using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    /// <summary>
    /// Represents a facility for token access and checking during syntax analysis.
    /// </summary>
    public class TokenConsumer
    {
        private readonly IList<Token> _tokens;
        private int _currPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenConsumer"/> class.
        /// </summary>
        /// <param name="tokens">The tokens to traverse.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tokens"/> is null.</exception>
        public TokenConsumer(IList<Token> tokens)
        {
            ArgumentNullException.ThrowIfNull(tokens, nameof(tokens));

            _tokens = tokens;
            _currPosition = 0;
        }

        /// <summary>
        /// Returns the token at the current position.
        /// </summary>
        /// <returns>The token at the current position.</returns>
        public Token CurrentToken()
            => _tokens[_currPosition];

        /// <summary>
        /// Returns a value indicating whether the current token has a certain type.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <returns>true if the current token's type is the same with <paramref name="tokenType"/>; otherwise, false.</returns>
        public bool Check(TokenType tokenType)
            => Check(tokenType, _currPosition);

        /// <summary>
        /// Returns a value indicating whether the next token has a certain type.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <returns>true if there is a next token and its type is the same with <paramref name="tokenType"/>; otherwise, false.</returns>
        public bool CheckNext(TokenType tokenType)
            => Check(tokenType, _currPosition + 1);

        /// <summary>
        /// Returns a value indicating whether the current token has a certain type and if it does it advances the current position.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <returns>true if the current token's type is the same with <paramref name="tokenType"/>; otherwise, false.</returns>
        public bool Match(TokenType tokenType)
            => Match(tokenType, CurrentToken());

        /// <summary>
        /// Returns a value indicating whether <paramref name="token"/> has a certain type and if it does it advances the current position.
        /// It is preferable to call this overload if the current token has already been acquired for other purposes,
        /// in which case it's the caller's resposibility to ensure that <paramref name="token"/> is the current one.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <param name="token">The token to check. It should be the current one.</param>
        /// <returns>true if the <paramref name="token"/>'s type is the same with <paramref name="tokenType"/>; otherwise, false.</returns>
        public bool Match(TokenType tokenType, Token token)
        {
            if (tokenType != token.Type)
                return false;

            _currPosition++;
            return true;
        }

        /// <summary>
        /// Returns a value indicating whether <paramref name="token"/> has any type other than <paramref name="tokenTypes"/> and if it does it advances the current position.
        /// It is preferable to call this overload if the current token has already been acquired for other purposes,
        /// in which case it's the caller's resposibility to ensure that <paramref name="token"/> is the current one.
        /// </summary>
        /// <param name="tokenTypes">The undesirable token types.</param>
        /// <param name="token">The token to check. It should be the current one.</param>
        /// <returns>true if the <paramref name="token"/>'s type is any other than <paramref name="tokenTypes"/>; otherwise, false.</returns>
        public bool MatchAnyBut(TokenType[] tokenTypes, Token token)
        {
            foreach (var tokenType in tokenTypes)
            {
                if (tokenType == token.Type)
                    return false;
            }

            _currPosition++;
            return true;
        }

        /// <summary>
        /// Requires that the current token has a certain type. If it does, it advances the current position; otherwise, it throws an exception.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <exception cref="ParsingException">The current token's type is not the same with <paramref name="tokenType"/>.</exception>
        public void Require(TokenType tokenType)
            => Require(tokenType, CurrentToken());

        /// <summary>
        /// Requires that <paramref name="token"/> has a certain type. If it does, it advances the current position; otherwise, it throws an exception.
        /// It is preferable to call this overload if the current token has already been acquired for other purposes,
        /// in which case it's the caller's resposibility to ensure that <paramref name="token"/> is the current one.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <param name="token">The token to check. It should be the current one.</param>
        /// <exception cref="ParsingException">The <paramref name="token"/>'s type is not the same with <paramref name="tokenType"/>.</exception>
        public void Require(TokenType tokenType, Token token)
        {
            // It's the caller's resposibility to ensure that the given token is the current one.
            if (!Match(tokenType, token))
                throw new ParsingException(token.Line, token.Position, new[] { tokenType });
        }

        /// <summary>
        /// Requires that the current token has a certain type. If it does, it advances the current position.
        /// Otherwise, it throws an exception, reporting <paramref name="alternatives"/> as missing token types.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <param name="alternatives">Token types to report as missing if the current token's type is not matched.</param>
        /// <exception cref="ParsingException">The current token's type is not the same with <paramref name="tokenType"/>.</exception>
        public void Require(TokenType tokenType, params TokenType[] alternatives)
            => Require(tokenType, CurrentToken(), alternatives);

        /// <summary>
        /// Requires that <paramref name="token"/> has a certain type. If it does, it advances the current position.
        /// Otherwise, it throws an exception, reporting <paramref name="alternatives"/> as missing token types.
        /// It is preferable to call this overload if the current token has already been acquired for other purposes,
        /// in which case it's the caller's resposibility to ensure that <paramref name="token"/> is the current one.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <param name="token">The token to check. It should be the current one.</param>
        /// <param name="alternatives">Token types to report as missing if the <paramref name="token"/>'s type is not matched.</param>
        /// <exception cref="ParsingException">The <paramref name="token"/>'s type is not the same with <paramref name="tokenType"/>.</exception>
        public void Require(TokenType tokenType, Token token, params TokenType[] alternatives)
        {
            // It's the caller's resposibility to ensure that the given token is the current one.
            if (!Match(tokenType, token))
                throw new ParsingException(token.Line, token.Position, alternatives);
        }

        private bool Check(TokenType tokenType, int position)
            => position < _tokens.Count && _tokens[position].Type == tokenType;
    }
}
