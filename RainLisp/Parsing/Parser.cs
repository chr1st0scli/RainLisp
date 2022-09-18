﻿using RainLisp.AbstractSyntaxTree;
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

            string identifierName = CurrentToken().Value;
            Definition definition;

            if (Match(TokenType.Identifier))
                definition = new Definition(identifierName, Expression());

            else if (Match(TokenType.LParen))
            {
                // Function name
                identifierName = CurrentToken().Value;
                Require(TokenType.Identifier);

                List<string>? parameters = null;

                // Function parameters
                if (!Match(TokenType.RParen))
                {
                    parameters = new() { CurrentToken().Value };
                    Require(TokenType.Identifier);

                    while (!Match(TokenType.RParen))
                    {
                        parameters.Add(CurrentToken().Value);
                        Require(TokenType.Identifier);
                    }
                }

                // Defining a function like (define (foo a) a) is just syntactic sugar for (define foo (lambda (a) a))
                var lambda = new Lambda(parameters, Body());

                definition = new Definition(identifierName, lambda);
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
                    definitions.Add(Definition());
            }

            // If I wanted more than one expression, I would have a problem between detecting an additional expression or an erroneous one.
            // I.e. calling Expression again, I would have to catch the exception. But what would it mean? There is an additional erroneous expression,
            // or there is no additional expression? Maybe this indicates a problem with my grammar, but why doesn't the user use a singe begin expression to combine many?
            return new Body(definitions, Expression());
        }

        private Expression Expression()
        {
            string tokenValue = CurrentToken().Value;

            if (Match(TokenType.Number))
                return new NumberLiteral(double.Parse(tokenValue, CultureInfo.InvariantCulture));

            else if (Match(TokenType.String))
                return new StringLiteral(tokenValue);

            else if (Match(TokenType.Boolean))
                return new BooleanLiteral(bool.Parse(tokenValue));

            else if (Match(TokenType.Identifier))
                return new Identifier(tokenValue);

            else
            {
                if (!Match(TokenType.LParen))
                    throw new InvalidOperationException("Invalid expression.");

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
                    return BeginExpr();

                else if (Match(TokenType.Lambda))
                    return LambdaExpr();

                // let is a derived expression, so it gets converted to an equivalent lambda application.
                else if (Match(TokenType.Let))
                    return LetExpr().ToLambdaApplication();

                // If it is none of the above, then it can only be a function application.
                else
                    return ApplicationExpr();
            }
        }

        private ConditionClause ConditionClause()
        {
            Require(TokenType.LParen);

            var predicate = Expression();
            var expressions = ExpressionsUntilRightParenthesis();

            return new ConditionClause(predicate, expressions);
        }

        private ConditionElseClause ConditionElseClause()
        {
            Require(TokenType.LParen);
            Require(TokenType.Else);

            var expressions = ExpressionsUntilRightParenthesis();

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
            string identifierName = CurrentToken().Value;

            Require(TokenType.Identifier);
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

        private Begin BeginExpr()
        {
            var expressions = ExpressionsUntilRightParenthesis();

            return new Begin(expressions);
        }

        private Lambda LambdaExpr()
        {
            Require(TokenType.LParen);

            List<string>? parameters = null;

            // Optional lambda parameters
            if (!Match(TokenType.RParen))
            {
                parameters = new() { CurrentToken().Value };
                Require(TokenType.Identifier);

                while (!Match(TokenType.RParen))
                {
                    parameters.Add(CurrentToken().Value);
                    Require(TokenType.Identifier);
                }
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

        private List<Expression> ExpressionsUntilRightParenthesis()
        {
            var expressions = new List<Expression>();

            do
            {
                expressions.Add(Expression());
            } while (!Match(TokenType.RParen));

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
                throw new InvalidOperationException($"Missing required symbol {tokenType}.");
        }
        #endregion
    }
}