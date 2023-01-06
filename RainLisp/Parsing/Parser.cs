using RainLisp.AbstractSyntaxTree;
using RainLisp.DerivedExpressions;
using RainLisp.Tokenization;

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

        #region Nonterminals in the syntax grammar
        private Program Program()
        {
            var program = new Program();

            while (!Check(TokenType.EOF))
            {
                if (CheckNext(TokenType.Definition))
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
            Require(TokenType.LParen);
            Require(TokenType.Definition);

            var currentToken = CurrentToken();
            string identifierName = currentToken.Value;
            Definition definition;

            if (Match(TokenType.Identifier))
                definition = new Definition(identifierName, Expression());

            else if (Match(TokenType.LParen))
            {
                // Function name
                identifierName = RequireValueForIdentifier();

                List<string>? parameters = null;

                // Function parameters
                if (!Match(TokenType.RParen))
                {
                    parameters = new() { RequireValueForIdentifier() };

                    while (!Match(TokenType.RParen))
                        parameters.Add(RequireValueForIdentifier());
                }

                // Defining a function like (define (foo a) a) is just syntactic sugar for (define foo (lambda (a) a))
                var lambda = new Lambda(parameters, Body());

                definition = new Definition(identifierName, lambda);
            }
            // Invalid definition, one of the two expected tokens was not encountered.
            else
                throw new ParsingException(currentToken.Line, currentToken.Position, new[] { TokenType.Identifier, TokenType.LParen });

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
                    definitions.Add(Definition());
            }

            var expressions = OneOrMoreExpressionsUntilRightParen(false);

            return new Body(definitions, expressions);
        }

        private Expression Expression()
        {
            var currentToken = CurrentToken();
            string tokenValue = currentToken.Value;

            if (Match(TokenType.Number))
                return new NumberLiteral(currentToken.NumberValue);

            else if (Match(TokenType.String))
                return new StringLiteral(tokenValue);

            else if (Match(TokenType.Boolean))
                return new BooleanLiteral(currentToken.BooleanValue);

            else if (Match(TokenType.Identifier))
                return new Identifier(tokenValue);

            else
            {
                // Missing expression, one of the expected tokens was not encountered.
                if (!Match(TokenType.LParen))
                    throw new ParsingException(currentToken.Line, currentToken.Position, new[] { TokenType.Number, TokenType.String, TokenType.Boolean, TokenType.Identifier, TokenType.LParen });

                if (Match(TokenType.Quote))
                    return QuoteExpr();

                else if (Match(TokenType.Assignment))
                    return AssignmentExpr();

                else if (Match(TokenType.If))
                    return IfExpr();

                // cond is a derived expression, so it gets converted to an equivalent if.
                else if (Match(TokenType.Cond))
                    return ConditionExpr().ToIf();

                else if (Match(TokenType.Begin))
                    return new Begin(OneOrMoreExpressionsUntilRightParen());

                else if (Match(TokenType.Lambda))
                    return LambdaExpr();

                // let is a derived expression, so it gets converted to an equivalent lambda application.
                else if (Match(TokenType.Let))
                    return LetExpr().ToLambdaApplication();

                // and & or are derived expressions, so that they get converted to equivalent ifs.
                else if (Match(TokenType.And))
                    return new And(OneOrMoreExpressionsUntilRightParen()).ToIf();

                else if (Match(TokenType.Or))
                    return new Or(OneOrMoreExpressionsUntilRightParen()).ToIf();

                // If it is none of the above, then it can only be a function application.
                else
                    return ApplicationExpr();
            }
        }

        private ConditionClause ConditionClause()
        {
            Require(TokenType.LParen);

            var predicate = Expression();
            var expressions = OneOrMoreExpressionsUntilRightParen();

            return new ConditionClause(predicate, expressions);
        }

        private ConditionElseClause ConditionElseClause()
        {
            Require(TokenType.LParen);
            Require(TokenType.Else);

            var expressions = OneOrMoreExpressionsUntilRightParen();

            return new ConditionElseClause(expressions);
        }

        private LetClause LetClause()
        {
            Require(TokenType.LParen);

            string identifierName = RequireValueForIdentifier();
            var expression = Expression();

            Require(TokenType.RParen);

            return new LetClause(identifierName, expression);
        }
        #endregion

        #region Helper methods that are part of expression. They do not correspond to nonterminals in the grammar themselves.
        private Quote QuoteExpr()
        {
            var quoteExpression = new Quote(CurrentToken().Value);

            // Can there be more than one?
            // Support the 'a syntax or not
            Require(TokenType.Identifier);
            Require(TokenType.RParen);

            return quoteExpression;
        }

        private Assignment AssignmentExpr()
        {
            string identifierName = RequireValueForIdentifier();
            var value = Expression();
            Require(TokenType.RParen);

            return new Assignment(identifierName, value);
        }

        private If IfExpr()
        {
            var predicate = Expression();
            var consequent = Expression();

            // Optional alternative.
            Expression? alternative = null;

            if (!Match(TokenType.RParen))
            {
                alternative = Expression();
                Require(TokenType.RParen);
            }

            return new If(predicate, consequent, alternative);
        }

        private Condition ConditionExpr()
        {
            var clauses = new List<ConditionClause>();

            // Optional else.
            ConditionElseClause? elseClause = null;

            // We deal with conditional clauses until a conditional else or a closing parenthesis is reached.
            do
            {
                clauses.Add(ConditionClause());

                if (CheckNext(TokenType.Else))
                {
                    elseClause = ConditionElseClause();
                    Require(TokenType.RParen);
                    break;
                }

            } while (!Match(TokenType.RParen));

            return new Condition(clauses, elseClause);
        }

        private Lambda LambdaExpr()
        {
            Require(TokenType.LParen);

            List<string>? parameters = null;

            // Optional lambda parameters
            if (!Match(TokenType.RParen))
            {
                parameters = new() { RequireValueForIdentifier() };

                while (!Match(TokenType.RParen))
                    parameters.Add(RequireValueForIdentifier());
            }

            var body = Body();
            Require(TokenType.RParen);

            return new Lambda(parameters, body);
        }

        private Let LetExpr()
        {
            Require(TokenType.LParen);

            var letClauses = new List<LetClause>();

            do
            {
                letClauses.Add(LetClause());
            } while (!Match(TokenType.RParen));

            var body = Body();
            Require(TokenType.RParen);

            return new Let(letClauses, body);
        }

        private Application ApplicationExpr()
        {
            // Operator is an identifier for a function, a lambda, or a call that returns a function itself.
            var operatorToApply = Expression();

            // Parameter values
            List<Expression>? operands = null;

            if (!Match(TokenType.RParen))
            {
                operands = new() { Expression() };

                while (!Match(TokenType.RParen))
                    operands.Add(Expression());
            }

            return new Application(operatorToApply, operands);
        }
        #endregion

        private List<Expression> OneOrMoreExpressionsUntilRightParen(bool includeRightParen = true)
        {
            Func<TokenType, bool> checkBound = includeRightParen ? Match : Check;
            var expressions = new List<Expression>();

            do
            {
                expressions.Add(Expression());
            } while (!checkBound(TokenType.RParen));

            return expressions;
        }

        #region Methods for consuming and checking tokens.
        private Token CurrentToken() => _tokens[_currPosition];

        private bool Check(TokenType tokenType)
            => _currPosition < _tokens.Count && _tokens[_currPosition].Type == tokenType;

        private bool CheckNext(TokenType tokenType)
        {
            int pos = _currPosition + 1;

            return pos < _tokens.Count && _tokens[pos].Type == tokenType;
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
            {
                var currentToken = CurrentToken();
                throw new ParsingException(currentToken.Line, currentToken.Position, new[] { tokenType });
            }
        }

        private string RequireValueForIdentifier()
        {
            var currentToken = CurrentToken();
            Require(TokenType.Identifier);

            return currentToken.Value;
        }
        #endregion
    }
}
