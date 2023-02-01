using RainLisp.AbstractSyntaxTree;
using RainLisp.DerivedExpressions;
using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    public class Parser : IParser
    {
        private TokenStore _tokenStore = null!;

        public Program Parse(IList<Token> tokens)
        {
            _tokenStore = new TokenStore(tokens);

            return Program();
        }

        #region Nonterminals in the syntax grammar
        private Program Program()
        {
            var program = new Program();

            while (!_tokenStore.Check(TokenType.EOF))
            {
                if (_tokenStore.CheckNext(TokenType.Definition))
                {
                    program.Definitions ??= new List<Definition>();
                    program.Definitions.Add(Definition());
                }
                else
                {
                    program.Expressions ??= new List<Expression>();
                    program.Expressions.Add(Expression());
                }
            }

            return program;
        }

        private Definition Definition()
        {
            _tokenStore.Require(TokenType.LParen);
            _tokenStore.Require(TokenType.Definition);

            var currentToken = _tokenStore.CurrentToken();
            string identifierName = currentToken.Value;
            Definition definition;

            if (_tokenStore.Match(TokenType.Identifier, currentToken))
                definition = new Definition(identifierName, Expression());

            else if (_tokenStore.Match(TokenType.LParen, currentToken))
            {
                // Function name
                identifierName = _tokenStore.RequireIdentifierName();

                List<string>? parameters = null;

                // Function parameters
                if (!_tokenStore.Match(TokenType.RParen))
                {
                    parameters = new() { _tokenStore.RequireIdentifierName() };

                    while (!_tokenStore.Match(TokenType.RParen))
                        parameters.Add(_tokenStore.RequireIdentifierName());
                }

                // Defining a function like (define (foo a) a) is just syntactic sugar for (define foo (lambda (a) a))
                var lambda = new Lambda(parameters, Body());

                definition = new Definition(identifierName, lambda);
            }
            // Invalid definition, one of the two expected tokens was not encountered.
            else
                throw new ParsingException(currentToken.Line, currentToken.Position, new[] { TokenType.Identifier, TokenType.LParen });

            _tokenStore.Require(TokenType.RParen);

            return definition;
        }

        private Body Body()
        {
            List<Definition>? definitions = null;

            if (_tokenStore.CheckNext(TokenType.Definition))
            {
                definitions = new() { Definition() };

                while (_tokenStore.CheckNext(TokenType.Definition))
                    definitions.Add(Definition());
            }

            var expressions = OneOrMoreExpressionsUntilRightParen(false);

            return new Body(definitions, expressions);
        }

        private Expression Expression()
        {
            var currentToken = _tokenStore.CurrentToken();
            Expression expression;

            if (_tokenStore.Match(TokenType.Number, currentToken))
                expression = new NumberLiteral(currentToken.NumberValue);

            else if (_tokenStore.Match(TokenType.String, currentToken))
                expression = new StringLiteral(currentToken.Value);

            else if (_tokenStore.Match(TokenType.Boolean, currentToken))
                expression = new BooleanLiteral(currentToken.BooleanValue);

            else if (_tokenStore.Match(TokenType.Identifier, currentToken))
                expression = new Identifier(currentToken.Value);

            else
            {
                // Missing expression, one of the expected tokens was not encountered.
                if (!_tokenStore.Match(TokenType.LParen, currentToken))
                    throw new ParsingException(currentToken.Line, currentToken.Position, new[] { TokenType.Number, TokenType.String, TokenType.Boolean, TokenType.Identifier, TokenType.LParen });

                currentToken = _tokenStore.CurrentToken();

                if (_tokenStore.Match(TokenType.Quote, currentToken))
                    expression = CompleteQuote();

                else if (_tokenStore.Match(TokenType.Assignment, currentToken))
                    expression = CompleteAssignment();

                else if (_tokenStore.Match(TokenType.If, currentToken))
                    expression = CompleteIf();

                // cond is a derived expression, so it gets converted to an equivalent if.
                else if (_tokenStore.Match(TokenType.Cond, currentToken))
                    expression = CompleteCondition().ToIf();

                else if (_tokenStore.Match(TokenType.Begin, currentToken))
                    expression = new Begin(OneOrMoreExpressionsUntilRightParen());

                else if (_tokenStore.Match(TokenType.Lambda, currentToken))
                    expression = CompleteLambda();

                // let is a derived expression, so it gets converted to an equivalent lambda application.
                else if (_tokenStore.Match(TokenType.Let, currentToken))
                    expression = CompleteLet().ToLambdaApplication();

                // and & or are derived expressions, so that they get converted to equivalent ifs.
                else if (_tokenStore.Match(TokenType.And, currentToken))
                    expression = new And(OneOrMoreExpressionsUntilRightParen()).ToIf();

                else if (_tokenStore.Match(TokenType.Or, currentToken))
                    expression = new Or(OneOrMoreExpressionsUntilRightParen()).ToIf();

                // If it is none of the above, then it can only be a function application.
                else
                    expression = CompleteApplication();
            }

            return expression.AddDebugInfo(currentToken);
        }

        private ConditionClause ConditionClause()
        {
            _tokenStore.Require(TokenType.LParen);

            var predicate = Expression();
            var expressions = OneOrMoreExpressionsUntilRightParen();

            return new ConditionClause(predicate, expressions);
        }

        private ConditionElseClause ConditionElseClause()
        {
            _tokenStore.Require(TokenType.LParen);
            _tokenStore.Require(TokenType.Else);

            var expressions = OneOrMoreExpressionsUntilRightParen();

            return new ConditionElseClause(expressions);
        }

        private LetClause LetClause()
        {
            _tokenStore.Require(TokenType.LParen);

            string identifierName = _tokenStore.RequireIdentifierName();
            var expression = Expression();

            _tokenStore.Require(TokenType.RParen);

            return new LetClause(identifierName, expression);
        }
        #endregion

        #region Helper methods that are part of expression. They do not correspond to nonterminals in the grammar themselves.
        private Quote CompleteQuote()
        {
            var currentToken = _tokenStore.CurrentToken();
            var quoteExpression = new Quote(currentToken.Value);

            // Can there be more than one?
            // Support the 'a syntax or not
            _tokenStore.Require(TokenType.Identifier, currentToken);
            _tokenStore.Require(TokenType.RParen);

            return quoteExpression;
        }

        private Assignment CompleteAssignment()
        {
            string identifierName = _tokenStore.RequireIdentifierName();
            var value = Expression();
            _tokenStore.Require(TokenType.RParen);

            return new Assignment(identifierName, value);
        }

        private If CompleteIf()
        {
            var predicate = Expression();
            var consequent = Expression();

            // Optional alternative.
            Expression? alternative = null;

            if (!_tokenStore.Match(TokenType.RParen))
            {
                alternative = Expression();
                _tokenStore.Require(TokenType.RParen);
            }

            return new If(predicate, consequent, alternative);
        }

        private Condition CompleteCondition()
        {
            var clauses = new List<ConditionClause>();

            // Optional else.
            ConditionElseClause? elseClause = null;

            // We deal with conditional clauses until a conditional else or a closing parenthesis is reached.
            do
            {
                clauses.Add(ConditionClause());

                if (_tokenStore.CheckNext(TokenType.Else))
                {
                    elseClause = ConditionElseClause();
                    _tokenStore.Require(TokenType.RParen);
                    break;
                }

            } while (!_tokenStore.Match(TokenType.RParen));

            return new Condition(clauses, elseClause);
        }

        private Lambda CompleteLambda()
        {
            _tokenStore.Require(TokenType.LParen);

            List<string>? parameters = null;

            // Optional lambda parameters
            if (!_tokenStore.Match(TokenType.RParen))
            {
                parameters = new() { _tokenStore.RequireIdentifierName() };

                while (!_tokenStore.Match(TokenType.RParen))
                    parameters.Add(_tokenStore.RequireIdentifierName());
            }

            var body = Body();
            _tokenStore.Require(TokenType.RParen);

            return new Lambda(parameters, body);
        }

        private Let CompleteLet()
        {
            _tokenStore.Require(TokenType.LParen);

            var letClauses = new List<LetClause>();

            do
            {
                letClauses.Add(LetClause());
            } while (!_tokenStore.Match(TokenType.RParen));

            var body = Body();
            _tokenStore.Require(TokenType.RParen);

            return new Let(letClauses, body);
        }

        private Application CompleteApplication()
        {
            // Operator is an identifier for a function, a lambda, or a call that returns a function itself.
            var operatorToApply = Expression();

            // Parameter values
            List<Expression>? operands = null;

            if (!_tokenStore.Match(TokenType.RParen))
            {
                operands = new() { Expression() };

                while (!_tokenStore.Match(TokenType.RParen))
                    operands.Add(Expression());
            }

            return new Application(operatorToApply, operands);
        }
        #endregion

        private List<Expression> OneOrMoreExpressionsUntilRightParen(bool includeRightParen = true)
        {
            Func<TokenType, bool> checkBound = includeRightParen ? _tokenStore.Match : _tokenStore.Check;
            var expressions = new List<Expression>();

            do
            {
                expressions.Add(Expression());
            } while (!checkBound(TokenType.RParen));

            return expressions;
        }
    }
}
