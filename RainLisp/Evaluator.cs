namespace RainLisp
{
    public class Evaluator
    {
        public Expression Evaluate(Expression expression, Environment environment)
        {
            if (IsSelfEvaluating(expression))
                return expression;

            else if (IsVariable(expression))
                return LookupVariableValue(expression, environment);

            else if (IsQuoted(expression))
                return GetTextOfQuotation(expression);

            else if (IsAssignment(expression))
                return EvaluateAssignment(expression, environment);

            else if (IsDefinition(expression))
                return EvaluateDefinition(expression, environment);

            else if (IsIf(expression))
                return EvaluateIf(expression, environment);

            else if (IsLambda(expression))
                return MakeProcedure(expression, environment);

            else if (IsBegin(expression))
                return EvaluateSequence(expression, environment);

            else if (IsCondition(expression))
                return Evaluate(MakeIfExpression(expression), environment);

            else if (IsApplication(expression))
                return Apply(expression, environment);

            else
                throw new InvalidOperationException("Unknown expression.");
        }

        private Expression Apply(Expression expression, Environment environment)
        {
            Expression oper = GetApplicationOperator(expression);
            Procedure procedure = GetProcedure(Evaluate(oper, environment));

            Expression[] args = GetApplicationOperands(expression);
            var argValues = args.Select(arg => Evaluate(arg, environment)).ToArray();

            return Apply(procedure, args);
        }

        private Expression Apply(Procedure procedure, Expression[] arguments)
        {
            if (IsPrimitive(procedure))
                return ApplyPrimitive(procedure, arguments);

            else if (IsCompound(procedure))
            {
                var extendedEnv = ExtendEnvironment(
                    GetParameters(procedure),
                    arguments,
                    GetEnvironment(procedure));

                return EvaluateSequence(GetBody(procedure), extendedEnv);
            }
            else
                throw new InvalidOperationException("Uknown procedure type.");
        }

        private Procedure GetProcedure(Expression expression)
        {
            throw new NotImplementedException();
        }

        private Expression[] GetApplicationOperands(Expression expression)
        {
            throw new NotImplementedException();
        }

        private Expression GetApplicationOperator(Expression expression)
        {
            throw new NotImplementedException();
        }

        private Environment ExtendEnvironment(string[] parameters, Expression[] arguments, Environment environment)
        {
            throw new NotImplementedException();
        }

        private string[] GetParameters(Procedure procedure)
        {
            throw new NotImplementedException();
        }

        private Environment GetEnvironment(Procedure procedure)
        {
            throw new NotImplementedException();
        }

        private Expression GetBody(Procedure procedure)
        {
            throw new NotImplementedException();
        }

        private bool IsCompound(Procedure procedure)
        {
            throw new NotImplementedException();
        }

        private Expression ApplyPrimitive(Procedure procedure, Expression[] arguments)
        {
            throw new NotImplementedException();
        }

        private bool IsPrimitive(Procedure procedure)
        {
            throw new NotImplementedException();
        }

        private Expression GetOperator(Expression expression)
        {
            throw new NotImplementedException();
        }

        private bool IsApplication(Expression expression)
        {
            throw new NotImplementedException();
        }

        private Expression MakeIfExpression(Expression expression)
        {
            throw new NotImplementedException();
        }

        private bool IsCondition(Expression expression)
        {
            throw new NotImplementedException();
        }

        private Expression EvaluateSequence(Expression expression, Environment environment)
        {
            Expression[] actions = GetActions(expression);
            return EvaluateSequence(actions, environment);
        }

        private Expression EvaluateSequence(Expression[] expressions, Environment environment)
        {
            if (expressions == null || expressions.Length == 0)
                throw new ArgumentException("No expressions", nameof(expressions));

            // Evaluate expressions in the order they occur
            Expression result = null!;
            foreach (var expression in expressions)
                result = Evaluate(expression, environment);

            return result!;
        }

        private Expression[] GetActions(Expression expression)
        {
            throw new NotImplementedException();
        }

        private bool IsBegin(Expression expression)
        {
            return IsTaggedList(expression, "begin");
        }

        private Expression MakeProcedure(Expression expression, Environment environment)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);

            var parameters = tokens.Skip(3).TakeWhile(t => t != ")");
            var body = tokens.Skip(3).SkipWhile(t => t != ")").Skip(1).SkipLast(1);

            var proc = new Procedure 
            {
                Parameters = parameters.ToArray(),
                Body = new Expression { ExpressionText = string.Join("", body) },
                Environment = environment
            };
            return proc;
        }

        private bool IsLambda(Expression expression)
        {
            return IsTaggedList(expression, "lambda");
        }

        private Expression EvaluateIf(Expression expression, Environment environment)
        {
            Expression ifPredicate = GetIfPredicate(expression);
            var result = Evaluate(ifPredicate, environment);
            Expression expressionToEvaluate;
            if (GetBooleanValue(result))
                expressionToEvaluate = GetConsequent(ifPredicate);
            else
                expressionToEvaluate = GetAlternative(ifPredicate);

            return Evaluate(expressionToEvaluate, environment);
        }

        private Expression GetAlternative(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            // wrong
            // Also take into account that an alternative part might be absent.
            return new Expression { ExpressionText = string.Join("", tokens.Skip(8).Take(1)) };
        }

        private Expression GetConsequent(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            // wrong
            return new Expression { ExpressionText = string.Join("", tokens.Skip(7).Take(1)) };
        }

        private bool GetBooleanValue(Expression result)
        {
            throw new NotImplementedException();
        }

        private Expression GetIfPredicate(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            // This is wrong, we need a parser to extract the parts correctly.
            return new Expression { ExpressionText = string.Join("", tokens.Skip(2).TakeWhile(t => t != ")").Take(1)) };
        }

        private bool IsIf(Expression expression)
        {
            return IsTaggedList(expression, "if");
        }

        private bool IsDefinition(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            return IsTaggedList(expression, "define");
        }

        private Expression EvaluateDefinition(Expression expression, Environment environment)
        {
            string variable = GetDefinitionVariable(expression);
            Expression value = GetDefinitionValue(expression);

            var valueToSet = Evaluate(value, environment);

            DefineVariable(variable, valueToSet, environment);

            // A do nothing expression, e.g. 'ok
            return new Expression();
        }

        private void DefineVariable(string variable, Expression valueToSet, Environment environment)
        {
            throw new NotImplementedException();
        }

        private Expression GetDefinitionValue(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            string variable = tokens[2];
            if (IsSymbol(new Expression { ExpressionText = variable }))
                return new Expression { ExpressionText = tokens[3] };
            else
            {
                var parameters = tokens.Skip(3).TakeWhile(t => t != ")");
                var body = tokens.Skip(3).SkipWhile(t => t != ")").Skip(1).SkipLast(1);
                return MakeLambda(parameters, body);
            }
        }

        private Expression MakeLambda(IEnumerable<string> parameters, IEnumerable<string> body)
        {
            return new Expression { ExpressionText = $"(lambda ({parameters}) {body}" };
        }

        private string GetDefinitionVariable(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            string variable = tokens[2];
            if (IsSymbol(new Expression { ExpressionText = variable }))
                return variable;    // name of symbol
            else
                return tokens[3];   // name of function
        }

        private Expression EvaluateAssignment(Expression expression, Environment environment)
        {
            string variable = GetAssignmentVariable(expression);
            Expression value = GetAssignmentValue(expression);
            
            var valueToSet = Evaluate(value, environment);

            SetVariableValue(variable, valueToSet, environment);

            // A do nothing expression, e.g. 'ok
            return new Expression();
        }

        private void SetVariableValue(string variable, Expression valueToSet, Environment environment)
        {
            throw new NotImplementedException();
        }

        private Expression GetAssignmentValue(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            return new Expression() { ExpressionText = tokens[3] };
        }

        private string GetAssignmentVariable(Expression expression)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);
            return tokens[2];
        }

        private bool IsAssignment(Expression expression)
        {
            return IsTaggedList(expression, "set!");
        }

        private Expression GetTextOfQuotation(Expression expression)
        {
            throw new NotImplementedException();
        }

        private bool IsQuoted(Expression expression)
        {
            return IsTaggedList(expression, "quote");
        }

        private bool IsTaggedList(Expression expression, string tag)
        {
            var tokens = Tokenizer.Tokenize(expression.ExpressionText);

            return tokens[0] == "(" && tokens[1] == tag && tokens[tokens.Count - 1] == ")";
        }

        private Expression LookupVariableValue(Expression expression, Environment environment)
        {
            throw new NotImplementedException();
        }

        private bool IsSelfEvaluating(Expression expression)
        {
            if (expression.ExpressionText.StartsWith("\"") && expression.ExpressionText.EndsWith("\""))
                return true;

            if (double.TryParse(expression.ExpressionText, out var _))
                return true;

            return false;
        }

        private bool IsVariable(Expression expression) => IsSymbol(expression);

        private bool IsSymbol(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}