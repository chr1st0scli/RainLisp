using RainLisp;

namespace RainLispTests
{
    public class EvaluatorTests
    {
        private readonly IVisitor _evaluator;
        private readonly Parser _parser;

        public EvaluatorTests()
        {
            _evaluator = new EvaluatorVisitor();
            _parser = new Parser();
        }

        [Theory]
        [InlineData("1", 1d)]
        [InlineData("10.54", 10.54)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("\"hello world\"", "\"hello world\"")]
        public void Evaluate_Literal_Correctly(string expression, object expectedResult)
        {
            // Arrange
            // Act
            var tokens = Tokenizer.TokenizeExt(expression);
            var ast = _parser.Parse(tokens);
            var result = _evaluator.VisitProgram(ast);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("(if true 1)", 1d)]
        [InlineData("(if true 1 0)", 1d)]
        [InlineData("(if false 1 0)", 0d)]
        [InlineData("(begin 0)", 0d)]
        [InlineData("(begin 0 1)", 1d)]
        [InlineData("(begin 0 1 2)", 2d)]
        public void Evaluate_Expression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var tokens = Tokenizer.TokenizeExt(expression);
            var ast = _parser.Parse(tokens);
            var result = _evaluator.VisitProgram(ast);

            // Assert
            Assert.Equal(expectedResult, (double)result);
        }
    }
}
