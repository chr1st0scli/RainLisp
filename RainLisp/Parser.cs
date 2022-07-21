using RainLisp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private bool Check(TokenType tokenType) => _tokens[currPosition].Type == tokenType;

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
                throw new InvalidOperationException($"Missing required symbol {tokenType.ToString()}.");
        }

        private void NextToken() => currPosition++;

        private void ChainAlternativeNonTerminals(params Action[] nonTerminalActions)
        {
            int currPositionSnapshot = currPosition;
            for (int i = 0; i < nonTerminalActions.Length; i++)
            {
                var action = nonTerminalActions[i];
                try
                {
                    action();
                    // if non terminal is matched stop
                    break;
                }
                catch
                {
                    currPosition = currPositionSnapshot;
                    // If the last non terminal is not matched, throw the exception.
                    if (i == nonTerminalActions.Length - 1)
                        throw;
                }
            }
        }

        private void Program()
        {
            do
            {
                ExprExt();
            } while (!Check(TokenType.EOF));
        }

        private void Quote()
        {
            Require(TokenType.LParen);
            Require(TokenType.Quote);
            // Can there be more than one?
            Require(TokenType.Identifier);
            Require(TokenType.RParen);
        }

        private void Assignment()
        {
            Require(TokenType.LParen);
            Require(TokenType.Assignment);
            Require(TokenType.Identifier);
            Expr();
            Require(TokenType.RParen);
        }

        private void Definition()
        {
            Require(TokenType.LParen);
            Require(TokenType.Definition);
            if (Match(TokenType.Identifier))
            {
                Expr();
            }
            else if (Match(TokenType.RParen))
            {
                // Method name
                Require(TokenType.Identifier);
                // Formal arguments
                while (Match(TokenType.Identifier))
                {

                }
                Require(TokenType.RParen);
                ExprExt();
            }
            else
                throw new InvalidOperationException("Expected either an identifier or ( symbol.");

            Require(TokenType.RParen);
        }

        private void If()
        {
            Require(TokenType.LParen);
            Require(TokenType.If);
            Cond();
            // Consequent
            ExprExt();

            // Optional alternative
            if (!Match(TokenType.RParen))
            {
                ExprExt();
                Require(TokenType.RParen);
            }
        }

        private void Cond()
        {
            if (!Match(TokenType.Boolean) && !Match(TokenType.Identifier))
            {
                ChainAlternativeNonTerminals(If, Block, Application);
            }
        }

        private void Application()
        {
            Require(TokenType.LParen);
            if (Match(TokenType.Identifier))
            {
                while (!Check(TokenType.RParen))
                    Expr();
            }
            else
            {
                Lambda();
                do
                {
                    Expr(); 
                } while (!Check(TokenType.RParen));
            }
            Require(TokenType.RParen);
        }

        private void Block()
        {
            Require(TokenType.LParen);
            Require(TokenType.Begin);
            ExprExt();
            
            // Rest of optional expressions.
            while (!Check(TokenType.RParen))
            {
                ExprExt();
            }
            Require(TokenType.RParen);
        }

        private void Lambda()
        {
            Require(TokenType.LParen);
            Require(TokenType.Lambda);
            Require(TokenType.LParen);

            // Optional lambda formal arguments
            while (Match(TokenType.Identifier))
            {

            }

            Require(TokenType.RParen);
            ExprExt();
            Require(TokenType.RParen);
        }

        private void ExprExt()
        {
            ChainAlternativeNonTerminals(Expr, Assignment, Definition);
        }

        private void Expr()
        {
            if (!Match(TokenType.Number) && 
                !Match(TokenType.String) && 
                !Match(TokenType.Boolean) &&
                !Match(TokenType.Identifier))
                ChainAlternativeNonTerminals(Quote, If, Block, Application);
        }
    }
}
