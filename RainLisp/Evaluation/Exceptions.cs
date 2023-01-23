namespace RainLisp.Evaluation
{
    public class EvaluationException : Exception
    {
        protected EvaluationException()
        {
        }

        protected EvaluationException(string? message) : base(message)
        {
        }

        protected EvaluationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public IList<IDebugInfo>? CallStack { get; private set; }

        public void AddToCallStack(IDebugInfo debugInfo)
        {
            if (!debugInfo.HasDebugInfo)
                return;

            CallStack ??= new List<IDebugInfo>();
            CallStack.Add(debugInfo);
        }
    }

    public class WrongNumberOfArgumentsException : EvaluationException
    {
        public WrongNumberOfArgumentsException(int actual, int expected, bool orMore = false)
        {
            Actual = actual;
            Expected = expected;
            OrMore = orMore;
        }

        public WrongNumberOfArgumentsException(int actual, int expected, bool orMore, string? message) : base(message)
        {
            Actual = actual;
            Expected = expected;
            OrMore = orMore;
        }

        public WrongNumberOfArgumentsException(int actual, int expected, bool orMore, string? message, Exception? innerException) : base(message, innerException)
        {
            Actual = actual;
            Expected = expected;
            OrMore = orMore;
        }

        public int Actual { get; init; }

        public int Expected { get; init; }

        public bool OrMore { get; init; }
    }

    public class WrongTypeOfArgumentException : EvaluationException
    {
        public WrongTypeOfArgumentException(Type actual, Type[] expected)
        {
            Actual = actual;
            Expected = expected;
        }

        public WrongTypeOfArgumentException(Type actual, Type[] expected, string? message) : base(message)
        {
            Actual = actual;
            Expected = expected;
        }

        public WrongTypeOfArgumentException(Type actual, Type[] expected, string? message, Exception? innerException) : base(message, innerException)
        {
            Actual = actual;
            Expected = expected;
        }

        public Type Actual { get; init; }

        public Type[] Expected { get; init; }
    }

    public class UnknownIdentifierException : EvaluationException
    {
        public UnknownIdentifierException(string identifierName)
            => IdentifierName = identifierName;

        public UnknownIdentifierException(string identifierName, string? message) : base(message)
            => IdentifierName = identifierName;

        public UnknownIdentifierException(string identifierName, string? message, Exception? innerException) : base(message, innerException)
            => IdentifierName = identifierName;

        public string IdentifierName { get; init; }
    }

    public class NotProcedureException : EvaluationException
    {
        public NotProcedureException()
        {
        }

        public NotProcedureException(string? message) : base(message)
        {
        }

        public NotProcedureException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class UserException : EvaluationException
    {
        public UserException()
        {
        }

        public UserException(string? message) : base(message)
        {
        }

        public UserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidValueException : EvaluationException
    {
        public InvalidValueException()
        {
        }

        public InvalidValueException(string? message) : base(message)
        {
        }

        public InvalidValueException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
