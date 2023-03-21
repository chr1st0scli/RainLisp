namespace RainLisp
{
    /// <summary>
    /// Encapsulates a method accepting the string representation of an evaluation result.
    /// </summary>
    /// <param name="result">The string representation of the evaluation result.</param>
    public delegate void PrintResult(string result);

    /// <summary>
    /// Encapsulates a method accepting information relating to an error during evaluation.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="exception">The actual exception.</param>
    /// <param name="unknownError">true if the exception was not anticipated; otherwise, false. Default value is false.</param>
    public delegate void PrintError(string message, Exception exception, bool unknownError = false);
}
