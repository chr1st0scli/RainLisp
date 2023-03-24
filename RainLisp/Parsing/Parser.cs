using RainLisp.AbstractSyntaxTree;
using RainLisp.DerivedExpressions;
using RainLisp.Grammar;
using RainLisp.Tokenization;

namespace RainLisp.Parsing
{
    /// <summary>
    /// Represents a parser that is capable of performing syntax analysis on tokens, based on the language's syntax grammar.
    /// </summary>
    public class Parser : IParser
    {
        private TokenConsumer _tokens = null!;

        /// <summary>
        /// Returns an abstract syntax tree by syntactically analyzing tokens that are produced by a tokenizer.
        /// </summary>
        /// <param name="tokens">The tokens to syntactically analyze.</param>
        /// <returns>An abstract syntax tree to evaluate.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tokens"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="tokens"/> is empty.</exception>
        /// <exception cref="ParsingException">The token sequence is syntactically incorrect.</exception>
        public Program Parse(IList<Token> tokens)
        {
            _tokens = new TokenConsumer(tokens);

            return Program();
        }

        #region Nonterminals in the syntax grammar.
        private Program Program()
        {
            var program = new Program();

            if (_tokens.Check(TokenType.EOF))
                return program;

            program.DefinitionsAndExpressions = new List<Node>();

            do
            {
                Node node = DefinitionFollows() ? Definition() : Expression();
                program.DefinitionsAndExpressions.Add(node);

            } while (!_tokens.Check(TokenType.EOF));

            return program;
        }

        private Definition Definition()
        {
            _tokens.Require(TokenType.LParen);
            _tokens.Require(TokenType.Definition);

            var currentToken = _tokens.CurrentToken();
            string identifierName = currentToken.Value;
            Definition definition;

            if (_tokens.Match(TokenType.Identifier))
                definition = new Definition(identifierName, Expression());
            else
            {
                // Invalid definition, if none of the two expected tokens is encountered.
                _tokens.Require(TokenType.LParen, TokenType.Identifier, TokenType.LParen);

                // Function name
                identifierName = _tokens.RequireIdentifierName();
                List<string>? parameters = OptionalFunctionParameters();

                // Defining a function like (define (foo a) a) is just syntactic sugar for (define foo (lambda (a) a))
                var lambda = new Lambda(parameters, Body());

                definition = new Definition(identifierName, lambda);
            }

            _tokens.Require(TokenType.RParen);

            return definition;
        }

        private Body Body()
        {
            List<Definition>? definitions = null;

            if (DefinitionFollows())
            {
                definitions = new() { Definition() };

                while (DefinitionFollows())
                    definitions.Add(Definition());
            }

            var expressions = OneOrMoreExpressionsUntilRightParen(false);

            return new Body(definitions, expressions);
        }

        private Expression Expression()
        {
            var currentToken = _tokens.CurrentToken();
            Expression expression;

            if (_tokens.Match(TokenType.Number))
                expression = new NumberLiteral(currentToken.NumberValue);

            else if (_tokens.Match(TokenType.String))
                expression = new StringLiteral(currentToken.StringValue);

            else if (_tokens.Match(TokenType.Boolean))
                expression = new BooleanLiteral(currentToken.BooleanValue);

            else if (_tokens.Match(TokenType.Identifier))
                expression = new Identifier(currentToken.Value);

            else if (_tokens.Match(TokenType.QuoteAlt))
                expression = new Quote(Quotable());

            else
            {
                // Missing expression, one of the expected tokens was not encountered.
                _tokens.Require(TokenType.LParen, TokenType.Number, TokenType.String, TokenType.Boolean, TokenType.Identifier, TokenType.QuoteAlt, TokenType.LParen);

                currentToken = _tokens.CurrentToken();

                if (_tokens.Match(TokenType.Quote))
                    expression = CompleteQuote();

                else if (_tokens.Match(TokenType.Assignment))
                    expression = CompleteAssignment();

                else if (_tokens.Match(TokenType.If))
                    expression = CompleteIf();

                // cond is a derived expression, so it gets converted to an equivalent if.
                else if (_tokens.Match(TokenType.Cond))
                    expression = CompleteCondition().ToIf();

                else if (_tokens.Match(TokenType.Begin))
                    expression = new Begin(OneOrMoreExpressionsUntilRightParen());

                else if (_tokens.Match(TokenType.Lambda))
                    expression = CompleteLambda();

                // let is a derived expression, so it gets converted to an equivalent lambda application.
                else if (_tokens.Match(TokenType.Let))
                    expression = CompleteLet().ToLambdaApplication();

                // and & or are derived expressions, so that they get converted to equivalent ifs.
                else if (_tokens.Match(TokenType.And))
                    expression = new And(OneOrMoreExpressionsUntilRightParen()).ToIf();

                else if (_tokens.Match(TokenType.Or))
                    expression = new Or(OneOrMoreExpressionsUntilRightParen()).ToIf();

                // If it is none of the above, then it can only be a function application.
                else
                    expression = CompleteApplication();
            }

            return expression.WithDebugInfo(currentToken);
        }

        private Quotable Quotable()
        {
            var currentToken = _tokens.CurrentToken();

            string? quoteText = null;
            List<Quotable>? quotes = null;

            // If the quotable itself is of the form '<quotable>, it gets converted to the equivalent list of quotables (quote <quotable>).
            if (_tokens.Match(TokenType.QuoteAlt))
                quotes = new List<Quotable> { new Quotable(Keywords.QUOTE), Quotable() };

            // All other tokens are valid for a singular (i.e. non list) quotable.
            else if (_tokens.MatchAnyBut(new[] { TokenType.LParen, TokenType.RParen, TokenType.EOF }))
                quoteText = currentToken.Value;

            else
            {
                // Only tokens that can start an expression are reported. Other keywords, such as special forms and derived expressions, don't need to.
                _tokens.Require(TokenType.LParen, TokenType.Number, TokenType.String, TokenType.Boolean, TokenType.Identifier, TokenType.QuoteAlt, TokenType.LParen);

                quotes = new List<Quotable>();

                while (!_tokens.Match(TokenType.RParen))
                    quotes.Add(Quotable());
            }

            return new Quotable(quoteText, quotes);
        }
        #endregion

        #region Helper methods that are part of expression. They do not correspond to nonterminals in the grammar themselves.
        private Quote CompleteQuote()
        {
            var quotable = Quotable();
            _tokens.Require(TokenType.RParen);

            return new Quote(quotable);
        }

        private Assignment CompleteAssignment()
        {
            string identifierName = _tokens.RequireIdentifierName();
            var value = Expression();
            _tokens.Require(TokenType.RParen);

            return new Assignment(identifierName, value);
        }

        private If CompleteIf()
        {
            var predicate = Expression();
            var consequent = Expression();

            // Optional alternative.
            Expression? alternative = null;

            if (!_tokens.Match(TokenType.RParen))
            {
                alternative = Expression();
                _tokens.Require(TokenType.RParen);
            }

            return new If(predicate, consequent, alternative);
        }

        private Condition CompleteCondition()
        {
            var clauses = new List<ConditionClause>();

            // Optional else.
            ConditionElseClause? elseClause = null;

            // We deal with conditional clauses until a conditional else or a closing parenthesis is reached.
            bool isFirstConditionClause = true;
            do
            {
                // Condition clause.
                // The first failure means a missing left parenthesis.
                if (isFirstConditionClause)
                    _tokens.Require(TokenType.LParen);
                // Consecutive ones mean a missing right or left parenthesis.
                else
                    _tokens.Require(TokenType.LParen, TokenType.RParen, TokenType.LParen);

                var predicate = Expression();
                var expressions = OneOrMoreExpressionsUntilRightParen();

                clauses.Add(new(predicate, expressions));
                isFirstConditionClause = false;

                // Optional else clause in the end.
                if (_tokens.CheckNext(TokenType.Else))
                {
                    _tokens.Require(TokenType.LParen);
                    _tokens.Require(TokenType.Else);

                    expressions = OneOrMoreExpressionsUntilRightParen();
                    elseClause = new(expressions);

                    _tokens.Require(TokenType.RParen);
                    break;
                }

            } while (!_tokens.Match(TokenType.RParen));

            return new Condition(clauses, elseClause);
        }

        private Lambda CompleteLambda()
        {
            _tokens.Require(TokenType.LParen);

            List<string>? parameters = OptionalFunctionParameters();
            var body = Body();

            _tokens.Require(TokenType.RParen);

            return new Lambda(parameters, body);
        }

        private Let CompleteLet()
        {
            _tokens.Require(TokenType.LParen);

            var letClauses = new List<LetClause>();

            bool isFirstLetClause = true;
            do
            {
                // Let clause.
                // The first failure means a missing left parenthesis.
                if (isFirstLetClause)
                    _tokens.Require(TokenType.LParen);
                // Consecutive ones mean a missing right or left parenthesis.
                else
                    _tokens.Require(TokenType.LParen, TokenType.RParen, TokenType.LParen);

                string identifierName = _tokens.RequireIdentifierName();
                var expression = Expression();

                _tokens.Require(TokenType.RParen);

                letClauses.Add(new(identifierName, expression));
                isFirstLetClause = false;

            } while (!_tokens.Match(TokenType.RParen));

            var body = Body();
            _tokens.Require(TokenType.RParen);

            return new Let(letClauses, body);
        }

        private Application CompleteApplication()
        {
            // Operator is an identifier for a function, a lambda, or a call that returns a function itself.
            var operatorToApply = Expression();

            // Parameter values
            List<Expression>? operands = null;

            if (!_tokens.Match(TokenType.RParen))
            {
                operands = new() { Expression() };

                while (!_tokens.Match(TokenType.RParen))
                    operands.Add(Expression());
            }

            return new Application(operatorToApply, operands);
        }
        #endregion

        #region General Helpers
        private bool DefinitionFollows() 
            => _tokens.Check(TokenType.LParen) && _tokens.CheckNext(TokenType.Definition);

        private List<Expression> OneOrMoreExpressionsUntilRightParen(bool consumeLastRightParen = true)
        {
            Func<TokenType, bool> checkBound = consumeLastRightParen ? _tokens.Match : _tokens.Check;
            var expressions = new List<Expression>();

            do
            {
                expressions.Add(Expression());
            } while (!checkBound(TokenType.RParen));

            return expressions;
        }

        private List<string>? OptionalFunctionParameters()
        {
            List<string>? parameters = null;

            // Optional parameters
            if (!_tokens.Match(TokenType.RParen))
            {
                parameters = new() { _tokens.RequireIdentifierName() };

                while (!_tokens.Match(TokenType.RParen))
                    parameters.Add(_tokens.RequireIdentifierName());
            }

            return parameters;
        } 
        #endregion
    }
}
