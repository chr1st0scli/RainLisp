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
        private Token _currentToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenConsumer"/> class.
        /// </summary>
        /// <param name="tokens">The tokens to traverse. It must be terminated with an <see cref="TokenType.EOF"/> token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tokens"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="tokens"/> is empty.</exception>
        public TokenConsumer(IList<Token> tokens)
        {
            ArgumentNullException.ThrowIfNull(tokens, nameof(tokens));

            _tokens = tokens;
            _currPosition = 0;
            _currentToken = tokens[0];
        }

        /// <summary>
        /// Returns the token at the current position.
        /// </summary>
        /// <returns>The token at the current position.</returns>
        public Token CurrentToken()
            => _currentToken;

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
        /// <exception cref="ArgumentOutOfRangeException">The matched token is the last one, so the token position cannot be advanced.</exception>
        public bool Match(TokenType tokenType)
        {
            if (tokenType != _currentToken.Type)
                return false;

            _currentToken = _tokens[++_currPosition];
            return true;
        }

        /// <summary>
        /// Returns a value indicating whether the current token has any type other than <paramref name="tokenTypes"/> and if it does it advances the current position.
        /// </summary>
        /// <param name="tokenTypes">The undesirable token types.</param>
        /// <returns>true if the current token's type is any other than <paramref name="tokenTypes"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The matched token is the last one, so the token position cannot be advanced.</exception>
        public bool MatchAnyBut(TokenType[] tokenTypes)
        {
            foreach (var tokenType in tokenTypes)
            {
                if (tokenType == _currentToken.Type)
                    return false;
            }

            _currentToken = _tokens[++_currPosition];
            return true;
        }

        /// <summary>
        /// Requires that the current token has a certain type. If it does, it advances the current position; otherwise, it throws an exception.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <exception cref="ArgumentOutOfRangeException">The required token is the last one, so the token position cannot be advanced.</exception>
        /// <exception cref="ParsingException">The current token's type is not the same with <paramref name="tokenType"/>.</exception>
        public void Require(TokenType tokenType)
        {
            if (!Match(tokenType))
                throw new ParsingException(_currentToken.Line, _currentToken.Position, new[] { tokenType });
        }

        /// <summary>
        /// Requires that the current token has a certain type. If it does, it advances the current position.
        /// Otherwise, it throws an exception, reporting <paramref name="alternatives"/> as missing token types.
        /// </summary>
        /// <param name="tokenType">The token type to look for.</param>
        /// <param name="alternatives">Token types to report as missing if the current token's type is not matched.</param>
        /// <exception cref="ArgumentOutOfRangeException">The required token is the last one, so the token position cannot be advanced.</exception>
        /// <exception cref="ParsingException">The current token's type is not the same with <paramref name="tokenType"/>.</exception>
        public void Require(TokenType tokenType, params TokenType[] alternatives)
        {
            if (!Match(tokenType))
                throw new ParsingException(_currentToken.Line, _currentToken.Position, alternatives);
        }

        private bool Check(TokenType tokenType, int position)
            => position < _tokens.Count && _tokens[position].Type == tokenType;
    }
}
