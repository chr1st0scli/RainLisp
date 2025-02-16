using RainLisp;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLispTests
{
    public class PrintEvaluationResultTests
    {
        private readonly Interpreter _interpreter = new();
        private readonly IEvaluationResultVisitor<string> _printVisitor = new EvaluationResultPrintVisitor();

        [Theory]
        [InlineData("0", "0")]
        [InlineData("1", "1")]
        [InlineData("21", "21")]
        [InlineData("21.64", "21.64")]
        [InlineData("213423.643243", "213423.643243")]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        [InlineData("\"\"", "\"\"")]
        [InlineData("\"hello world\"", "\"hello world\"")]
        [InlineData("\"hello\\nworld\"", "\"hello\\nworld\"")]
        [InlineData("\"hello\\rworld\"", "\"hello\\rworld\"")]
        [InlineData("\"hello\\r\\nworld\"", "\"hello\\r\\nworld\"")]
        [InlineData("\"hello\\tworld\"", "\"hello\\tworld\"")]
        [InlineData("\"hello\\\"world\"", "\"hello\\\"world\"")]
        [InlineData("\"hello\\\\world\"", "\"hello\\\\world\"")]
        [InlineData("(make-datetime 2022 12 31 10 30 45 100)", "2022-12-31 10:30:45.100")]
        public void Evaluate_ExpressionGivingPrimitive_PrintsCorrectly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.IsAssignableFrom<IPrimitiveDatum>(result);
            Assert.Equal(expectedResult, printedResult);
        }

        [Theory]
        [InlineData("+", "[PrimitiveProcedure] +")]
        [InlineData("-", "[PrimitiveProcedure] -")]
        [InlineData("*", "[PrimitiveProcedure] *")]
        [InlineData("/", "[PrimitiveProcedure] /")]
        [InlineData("%", "[PrimitiveProcedure] %")]
        [InlineData(">", "[PrimitiveProcedure] >")]
        [InlineData(">=", "[PrimitiveProcedure] >=")]
        [InlineData("<", "[PrimitiveProcedure] <")]
        [InlineData("<=", "[PrimitiveProcedure] <=")]
        [InlineData("=", "[PrimitiveProcedure] =")]
        [InlineData("xor", "[PrimitiveProcedure] xor")]
        [InlineData("not", "[PrimitiveProcedure] not")]
        [InlineData("cons", "[PrimitiveProcedure] cons")]
        [InlineData("car", "[PrimitiveProcedure] car")]
        [InlineData("cdr", "[PrimitiveProcedure] cdr")]
        [InlineData("list", "[PrimitiveProcedure] list")]
        [InlineData("null?", "[PrimitiveProcedure] null?")]
        [InlineData("set-car!", "[PrimitiveProcedure] set-car!")]
        [InlineData("set-cdr!", "[PrimitiveProcedure] set-cdr!")]
        public void Evaluate_ExpressionGivingPrimitiveProcedure_PrintsCorrectly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.IsType<PrimitiveProcedure>(result);
            Assert.Equal(expectedResult, printedResult);
        }

        [Theory]
        [InlineData("(define (foo) true) foo", "[UserProcedure] Parameters: 0")]
        [InlineData("(define (foo x) true) foo", "[UserProcedure] Parameters: x")]
        [InlineData("(define (foo x y) true) foo", "[UserProcedure] Parameters: x, y")]
        [InlineData("(define (foo ab cd ef) true) foo", "[UserProcedure] Parameters: ab, cd, ef")]
        [InlineData("(lambda (ab cd ef) true)", "[UserProcedure] Parameters: ab, cd, ef")]
        [InlineData("(delay true)", "[UserProcedure] Parameters: 0")]
        public void Evaluate_ExpressionGivingUserProcedure_PrintsCorrectly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.True(result is UserProcedure);
            Assert.Equal(expectedResult, printedResult);
        }

        [Fact]
        public void Evaluate_ExpressionGivingUnspecified_PrintsEmptyString()
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate("(define a 1)").Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.IsType<Unspecified>(result);
            Assert.Equal(string.Empty, printedResult);
        }

        [Theory]
        [InlineData("nil", "()")]
        [InlineData("(list)", "()")]
        public void Evaluate_ExpressionGivingNil_PrintsCorrectly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.IsType<Nil>(result);
            Assert.Equal(expectedResult, printedResult);
        }

        [Theory]
        [InlineData("(cons 1 nil)", "(1)")]
        [InlineData("(cons nil 1)", "(() . 1)")]
        [InlineData("(cons nil nil)", "(())")]
        [InlineData("(cons 1 2)", "(1 . 2)")]
        [InlineData("(cons 1 (cons 2 3))", "(1 2 . 3)")]
        [InlineData("(cons (cons 1 2) 3)", "((1 . 2) . 3)")]
        [InlineData("(cons (cons 1 2) (cons 3 4))", "((1 . 2) 3 . 4)")]
        [InlineData("(list 1 2)", "(1 2)")]
        [InlineData("(cons 1 (cons 2 nil))", "(1 2)")]
        [InlineData("(cons (list 1 2) (list 3 4))", "((1 2) 3 4)")]
        [InlineData("(cons (cons 1 (cons 2 nil)) (cons 3 (cons 4 nil)))", "((1 2) 3 4)")]
        [InlineData("(cons 1 (list 2 3))", "(1 2 3)")]
        [InlineData("(cons 1 (cons 2 (cons 3 nil)))", "(1 2 3)")]
        [InlineData("(cons (list 1 2) 3)", "((1 2) . 3)")]
        [InlineData("(cons (cons 1 (cons 2 nil)) 3)", "((1 2) . 3)")]
        [InlineData("(list (list 1 2) 3)", "((1 2) 3)")]
        [InlineData("(cons (cons 1 (cons 2 nil)) (cons 3 nil))", "((1 2) 3)")]
        [InlineData("(list 1 (list 2 3))", "(1 (2 3))")]
        [InlineData("(cons 1 (cons (cons 2 (cons 3 nil)) nil))", "(1 (2 3))")]
        [InlineData("(list (list 1 2) (list 3 4))", "((1 2) (3 4))")]
        [InlineData("(cons (cons 1 (cons 2 nil)) (cons (cons 3 (cons 4 nil)) nil))", "((1 2) (3 4))")]
        [InlineData("(cons (list 1 2) (list (list 3 4) 5))", "((1 2) (3 4) 5)")]
        [InlineData("(cons (list 1 2) (list 3 (list 4 5)))", "((1 2) 3 (4 5))")]
        public void Evaluate_ExpressionGivingPairs_PrintsCorrectly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.IsType<Pair>(result);
            Assert.Equal(expectedResult, printedResult);
        }

        [Theory]
        [InlineData(10, "(1 2 3 4 5 6 7 8 9 10)")]
        [InlineData(20, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20)")]
        [InlineData(100, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100)")]
        [InlineData(101, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100 ...)")]
        [InlineData(110, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100 ...)")]
        public void Evaluate_ExpressionGivingLargeList_PrintsCorrectly(int count, string expectedResult)
        {
            // Arrange
            string expression = $@"
(define (numbers count)
  (define (numbers-iter i)
    (if (= i count)
        nil
        (cons (+ i 1) (numbers-iter (+ i 1)))))
  (numbers-iter 0))

(numbers {count})";

            // Act
            var result = _interpreter.Evaluate(expression).Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.IsType<Pair>(result);
            Assert.Equal(expectedResult, printedResult);
        }

        [Theory]
        [InlineData(10, "(1 (2 (3 (4 (5 (6 (7 (8 (9 10)))))))))")]
        [InlineData(20, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 20)))))))))))))))))))")]
        [InlineData(30, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 (20 (21 (22 (23 (24 (25 (26 (27 (28 (29 30)))))))))))))))))))))))))))))")]
        [InlineData(60, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 (20 (21 (22 (23 (24 (25 (26 (27 (28 (29 (30 (31 (32 (33 (34 (35 (36 (37 (38 (39 (40 (41 (42 (43 (44 (45 (46 (47 (48 (49 (50 ...))))))))))))))))))))))))))))))))))))))))))))))))))")]
        [InlineData(100, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 (20 (21 (22 (23 (24 (25 (26 (27 (28 (29 (30 (31 (32 (33 (34 (35 (36 (37 (38 (39 (40 (41 (42 (43 (44 (45 (46 (47 (48 (49 (50 ...))))))))))))))))))))))))))))))))))))))))))))))))))")]
        public void Evaluate_ExpressionGivingLargeNestedList_PrintsCorrectly(int count, string expectedResult)
        {
            // Arrange
            string expression = $@"
(define (numbers count)
  (define (numbers-iter i)
    (if (= i count)
        i
        (list i (numbers-iter (+ i 1)))))
  (numbers-iter 1))

(numbers {count})";

            // Act
            var result = _interpreter.Evaluate(expression).Last();
            string printedResult = result.AcceptVisitor(_printVisitor);

            // Assert
            Assert.IsType<Pair>(result);
            Assert.Equal(expectedResult, printedResult);
        }
    }
}
