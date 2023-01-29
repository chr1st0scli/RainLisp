using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    public class TokenStore
    {
        private readonly IList<Token> _tokens;
        private int _currPosition;

        public TokenStore(IList<Token> tokens)
        {
            ArgumentNullException.ThrowIfNull(tokens, nameof(tokens));

            _tokens = tokens;
            _currPosition = 0;
        }

        public Token CurrentToken() => _tokens[_currPosition];

        public bool Check(TokenType tokenType)
            => Check(tokenType, _currPosition);

        public bool CheckNext(TokenType tokenType)
            => Check(tokenType, _currPosition + 1);

        public bool Match(TokenType tokenType)
            => Match(tokenType, CurrentToken());

        public bool Match(TokenType tokenType, Token token)
        {
            // It's the caller's resposibility to ensure that the given token is the current one.
            if (tokenType != token.Type)
                return false;

            _currPosition++;
            return true;
        }

        public void Require(TokenType tokenType)
            => Require(tokenType, CurrentToken());

        public void Require(TokenType tokenType, Token token)
        {
            // It's the caller's resposibility to ensure that the given token is the current one.
            if (!Match(tokenType, token))
                throw new ParsingException(token.Line, token.Position, new[] { tokenType });
        }

        private bool Check(TokenType tokenType, int position)
            => position < _tokens.Count && _tokens[position].Type == tokenType;
    }
}
