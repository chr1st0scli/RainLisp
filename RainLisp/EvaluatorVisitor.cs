using RainLisp.AbstractSyntaxTree;

namespace RainLisp
{
    public class EvaluatorVisitor : IVisitor
    {
        public void VisitApplication(Application application)
        {
            throw new NotImplementedException();
        }

        public void VisitAssignment(Assignment assignment)
        {
            throw new NotImplementedException();
        }

        public void VisitBegin(Begin begin)
        {
            throw new NotImplementedException();
        }

        public void VisitBody(Body body)
        {
            throw new NotImplementedException();
        }

        public void VisitBooleanLiteral(BooleanLiteral boolLiteral)
        {
            boolLiteral.EvaluationResult = boolLiteral.Value;
        }

        public void VisitDefinition(Definition definition)
        {
            throw new NotImplementedException();
        }

        public void VisitIdentifier(Identifier identifier)
        {
            throw new NotImplementedException();
        }

        public void VisitIf(If ifExpression)
        {
            ifExpression.Predicate.AcceptVisitor(this);

            if (ifExpression.Predicate.EvaluationResult is bool condition)
            {
                if (condition)
                {
                    ifExpression.Consequent.AcceptVisitor(this);
                    ifExpression.EvaluationResult = ifExpression.Consequent.EvaluationResult;
                }
                else if (ifExpression.Alternative != null)
                {
                    ifExpression.Alternative.AcceptVisitor(this);
                    ifExpression.EvaluationResult = ifExpression.Alternative.EvaluationResult;
                }
                else
                    ifExpression.EvaluationResult = "undefined";
            }
            else
                throw new InvalidOperationException();
        }

        public void VisitLambda(Lambda lambda)
        {
            throw new NotImplementedException();
        }

        public void VisitNumberLiteral(NumberLiteral numberLiteral)
        {
            numberLiteral.EvaluationResult = numberLiteral.Value;
        }

        public void VisitProgram(Program program)
        {
            throw new NotImplementedException();
        }

        public void VisitQuote(Quote quote)
        {
            throw new NotImplementedException();
        }

        public void VisitStringLiteral(StringLiteral stringLiteral)
        {
            stringLiteral.EvaluationResult = stringLiteral.Value;
        }
    }
}
