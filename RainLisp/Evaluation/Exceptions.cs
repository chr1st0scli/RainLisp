namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents an exception during the evaluation of an abstract syntax tree.
    /// </summary>
    public class EvaluationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationException"/> class.
        /// </summary>
        protected EvaluationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected EvaluationException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        protected EvaluationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets or sets the call stack that is associated with the evaluation error.
        /// </summary>
        public IList<IDebugInfo>? CallStack { get; private set; }

        /// <summary>
        /// Adds debugging info to the call stack to progressively reproduce the whole call stack that caused the error.
        /// </summary>
        /// <param name="debugInfo">The debugging info to add to the call stack of the error.</param>
        public void AddToCallStack(IDebugInfo debugInfo)
        {
            if (!debugInfo.HasDebugInfo)
                return;

            CallStack ??= new List<IDebugInfo>();
            CallStack.Add(debugInfo);
        }
    }

    /// <summary>
    /// Represents an exception that occurs when a procedure is called with the wrong number of arguments.
    /// </summary>
    public class WrongNumberOfArgumentsException : EvaluationException
    {
        /// <summary>
        /// Intitializes a new instance of the <see cref="WrongNumberOfArgumentsException"/> class.
        /// </summary>
        /// <param name="actual">The number of arguments passed.</param>
        /// <param name="expected">The number of expected arguments.</param>
        /// <param name="orMore">Specifies whether the procedure accepts more arguments than <paramref name="expected"/>; default is false.</param>
        public WrongNumberOfArgumentsException(int actual, int expected, bool orMore = false)
        {
            Actual = actual;
            Expected = expected;
            OrMore = orMore;
        }

        /// <summary>
        /// Intitializes a new instance of the <see cref="WrongNumberOfArgumentsException"/> class.
        /// </summary>
        /// <param name="actual">The number of arguments passed.</param>
        /// <param name="expected">The number of expected arguments.</param>
        /// <param name="orMore">Specifies whether the procedure accepts more arguments than <paramref name="expected"/>; default is false.</param>
        /// <param name="message">The message that describes the error.</param>
        public WrongNumberOfArgumentsException(int actual, int expected, bool orMore, string? message) : base(message)
        {
            Actual = actual;
            Expected = expected;
            OrMore = orMore;
        }

        /// <summary>
        /// Intitializes a new instance of the <see cref="WrongNumberOfArgumentsException"/> class.
        /// </summary>
        /// <param name="actual">The number of arguments passed.</param>
        /// <param name="expected">The number of expected arguments.</param>
        /// <param name="orMore">Specifies whether the procedure accepts more arguments than <paramref name="expected"/>; default is false.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public WrongNumberOfArgumentsException(int actual, int expected, bool orMore, string? message, Exception? innerException) : base(message, innerException)
        {
            Actual = actual;
            Expected = expected;
            OrMore = orMore;
        }

        /// <summary>
        /// Gets or sets the actual number of arguments passed.
        /// </summary>
        public int Actual { get; init; }

        /// <summary>
        /// Gets or sets the number of expected arguments.
        /// </summary>
        public int Expected { get; init; }

        /// <summary>
        /// Gets or sets whether the procedure accepts more arguments than <see cref="Expected"/>.
        /// </summary>
        public bool OrMore { get; init; }
    }

    /// <summary>
    /// Represents an exception that occurs when a procedure is called with the wrong type of argument.
    /// </summary>
    public class WrongTypeOfArgumentException : EvaluationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongTypeOfArgumentException"/> class.
        /// </summary>
        /// <param name="actual">The type of the actual argument passed.</param>
        /// <param name="expected">One or more alternatives of the expected type.</param>
        public WrongTypeOfArgumentException(Type actual, Type[] expected)
        {
            Actual = actual;
            Expected = expected;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongTypeOfArgumentException"/> class.
        /// </summary>
        /// <param name="actual">The type of the actual argument passed.</param>
        /// <param name="expected">One or more alternatives of the expected type.</param>
        /// <param name="message">The message that describes the error.</param>
        public WrongTypeOfArgumentException(Type actual, Type[] expected, string? message) : base(message)
        {
            Actual = actual;
            Expected = expected;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongTypeOfArgumentException"/> class.
        /// </summary>
        /// <param name="actual">The type of the actual argument passed.</param>
        /// <param name="expected">One or more alternatives of the expected type.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public WrongTypeOfArgumentException(Type actual, Type[] expected, string? message, Exception? innerException) : base(message, innerException)
        {
            Actual = actual;
            Expected = expected;
        }

        /// <summary>
        /// Gets or sets the type of the actual argument passed.
        /// </summary>
        public Type Actual { get; init; }

        /// <summary>
        /// Gets or sets the types that would be acceptable.
        /// </summary>
        public Type[] Expected { get; init; }
    }

    /// <summary>
    /// Represents an exception that occurs when an undefined identifier is evaluated.
    /// </summary>
    public class UnknownIdentifierException : EvaluationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownIdentifierException"/> class.
        /// </summary>
        /// <param name="identifierName">The identifier name that is not visible in the current scope.</param>
        public UnknownIdentifierException(string identifierName)
            => IdentifierName = identifierName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownIdentifierException"/> class.
        /// </summary>
        /// <param name="identifierName">The identifier name that is not visible in the current scope.</param>
        /// <param name="message">The message that describes the error.</param>
        public UnknownIdentifierException(string identifierName, string? message) : base(message)
            => IdentifierName = identifierName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownIdentifierException"/> class.
        /// </summary>
        /// <param name="identifierName">The identifier name that is not visible in the current scope.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public UnknownIdentifierException(string identifierName, string? message, Exception? innerException) : base(message, innerException)
            => IdentifierName = identifierName;

        /// <summary>
        /// Gets or sets the identifier name that is not visible in the current scope.
        /// </summary>
        public string IdentifierName { get; init; }
    }

    /// <summary>
    /// Represents an exception that occurs when a procedure application is evaluated on a value that is not a procedure.
    /// </summary>
    public class NotProcedureException : EvaluationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotProcedureException"/> class.
        /// </summary>
        public NotProcedureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotProcedureException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotProcedureException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotProcedureException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public NotProcedureException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents an exception that is thrown explicitly by the user.
    /// </summary>
    public class UserException : EvaluationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserException"/> class.
        /// </summary>
        public UserException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UserException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public UserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents an exception that occurs when a procedure is called with a wrong argument value.
    /// </summary>
    public class InvalidValueException : EvaluationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException"/> class.
        /// </summary>
        public InvalidValueException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidValueException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidValueException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
