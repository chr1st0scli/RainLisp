using RainLisp.AbstractSyntaxTree;

namespace RainLisp
{
    //public class Evaluator
    //{
    //    public Node Evaluate(Node expression, Environment environment)
    //    {
    //        if (IsSelfEvaluating(expression))
    //            return expression;

    //        else if (IsVariable(expression))
    //            return LookupVariableValue(expression, environment);

    //        else if (IsQuoted(expression))
    //            return GetTextOfQuotation(expression);

    //        else if (IsAssignment(expression))
    //            return EvaluateAssignment(expression, environment);

    //        else if (IsDefinition(expression))
    //            return EvaluateDefinition(expression, environment);

    //        else if (IsIf(expression))
    //            return EvaluateIf(expression, environment);

    //        //else if (IsLambda(expression))
    //        //    return MakeProcedure(expression, environment);

    //        else if (IsBegin(expression))
    //            return EvaluateSequence(expression, environment);

    //        else if (IsCondition(expression))
    //            return Evaluate(MakeIfExpression(expression), environment);

    //        else if (IsApplication(expression))
    //            return Apply(expression, environment);

    //        else
    //            throw new InvalidOperationException("Unknown expression.");
    //    }

    //    private Node Apply(Node expression, Environment environment)
    //    {
    //        Node oper = GetApplicationOperator(expression);
    //        Procedure procedure = GetProcedure(Evaluate(oper, environment));

    //        Node[] args = GetApplicationOperands(expression);
    //        var argValues = args.Select(arg => Evaluate(arg, environment)).ToArray();

    //        return Apply(procedure, args);
    //    }

    //    private Node Apply(Procedure procedure, Node[] arguments)
    //    {
    //        if (IsPrimitive(procedure))
    //            return ApplyPrimitive(procedure, arguments);

    //        else if (IsCompound(procedure))
    //        {
    //            var extendedEnv = ExtendEnvironment(
    //                GetParameters(procedure),
    //                arguments,
    //                GetEnvironment(procedure));

    //            return EvaluateSequence(GetBody(procedure), extendedEnv);
    //        }
    //        else
    //            throw new InvalidOperationException("Uknown procedure type.");
    //    }

    //    private Procedure GetProcedure(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node[] GetApplicationOperands(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node GetApplicationOperator(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Environment ExtendEnvironment(string[] parameters, Node[] arguments, Environment environment)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private string[] GetParameters(Procedure procedure)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Environment GetEnvironment(Procedure procedure)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node GetBody(Procedure procedure)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private bool IsCompound(Procedure procedure)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node ApplyPrimitive(Procedure procedure, Node[] arguments)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private bool IsPrimitive(Procedure procedure)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node GetOperator(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private bool IsApplication(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node MakeIfExpression(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private bool IsCondition(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node EvaluateSequence(Node expression, Environment environment)
    //    {
    //        Node[] actions = GetActions(expression);
    //        return EvaluateSequence(actions, environment);
    //    }

    //    private Node EvaluateSequence(Node[] expressions, Environment environment)
    //    {
    //        if (expressions == null || expressions.Length == 0)
    //            throw new ArgumentException("No expressions", nameof(expressions));

    //        // Evaluate expressions in the order they occur
    //        Node result = null!;
    //        foreach (var expression in expressions)
    //            result = Evaluate(expression, environment);

    //        return result!;
    //    }

    //    private Node[] GetActions(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private bool IsBegin(Node expression)
    //    {
    //        return IsTaggedList(expression, "begin");
    //    }

    //    private Procedure MakeProcedure(Node expression, Environment environment)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);

    //        var parameters = tokens.Skip(3).TakeWhile(t => t != ")");
    //        var body = tokens.Skip(3).SkipWhile(t => t != ")").Skip(1).SkipLast(1);

    //        var proc = new Procedure 
    //        {
    //            Parameters = parameters.ToArray(),
    //            Body = new Node { ExpressionText = string.Join("", body) },
    //            Environment = environment
    //        };
    //        return proc;
    //    }

    //    private bool IsLambda(Node expression)
    //    {
    //        return IsTaggedList(expression, "lambda");
    //    }

    //    private Node EvaluateIf(Node expression, Environment environment)
    //    {
    //        Node ifPredicate = GetIfPredicate(expression);
    //        var result = Evaluate(ifPredicate, environment);
    //        Node expressionToEvaluate;
    //        if (GetBooleanValue(result))
    //            expressionToEvaluate = GetConsequent(ifPredicate);
    //        else
    //            expressionToEvaluate = GetAlternative(ifPredicate);

    //        return Evaluate(expressionToEvaluate, environment);
    //    }

    //    private Node GetAlternative(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        // wrong
    //        // Also take into account that an alternative part might be absent.
    //        return new Node { ExpressionText = string.Join("", tokens.Skip(8).Take(1)) };
    //    }

    //    private Node GetConsequent(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        // wrong
    //        return new Node { ExpressionText = string.Join("", tokens.Skip(7).Take(1)) };
    //    }

    //    private bool GetBooleanValue(Node result)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node GetIfPredicate(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        // This is wrong, we need a parser to extract the parts correctly.
    //        return new Node { ExpressionText = string.Join("", tokens.Skip(2).TakeWhile(t => t != ")").Take(1)) };
    //    }

    //    private bool IsIf(Node expression)
    //    {
    //        return IsTaggedList(expression, "if");
    //    }

    //    private bool IsDefinition(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        return IsTaggedList(expression, "define");
    //    }

    //    private Node EvaluateDefinition(Node expression, Environment environment)
    //    {
    //        string variable = GetDefinitionVariable(expression);
    //        Node value = GetDefinitionValue(expression);

    //        var valueToSet = Evaluate(value, environment);

    //        DefineVariable(variable, valueToSet, environment);

    //        // A do nothing expression, e.g. 'ok
    //        return new Node();
    //    }

    //    private void DefineVariable(string variable, Node valueToSet, Environment environment)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node GetDefinitionValue(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        string variable = tokens[2];
    //        if (IsSymbol(new Node { ExpressionText = variable }))
    //            return new Node { ExpressionText = tokens[3] };
    //        else
    //        {
    //            var parameters = tokens.Skip(3).TakeWhile(t => t != ")");
    //            var body = tokens.Skip(3).SkipWhile(t => t != ")").Skip(1).SkipLast(1);
    //            return MakeLambda(parameters, body);
    //        }
    //    }

    //    private Node MakeLambda(IEnumerable<string> parameters, IEnumerable<string> body)
    //    {
    //        return new Node { ExpressionText = $"(lambda ({parameters}) {body}" };
    //    }

    //    private string GetDefinitionVariable(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        string variable = tokens[2];
    //        if (IsSymbol(new Node { ExpressionText = variable }))
    //            return variable;    // name of symbol
    //        else
    //            return tokens[3];   // name of function
    //    }

    //    private Node EvaluateAssignment(Node expression, Environment environment)
    //    {
    //        string variable = GetAssignmentVariable(expression);
    //        Node value = GetAssignmentValue(expression);
            
    //        var valueToSet = Evaluate(value, environment);

    //        SetVariableValue(variable, valueToSet, environment);

    //        // A do nothing expression, e.g. 'ok
    //        return new Node();
    //    }

    //    private void SetVariableValue(string variable, Node valueToSet, Environment environment)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Node GetAssignmentValue(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        return new Node() { ExpressionText = tokens[3] };
    //    }

    //    private string GetAssignmentVariable(Node expression)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);
    //        return tokens[2];
    //    }

    //    private bool IsAssignment(Node expression)
    //    {
    //        return IsTaggedList(expression, "set!");
    //    }

    //    private Node GetTextOfQuotation(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private bool IsQuoted(Node expression)
    //    {
    //        return IsTaggedList(expression, "quote");
    //    }

    //    private bool IsTaggedList(Node expression, string tag)
    //    {
    //        var tokens = Tokenizer.Tokenize(expression.ExpressionText);

    //        return tokens[0] == "(" && tokens[1] == tag && tokens[tokens.Count - 1] == ")";
    //    }

    //    private Node LookupVariableValue(Node expression, Environment environment)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private bool IsSelfEvaluating(Node expression)
    //    {
    //        if (expression.ExpressionText.StartsWith("\"") && expression.ExpressionText.EndsWith("\""))
    //            return true;

    //        if (double.TryParse(expression.ExpressionText, out var _))
    //            return true;

    //        return false;
    //    }

    //    private bool IsVariable(Node expression) => IsSymbol(expression);

    //    private bool IsSymbol(Node expression)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}