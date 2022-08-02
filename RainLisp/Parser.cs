using RainLisp.AbstractSyntaxTree;
using System.Globalization;

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

        public Program Parse(List<Token> tokens)
        {
            _tokens = tokens;
            currPosition = 0;

            return Program();
        }

        private Token Token() => _tokens[currPosition];

        private bool Check(TokenType tokenType)
            => currPosition < _tokens.Count && _tokens[currPosition].Type == tokenType;

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

            currPosition++;
            return true;
        }

        private void Require(TokenType tokenType)
        {
            if (!Match(tokenType))
                throw new InvalidOperationException($"Missing required symbol {tokenType}.");
        }

        private Program Program()
        {
            var programExpression = new Program();

            while (!Check(TokenType.EOF))
            {
                if (CheckFurther(TokenType.Definition))
                    programExpression.Definitions.Add(Definition());
                else
                    programExpression.Expressions.Add(Expression());
            }

            return programExpression;
        }

        private Definition Definition()
        {
            Require(TokenType.LParen);
            Require(TokenType.Definition);

            var token = Token();
            Definition definitionExpression;

            if (Match(TokenType.Identifier))
            {
                definitionExpression = new Definition(token.Value, Expression());
            }
            else if (Match(TokenType.LParen))
            {
                token = Token();

                // Function name
                Require(TokenType.Identifier);

                List<string>? parameters = null;

                // Formal arguments
                while (!Check(TokenType.RParen))
                {
                    parameters ??= new List<string>();
                    parameters.Add(Token().Value);
                    Require(TokenType.Identifier);
                }
                Require(TokenType.RParen);

                var lambda = new Lambda(parameters, Body());
                definitionExpression = new Definition(token.Value, lambda);
            }
            else
                throw new InvalidOperationException($"Expected either an {TokenType.Identifier} or {TokenType.LParen}.");

            Require(TokenType.RParen);

            return definitionExpression;
        }

        private Body Body()
        {
            List<Definition>? definitions = null;

            while (CheckFurther(TokenType.Definition))
            {
                definitions ??= new List<Definition>();
                definitions.Add(Definition());
            }

            return new Body(definitions, Expression());
        }

        private Expression Expression()
        {
            var token = Token();

            if (Match(TokenType.Number))
            {
                return new NumberLiteral(double.Parse(token.Value, CultureInfo.InvariantCulture));
            }
            else if (Match(TokenType.String))
            {
                return new StringLiteral(token.Value);
            }
            else if (Match(TokenType.Boolean))
            {
                return new BooleanLiteral(bool.Parse(token.Value));
            }
            else if (Match(TokenType.Identifier))
            {
                return new Identifier(token.Value);
            }
            else
            {
                Require(TokenType.LParen);
                Expression expression;

                if (Match(TokenType.Quote))
                {
                    var quoteExpression = new Quote(Token().Value);

                    // Can there be more than one?
                    Require(TokenType.Identifier);
                    expression = quoteExpression;
                }
                else if (Match(TokenType.Assignment))
                {
                    string identifierName = Token().Value;

                    Require(TokenType.Identifier);

                    expression = new Assignment(identifierName, Expression());
                }
                else if (Match(TokenType.If))
                {
                    var predicate = Expression();
                    var consequent = Expression();
                    Expression? alternative = null;

                    // Optional alternative
                    if (!Check(TokenType.RParen))
                    {
                        alternative = Expression();
                    }

                    expression = new If(predicate, consequent, alternative);
                }
                else if (Match(TokenType.Begin))
                {
                    var expressions = new List<Expression>();
                    do
                    {
                        expressions.Add(Expression());
                    } while (!Check(TokenType.RParen));

                    expression = new Begin(expressions);
                }
                else if (Match(TokenType.Lambda))
                {
                    Require(TokenType.LParen);

                    List<string>? parameters = null;

                    // Optional lambda parameters
                    while (!Check(TokenType.RParen))
                    {
                        parameters ??= new List<string>();
                        parameters.Add(Token().Value);
                        Require(TokenType.Identifier);
                    }

                    Require(TokenType.RParen);

                    expression = new Lambda(parameters, Body());
                }
                else
                {
                    // Application
                    // Function to be applied
                    var @operator = Expression();

                    // Parameter values
                    List<Expression>? operands = null;
                    while (!Check(TokenType.RParen))
                    {
                        operands ??= new List<Expression>();
                        operands.Add(Expression());
                    }

                    expression = new Application(@operator, operands);
                }

                Require(TokenType.RParen);

                return expression;
            }
        }
    }
}
