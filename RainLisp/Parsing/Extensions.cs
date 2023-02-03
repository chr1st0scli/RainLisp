using RainLisp.AbstractSyntaxTree;
using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    public static class Extensions
    {
        public static Expression WithDebugInfo(this Expression expression, Token token)
        {
            expression.Line = token.Line;
            expression.Position = token.Position;
            expression.HasDebugInfo = true;

            return expression;
        }

        public static string RequireIdentifierName(this TokenConsumer tokenConsumer)
        {
            var currentToken = tokenConsumer.CurrentToken();
            tokenConsumer.Require(TokenType.Identifier, currentToken);

            return currentToken.Value;
        }
    }
}
