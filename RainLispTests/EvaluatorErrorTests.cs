using RainLisp;
using RainLisp.Evaluation;
using RainLisp.Parsing;
using RainLisp.Tokenization;

namespace RainLispTests
{
    public class EvaluatorErrorTests
    {
        private readonly Interpreter _interpreter = new();

        [Theory]
        [InlineData("\"hello", typeof(NonTerminatedStringException))]
        [InlineData("\"hello \\a", typeof(InvalidEscapeSequenceException))]
        [InlineData("\"hello \n", typeof(InvalidStringCharacterException))]
        [InlineData("123a", typeof(InvalidNumberCharacterException))]
        [InlineData("(foo", typeof(ParsingException))]
        [InlineData("(cond (true 1)", typeof(ParsingException))]
        [InlineData("(+)", typeof(WrongNumberOfArgumentsException))]
        [InlineData("(cons 1)", typeof(WrongNumberOfArgumentsException))]
        [InlineData("(- 21 true)", typeof(WrongTypeOfArgumentException))]
        [InlineData("(> true 21)", typeof(WrongTypeOfArgumentException))]
        [InlineData("a", typeof(UnknownIdentifierException))]
        [InlineData("(set! a 2)", typeof(UnknownIdentifierException))]
        [InlineData("(1)", typeof(NotProcedureException))]
        [InlineData("(error 1)", typeof(UserException))]
        [InlineData("(round 2.5 -1)", typeof(InvalidValueException))]
        public void EvaluateAndPrint_WrongExpression_ReportsError(string expression, Type expectedExceptionType)
        {
            // Arrange
            IEvaluationEnvironment? environment = null;
            Exception? exception = null;
            void PrintError(string message, Exception ex) => exception = ex;

            // Act
            _interpreter.EvaluateAndPrint(expression, ref environment, str => { }, PrintError);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType(expectedExceptionType, exception);
        }
    }
}
