using RainLisp.AbstractSyntaxTree;
using RainLisp.DerivedExpressions;
using RainLisp.Tokenization;
using System.Globalization;

namespace RainLisp.Parsing
{
    public class Parser : IParser
    {
        private IList<Token> _tokens = null!;
        private int _currPosition;

        public Program Parse(IList<Token> tokens)
        {
            ArgumentNullException.ThrowIfNull(tokens, nameof(tokens));

            _tokens = tokens;
            _currPosition = 0;

            return Program();
        }

        #region Methods for consuming and checking tokens.
        private Token CurrentToken() => _tokens[_currPosition];

        private bool Check(TokenType tokenType)
            => _currPosition < _tokens.Count && _tokens[_currPosition].Type == tokenType;

        private bool CheckNext(TokenType tokenType)
        {
            int pos = _currPosition + 1;
            if (pos >= _tokens.Count)
                return false;

            return _tokens[pos].Type == tokenType;
        }

        private bool Match(TokenType tokenType)
        {
            if (!Check(tokenType))
                return false;

            _currPosition++;
            return true;
        }

        private void Require(TokenType tokenType)
        {
            if (!Match(tokenType))
                throw new InvalidOperationException($"Missing required symbol {tokenType}.");
        } 
        #endregion

        #region Nonterminals in the syntax grammar
        private Program Program()
        {
            var program = new Program();

            while (!Check(TokenType.EOF))
            {
                if (CheckNext(TokenType.Definition))
                    program.Definitions.Add(Definition());
                else
                    program.Expressions.Add(Expression());
            }

            return program;
        }

        private Definition Definition()
        {
            Require(TokenType.LParen);
            Require(TokenType.Definition);

            var identifierToken = CurrentToken();
            Definition definition;

            if (Match(TokenType.Identifier))
            {
                definition = new Definition(identifierToken.Value, Expression());
            }
            // Defining a function like (define (foo a) a) is just syntactic sugar for (define foo (lambda (a) a))
            else if (Match(TokenType.LParen))
            {
                identifierToken = CurrentToken();

                // Function name
                Require(TokenType.Identifier);

                List<string>? parameters = null;

                // Formal arguments
                if (!Check(TokenType.RParen))
                {
                    parameters = new() { CurrentToken().Value };
                    Require(TokenType.Identifier);

                    while (!Check(TokenType.RParen))
                    {
                        parameters.Add(CurrentToken().Value);
                        Require(TokenType.Identifier);
                    }
                }

                Require(TokenType.RParen);

                var lambda = new Lambda(parameters, Body());

                definition = new Definition(identifierToken.Value, lambda);
            }
            else
                throw new InvalidOperationException($"Invalid definition, expected either an {TokenType.Identifier} or {TokenType.LParen}.");

            Require(TokenType.RParen);

            return definition;
        }

        private Body Body()
        {
            List<Definition>? definitions = null;

            if (CheckNext(TokenType.Definition))
            {
                definitions = new() { Definition() };

                while (CheckNext(TokenType.Definition))
                {
                    definitions.Add(Definition());
                }
            }

            // If I wanted more than one expression, I would have a problem between detecting an additional expression or an erroneous one.
            // I.e. calling Expression again, I would have to catch the exception. But what would it mean? There is an additional erroneous expression,
            // or there is no additional expression? Maybe this indicates a problem with my grammar, but why don't I use a singe begin expression to combine many?
            return new Body(definitions, Expression());
        }

        private Expression Expression()
        {
            var token = CurrentToken();

            if (Match(TokenType.Number))
                return new NumberLiteral(double.Parse(token.Value, CultureInfo.InvariantCulture));

            else if (Match(TokenType.String))
                return new StringLiteral(token.Value);

            else if (Match(TokenType.Boolean))
                return new BooleanLiteral(bool.Parse(token.Value));

            else if (Match(TokenType.Identifier))
                return new Identifier(token.Value);

            else
            {
                if (!Match(TokenType.LParen))
                    throw new InvalidOperationException("Invalid expression.");

                Expression expression;

                if (Match(TokenType.Quote))
                    expression = Quote();

                else if (Match(TokenType.Assignment))
                    expression = Assignment();

                else if (Match(TokenType.If))
                    expression = If();

                // cond is a derived expression, so it gets converted to an equivalent if.
                else if (Match(TokenType.Cond))
                    expression = Condition().ToIf();

                else if (Match(TokenType.Begin))
                    expression = Begin();

                else if (Match(TokenType.Lambda))
                    expression = Lambda();

                // let is a derived expression, so it gets converted to an equivalent lambda application.
                else if (Match(TokenType.Let))
                    expression = Let().ToLambdaApplication();

                // If it is none of the above, then it can only be a function application.
                else
                    expression = Application();

                Require(TokenType.RParen);

                return expression;
            }
        }

        private ConditionClause ConditionClause()
        {
            Require(TokenType.LParen);

            var predicate = Expression();
            var expressions = new List<Expression>();

            do
            {
                expressions.Add(Expression());
            } while (!Check(TokenType.RParen));

            Require(TokenType.RParen); // Require here double checks the RParen above, use Match above! This pattern will be found elsewhere too...

            return new ConditionClause(predicate, expressions);
        }

        private ConditionElseClause ConditionElseClause()
        {
            Require(TokenType.LParen);

            Require(TokenType.Else);

            var expressions = new List<Expression>();

            do
            {
                expressions.Add(Expression());
            } while (!Check(TokenType.RParen));

            Require(TokenType.RParen);

            return new ConditionElseClause(expressions);
        }

        private LetClause LetClause()
        {
            Require(TokenType.LParen);

            string identifierName = CurrentToken().Value;
            Require(TokenType.Identifier);

            var expression = Expression();

            Require(TokenType.RParen);

            return new LetClause(identifierName, expression);
        }
        #endregion

        #region Helper methods that do not correspond to nonterminals in the grammar.
        private Quote Quote()
        {
            var quoteExpression = new Quote(CurrentToken().Value);

            // Can there be more than one?
            // Support the 'a syntax or not
            Require(TokenType.Identifier);

            return quoteExpression;
        }

        private Assignment Assignment()
        {
            string identifierName = CurrentToken().Value;

            Require(TokenType.Identifier);

            return new Assignment(identifierName, Expression());
        }

        private If If()
        {
            var predicate = Expression();
            var consequent = Expression();
            Expression? alternative = null;

            // Optional alternative
            if (!Check(TokenType.RParen))
            {
                alternative = Expression();
            }

            return new If(predicate, consequent, alternative);
        }

        private Condition Condition()
        {
            var clauses = new List<ConditionClause>();

            // We deal with conditional clauses until an else or a closing parenthesis is reached.
            do
            {
                clauses.Add(ConditionClause());
            } while (!CheckNext(TokenType.Else) && !Check(TokenType.RParen));

            ConditionElseClause? elseClause = null;
            if (CheckNext(TokenType.Else))
                elseClause = ConditionElseClause();

            return new Condition(clauses, elseClause);
        }

        private Begin Begin()
        {
            var expressions = new List<Expression>();
            do
            {
                expressions.Add(Expression());
            } while (!Check(TokenType.RParen));

            return new Begin(expressions);
        }

        private Lambda Lambda()
        {
            Require(TokenType.LParen);

            List<string>? parameters = null;

            // Optional lambda parameters
            if (!Check(TokenType.RParen))
            {
                parameters = new() { CurrentToken().Value };
                Require(TokenType.Identifier);

                while (!Check(TokenType.RParen))
                {
                    parameters.Add(CurrentToken().Value);
                    Require(TokenType.Identifier);
                }
            }

            Require(TokenType.RParen);

            return new Lambda(parameters, Body());
        }

        private Let Let()
        {
            Require(TokenType.LParen);

            var letClauses = new List<LetClause>();

            do
            {
                letClauses.Add(LetClause());
            } while (!Check(TokenType.RParen));

            Require(TokenType.RParen);

            return new Let(letClauses, Body());
        }

        private Application Application()
        {
            // Function to be applied is an identifier for a function, a lambda, or a call that returns a function itself.
            var function = Expression();

            // Parameter values
            List<Expression>? operands = null;

            if (!Check(TokenType.RParen))
            {
                operands = new() { Expression() };

                while (!Check(TokenType.RParen))
                {
                    operands.Add(Expression());
                }
            }

            return new Application(function, operands);
        } 
        #endregion
    }
}
