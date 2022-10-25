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

        public int Actual { get; init; }

        public int Expected { get; init; }

        public bool OrMore { get; init; }
    }

    public class WrongTypeOfArgumentException : Exception
    {
        public WrongTypeOfArgumentException(Type actual, Type expected)
        {
            Expected = expected;
            Actual = actual;
        }

        public Type Actual { get; init; }

        public Type Expected { get; init; }
    }

    public class UnknownIdentifierException : Exception
    {
        public UnknownIdentifierException(string identifierName)
            => IdentifierName = identifierName;

        public string IdentifierName { get; init; }
    }
}
