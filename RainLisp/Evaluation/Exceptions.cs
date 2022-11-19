namespace RainLisp.Evaluation
{
    //public class ProcedureCallException : Exception
    //{
    //    public ProcedureCallException(string procedureName)
    //        => ProcedureName = procedureName;

    //    public string ProcedureName { get; init; }
    //}

    public class WrongNumberOfArgumentsException : Exception
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

    public class WrongTypeOfArgumentException : Exception
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

    public class UnknownIdentifierException : Exception
    {
        public UnknownIdentifierException(string identifierName)
            => IdentifierName = identifierName;

        public UnknownIdentifierException(string identifierName, string? message) : base(message)
            => IdentifierName = identifierName;

        public UnknownIdentifierException(string identifierName, string? message, Exception? innerException) : base(message, innerException)
            => IdentifierName = identifierName;

        public string IdentifierName { get; init; }
    }

    public class NotProcedureException : Exception
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

    public class UserException : Exception
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

    public class InvalidValueException : Exception
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
