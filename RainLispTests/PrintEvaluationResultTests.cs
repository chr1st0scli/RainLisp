using RainLisp;

namespace RainLispTests
{
    public class PrintEvaluationResultTests
    {
        private readonly Interpreter _interpreter = new();

        [Theory]
        [InlineData("nil", "()")]
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
        public void Print_Pairs_Correctly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, result.ToString());
        }

        [Theory]
        [InlineData(10, "(1 2 3 4 5 6 7 8 9 10)")]
        [InlineData(20, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20)")]
        [InlineData(100, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100)")]
        [InlineData(101, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100 ...)")]
        [InlineData(110, "(1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100 ...)")]
        public void Print_LargeList_Correctly(int count, string expectedResult)
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
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, result.ToString());
        }

        [Theory]
        [InlineData(10, "(1 (2 (3 (4 (5 (6 (7 (8 (9 10)))))))))")]
        [InlineData(20, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 20)))))))))))))))))))")]
        [InlineData(30, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 (20 (21 (22 (23 (24 (25 (26 (27 (28 (29 30)))))))))))))))))))))))))))))")]
        [InlineData(60, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 (20 (21 (22 (23 (24 (25 (26 (27 (28 (29 (30 (31 (32 (33 (34 (35 (36 (37 (38 (39 (40 (41 (42 (43 (44 (45 (46 (47 (48 (49 (50 ...))))))))))))))))))))))))))))))))))))))))))))))))))")]
        [InlineData(100, "(1 (2 (3 (4 (5 (6 (7 (8 (9 (10 (11 (12 (13 (14 (15 (16 (17 (18 (19 (20 (21 (22 (23 (24 (25 (26 (27 (28 (29 (30 (31 (32 (33 (34 (35 (36 (37 (38 (39 (40 (41 (42 (43 (44 (45 (46 (47 (48 (49 (50 ...))))))))))))))))))))))))))))))))))))))))))))))))))")]
        public void Print_LargeNestedList_Correctly(int count, string expectedResult)
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
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, result.ToString());
        }
    }
}
