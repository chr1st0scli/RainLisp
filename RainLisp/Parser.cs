namespace RainLisp
{
    public class Parser
    {
        private List<Token> _tokens;
        private int currPosition;

        public Parser()
        {
            _tokens = new List<Token>();
        }

        public void Parse(List<Token> tokens)
        {
            _tokens = tokens;
            currPosition = 0;
            Program();
        }

        private bool Check(TokenType tokenType)
            => currPosition >= _tokens.Count ? false : _tokens[currPosition].Type == tokenType;

        private bool CheckFurther(TokenType tokenType)
        {
            int pos = currPosition + 1;
            if (pos >= _tokens.Count)
                return false;

            return _tokens[pos].Type == tokenType;
        }

        private bool Match(TokenType tokenType)
        {
            if (!Check(tokenType))
                return false;

            NextToken();
            return true;
        }

        private void Require(TokenType tokenType)
        {
            if (!Match(tokenType))
                throw new InvalidOperationException($"Missing required symbol {tokenType}.");
        }

        private void NextToken() => currPosition++;

        private void Program()
        {
            while (!Check(TokenType.EOF))
            {
                if (CheckFurther(TokenType.Definition))
                    Definition();
                else
                    Expr();
            }
        }

        private void Definition()
        {
            Require(TokenType.LParen);
            Require(TokenType.Definition);

            if (Match(TokenType.Identifier))
            {
                Expr();
            }
            else if (Match(TokenType.LParen))
            {
                // Function name
                Require(TokenType.Identifier);

                // Formal arguments
                while (Match(TokenType.Identifier))
                {

                }
                Require(TokenType.RParen);
                Body();
            }
            else
                throw new InvalidOperationException($"Expected either an {TokenType.Identifier} or {TokenType.LParen}.");

            Require(TokenType.RParen);
        }

        private void Body()
        {
            while (CheckFurther(TokenType.Definition))
            {
                Definition();
            }

            Expr();
        }

        private void Expr()
        {
            if (!Match(TokenType.Number) &&
                !Match(TokenType.String) &&
                !Match(TokenType.Boolean) &&
                !Match(TokenType.Identifier))
            {
                Require(TokenType.LParen);

                if (Match(TokenType.Quote))
                {
                    // Can there be more than one?
                    Require(TokenType.Identifier);
                }
                else if (Match(TokenType.Assignment))
                {
                    Require(TokenType.Identifier);
                    Expr();
                }
                else if (Match(TokenType.If))
                {
                    // Condition
                    Expr();
                    // Consequent
                    Expr();

                    // Optional alternative
                    if (!Check(TokenType.RParen))
                    {
                        Expr();
                    }
                }
                else if (Match(TokenType.Begin))
                {
                    do
                    {
                        Expr();
                    } while (!Check(TokenType.RParen));
                }
                else if (Match(TokenType.Lambda))
                {
                    Require(TokenType.LParen);

                    // Optional lambda formal arguments
                    while (Match(TokenType.Identifier))
                    {

                    }

                    Require(TokenType.RParen);
                    Body();
                }
                else
                {
                    // Application
                    // Function to be applied
                    Expr();

                    // Arguments
                    while (!Check(TokenType.RParen))
                        Expr();
                }

                Require(TokenType.RParen);
            }
        }
    }
}
