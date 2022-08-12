using RainLisp;
using RainLisp.Evaluator;

namespace RainLispTests
{
    public class EvaluatorTests
    {
        private readonly IEvaluatorVisitor _evaluator;
        private readonly Parser _parser;

        public EvaluatorTests()
        {
            _evaluator = new EvaluatorVisitor(new ProcedureApplicationVisitor());
            _parser = new Parser();
        }

        private object Evaluate(string expression)
        {
            var tokens = Tokenizer.TokenizeExt(expression);
            var ast = _parser.Parse(tokens);
            return _evaluator.EvaluateProgram(ast);
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
            var result = Evaluate(expression);

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
        [InlineData("(define a 1) (set! a 2) a", 2d)]
        [InlineData("(define a 1) (set! a (+ a 3)) a", 4d)]
        [InlineData("(cond (true 1))", 1d)]
        [InlineData("(cond (false 1) (true 2))", 2d)]
        [InlineData("(cond (false 1) (false 2) (else 3))", 3d)]
        [InlineData("(cond (true 1 2))", 2d)]
        [InlineData("(cond (false 1 2) (true 3 4))", 4d)]
        [InlineData("(cond (false 1 2) (false 3 4) (else 5 6))", 6d)]
        [InlineData("(define a 1) (cond ((<= a 5) 5) ((<= a 10) 10) (else -1))", 5d)]
        [InlineData("(define a 7) (cond ((<= a 5) 5) ((<= a 10) 10) (else -1))", 10d)]
        [InlineData("(define a 28) (cond ((<= a 5) 5) ((<= a 10) 10) (else -1))", -1d)]
        public void Evaluate_Expression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, (double)result);
        }

        [Theory]
        [InlineData("(+ 1 2)", 3d)]
        [InlineData("(+ -1 2)", 1d)]
        [InlineData("(+ -1 -2)", -3d)]
        [InlineData("(+ 1 2 3 4)", 10d)]
        [InlineData("(- 5 2 1)", 2d)]
        [InlineData("(- 5 8)", -3d)]
        [InlineData("(+ -5 -8)", -13d)]
        [InlineData("(- -5 -8)", 3d)]
        [InlineData("(* 5 8)", 40d)]
        [InlineData("(* 5 -8)", -40d)]
        [InlineData("(* 2 3 4)", 24d)]
        [InlineData("(/ 6 3)", 2d)]
        [InlineData("(/ 24 6 2)", 2d)]
        [InlineData("(/ 3 2)", 1.5)]
        [InlineData("(/ -3 2)", -1.5)]
        [InlineData("(% 4 2)", 0d)]
        [InlineData("(% 5 2)", 1d)]
        [InlineData("(% 15 6 2)", 1d)]
        [InlineData("(+ 1 (* 2 3))", 7d)]
        [InlineData("(+ 6 (- 7 (* (+ 8 (/ 3 (+ 2 (- 1 (+ (- 5 (* 3 6)) 3)))) 4) 9)))", -97.08)]
        public void Evaluate_NumberPrimitiveExpression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, Math.Round((double)result, 2, MidpointRounding.AwayFromZero));
        }

        [Theory]
        [InlineData("(> 2 1)", true)]
        [InlineData("(>= 2 1)", true)]
        [InlineData("(>= 2 2)", true)]
        [InlineData("(> 2 3)", false)]
        [InlineData("(>= 2 3)", false)]
        [InlineData("(< 1 2)", true)]
        [InlineData("(<= 1 2)", true)]
        [InlineData("(<= 2 2)", true)]
        [InlineData("(< 3 2)", false)]
        [InlineData("(<= 3 2)", false)]
        [InlineData("(not true)", false)]
        [InlineData("(not false)", true)]
        [InlineData("(not (not true))", true)]
        [InlineData("(and true false)", false)]
        [InlineData("(and false true)", false)]
        [InlineData("(and true true)", true)]
        [InlineData("(and false false)", false)]
        [InlineData("(and true true true)", true)]
        [InlineData("(and true false true)", false)]
        [InlineData("(or true false)", true)]
        [InlineData("(or false true)", true)]
        [InlineData("(or true true)", true)]
        [InlineData("(or false false)", false)]
        [InlineData("(or true true true)", true)]
        [InlineData("(or true false true)", true)]
        [InlineData("(xor true false)", true)]
        [InlineData("(xor false true)", true)]
        [InlineData("(xor true true)", false)]
        [InlineData("(xor false false)", false)]
        [InlineData("(xor false false true)", true)]
        [InlineData("(and (or true false) false)", false)]
        [InlineData("(or (or true false) false)", true)]
        [InlineData("(or (and true false) false)", false)]
        [InlineData("(or (and true false) true)", true)]
        [InlineData("(or (and true false) (and false true))", false)]
        [InlineData("(and (or true false) (or false true))", true)]
        [InlineData("(not (and (or true false) (or false true)))", false)]
        [InlineData("(and (or (> 3 1) (>= 1 4)) (< 5 2))", false)]
        [InlineData("(or (or (>= 6 2) (<= 9 4)) (< 7 4))", true)]
        [InlineData("(or (and (> 9 3) (> 9 11)) (< 9 5))", false)]
        [InlineData("(or (and (> 10 8) (< 11 19)) true)", true)]
        [InlineData("(or (and (>= 10 10) (<= 10 9)) (and (> 5 6) (<= 4 4)))", false)]
        [InlineData("(and (or (>= 5 5) (> 5 5)) (or (> 5 6) (<= 5 6)))", true)]
        [InlineData("(not (and (or (> 6 2) (< 6 2)) (or (< 4 2) (> 4 2))))", false)]
        [InlineData("(and (> (+ 5 4) (- 5 4)) (> (/ 4 5) (* 4 5)))", false)]
        [InlineData("(or (> (+ 5 4) (- 5 4)) (> (/ 4 5) (* 4 5)))", true)]
        public void Evaluate_LogicalPrimitiveExpression_Correctly(string expression, bool expectedResult)
        {
            // Arrange
            // Act
            var result = Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, (bool)result);
        }
    }
}
