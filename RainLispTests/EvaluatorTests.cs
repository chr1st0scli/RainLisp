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
        [InlineData("-0.5", -0.5)]
        [InlineData("+0.5", 0.5)]
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
        [InlineData("(define (foo) 1) (foo)", 1d)]
        [InlineData("(define (foo x y) (+ x y)) (foo 3 4)", 7d)]
        [InlineData("(+ 1 2)", 3d)]
        [InlineData("(+ -1 2)", 1d)]
        [InlineData("(+ 1 2 3 4)", 10d)]
        [InlineData("(- 5 2 1)", 2d)]
        [InlineData("(- 5 8)", -3d)]
        [InlineData("(* 5 8)", 40d)]
        [InlineData("(* 2 3 4)", 24d)]
        [InlineData("(/ 6 3)", 2d)]
        [InlineData("(/ 24 6 2)", 2d)]
        [InlineData("(/ 3 2)", 1.5)]
        [InlineData("(% 4 2)", 0d)]
        [InlineData("(% 5 2)", 1d)]
        [InlineData("(% 15 6 2)", 1d)]
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
