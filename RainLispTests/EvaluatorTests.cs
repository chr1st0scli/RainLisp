using RainLisp;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;

namespace RainLispTests
{
    public class EvaluatorTests
    {
        private readonly Interpreter _interpreter = new();

        [Theory]
        [InlineData("1", 1d)]
        [InlineData("10.54", 10.54)]
        [InlineData("-0.5", -0.5)]
        [InlineData("+0.5", 0.5)]
        public void Evaluate_NumberLiteral_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void Evaluate_BooleanLiteral_Correctly(string expression, bool expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((BoolDatum)result).Value);
        }

        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\" \"", " ")]
        [InlineData("\"\\n\\n\"", "\n\n")]
        [InlineData("\"hello world\"", "hello world")]
        public void Evaluate_StringLiteral_Correctly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((StringDatum)result).Value);
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
        [InlineData("(define ab 10) (cond (true (set! ab (+ ab 1)) (set! ab (+ ab 1)) ab) (false (set! ab (+ ab 2)) (set! ab (+ ab 2)) ab) (else (set! ab (+ ab 3)) (set! ab (+ ab 3)) ab))", 12d)]
        [InlineData("(define ab 10) (cond (false (set! ab (+ ab 1)) (set! ab (+ ab 1)) ab) (true (set! ab (+ ab 2)) (set! ab (+ ab 2)) ab) (else (set! ab (+ ab 3)) (set! ab (+ ab 3)) ab))", 14d)]
        [InlineData("(define ab 10) (cond (false (set! ab (+ ab 1)) (set! ab (+ ab 1)) ab) (false (set! ab (+ ab 2)) (set! ab (+ ab 2)) ab) (else (set! ab (+ ab 3)) (set! ab (+ ab 3)) ab))", 16d)]
        [InlineData("(let ((a 1)) 0)", 0d)]
        [InlineData("(let ((a 1)) a)", 1d)]
        [InlineData("(let ((a 1) (b 2)) (+ a b))", 3d)]
        [InlineData("(let ((a 1)) (define b 2) (define c 3) (+ a b c))", 6d)]
        [InlineData("(let ((a 1) (b 2)) (define c 3) (define d 4) (+ a b c d))", 10d)]
        [InlineData("(define a 2) ((lambda () (begin (set! a 4) a)))", 4d)]
        [InlineData("(define a 2) ((lambda () (set! a 4))) a", 4d)]
        [InlineData("(define a 2) ((lambda () (define b 7) (begin (set! a (+ a b)) (set! b 11)))) a", 9d)]
        [InlineData("(define (foo) (define a 1) (define b 2) (set! a (+ a b)) a) (foo)", 3d)]
        [InlineData("((lambda () (define a 1) (define b 2) (set! a (+ a b)) a))", 3d)]
        [InlineData("(let ((a 1) (b 2)) (set! a (+ a b)) a)", 3d)]
        [InlineData("(define (foo a b) (set! a (+ a b)) (set! b (* a b)) (- b a)) (foo 10 12)", 242d)]
        [InlineData("((lambda (a b) (set! a (+ a b)) (set! b (* a b)) (- b a)) 10 12)", 242d)]
        [InlineData("(let ((a 10) (b 12)) (set! a (+ a b)) (set! b (* a b)) (- b a))", 242d)]
        public void Evaluate_NumericExpression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);
            result = GetLastResult(result);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
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
        public void Evaluate_NumericPrimitiveExpression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, Math.Round(((NumberDatum)result).Value, 2, MidpointRounding.AwayFromZero));
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
        [InlineData("(= 21 21)", true)]
        [InlineData("(= 21.56 21.56)", true)]
        [InlineData("(= 21.56 21.57)", false)]
        [InlineData("(= 32 23)", false)]
        [InlineData("(= true true)", true)]
        [InlineData("(= false false)", true)]
        [InlineData("(= true false)", false)]
        [InlineData("(= \"abc\" \"abcd\")", false)]
        [InlineData("(= \"abcd\" \"abcd\")", true)]
        [InlineData("(= \"abcd\" 1)", false)]
        [InlineData("(= true 1)", false)]
        [InlineData("(not true)", false)]
        [InlineData("(not false)", true)]
        [InlineData("(not (not true))", true)]
        [InlineData("(not \"hi\")", false)]
        [InlineData("(not 1)", false)]
        [InlineData("(not nil)", false)]
        [InlineData("(not (cons 1 2))", false)]
        [InlineData("(not (lambda(x) x))", false)]
        [InlineData("(not -)", false)]
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
        [InlineData("(xor \"hi\" 1)", false)]
        [InlineData("(xor 1 \"hi\")", false)]
        [InlineData("(xor \"hi\" \"there\")", false)]
        [InlineData("(xor 1 false)", true)]
        [InlineData("(xor true 1)", false)]
        [InlineData("(xor nil nil)", false)]
        [InlineData("(xor (cons 1 2) (cons 3 4))", false)]
        [InlineData("(xor (lambda(x) x) (lambda(x) x))", false)]
        [InlineData("(xor - -)", false)]
        [InlineData("(and (or true false) false)", false)]
        [InlineData("(or (or true false) false)", true)]
        [InlineData("(or (and true false) false)", false)]
        [InlineData("(or (and true false) true)", true)]
        [InlineData("(or (and true false) (and false true))", false)]
        [InlineData("(and (or true false) (or false true))", true)]
        [InlineData("(not (and (or true false) (or false true)))", false)]
        [InlineData("(and (= 2 3) (= 4 4))", false)]
        [InlineData("(and (= 24 24) (= 4 5))", false)]
        [InlineData("(and (= 24 24) (= 7 7))", true)]
        [InlineData("(and (or (> 3 1) (>= 1 4)) (< 5 2))", false)]
        [InlineData("(or (or (>= 6 2) (<= 9 4)) (< 7 4))", true)]
        [InlineData("(or (and (> 9 3) (> 9 11)) (< 9 5))", false)]
        [InlineData("(or (and (> 10 8) (< 11 19)) true)", true)]
        [InlineData("(or (and (>= 10 10) (<= 10 9)) (and (> 5 6) (<= 4 4)))", false)]
        [InlineData("(and (or (>= 5 5) (> 5 5)) (or (> 5 6) (<= 5 6)))", true)]
        [InlineData("(not (and (or (> 6 2) (< 6 2)) (or (< 4 2) (> 4 2))))", false)]
        [InlineData("(and (> (+ 5 4) (- 5 4)) (> (/ 4 5) (* 4 5)))", false)]
        [InlineData("(or (> (+ 5 4) (- 5 4)) (> (/ 4 5) (* 4 5)))", true)]
        public void Evaluate_LogicalExpression_Correctly(string expression, bool expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((BoolDatum)result).Value);
        }

        [Theory]
        [InlineData("(car l)", 3d)]
        [InlineData("(cadr l)", 2d)]
        [InlineData("(caddr l)", 1d)]
        public void Evaluate_FunctionArguments_FromLeftToRight(string expression, double expectedResult)
        {
            // Arrange
            string program = $@"
(define (foo a b c) true)

(define l nil)

(foo (set! l (cons 1 l))
     (set! l (cons 2 l))
     (set! l (cons 3 l)))
{expression}";

            // Act
            var result = _interpreter.Evaluate(program);
            result = GetLastResult(result);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("(define a 1)")]
        [InlineData("(define (foo) 1)")]
        [InlineData("(define (foo x) x)")]
        [InlineData("(define a 0) (set! a 2)")]
        [InlineData("(define a 0) (set! a (lambda () true))")]
        [InlineData("(if false 1)")]    // If with no alternative to enter.
        [InlineData("(set-car! (cons 1 2) 0)")]
        [InlineData("(set-cdr! (cons 1 2) 0)")]
        [InlineData("(display 0)")]
        [InlineData("(debug 0)")]
        [InlineData("(trace 0)")]
        [InlineData("(newline)")]
        public void Evaluate_ExpressionWithNothingToReturn_GivesUnspecified(string expression)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.IsType<Unspecified>(result);
            Assert.Equal(Unspecified.GetUnspecified(), result);
        }

        [Theory]
        [InlineData("(list)")]  // TODO, add quote alternatives.
        public void Evaluate_EmptyList_ReturnsNil(string expression)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.IsType<Nil>(result);
            Assert.Equal(Nil.GetNil(), result);
        }

        [Theory]
        [InlineData(@"
(define (factorial n)
    (if (= n 1)
        1
        (* n (factorial (- n 1)))))

(factorial 5)", 120d)]

        [InlineData(@"
(define (factorial n)
    (define (iter i acc)
        (if (> i n)
            acc
            (iter (+ i 1) (* i acc))))
    (iter 1 1))

(factorial 6)", 720d)]
        public void Evaluate_RecursiveExpression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("((foo true))", 1d)]
        [InlineData("((foo false))", 0d)]
        public void Evaluate_Lambda_InTheRightEnvironment(string lambdaCall, double expectedResult)
        {
            // Arrange
            string expression = $@"
(define a 0)

(define (foo b)

    (define (bar)
        (define a 1)
        (lambda () a))

    (if b
        (bar)
        (lambda () a)))

{lambdaCall}";

            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("(foo 4)", 0d)]
        [InlineData("(foo 5)", 1d)]
        [InlineData("(foo 6)", 0d)]
        [InlineData("(foo 7)", 1d)]
        public void Evaluate_ProcedureInternalDefinitions_Correctly(string innerCall, double expectedResult)
        {
            // Arrange
            string expression = $@"
(define (f)
  (define (foo n)
    (if (= n 0)
        0
        (bar (- n 1))))
  
  (define (bar n)
    (if (= n 0)
        1
        (foo (- n 1))))
  {innerCall})

(f)";

            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Fact]
        public void Evaluate_SimpleInternalDefinitions_Correctly()
        {
            // Note that the above internal definitions (procedures) work differently than these simple internal definitions.
            // It's because a procedure definition's value is a lambda and when it's evaluated it returns a procedure, it doesn't get called.
            // So, by the time a call happens, all definitions are established. In contrast, in the example below, the value
            // a is evaluated but it is 1 from the 1st line and not 5 from the line below. So, we see a discrepancy between the two.
            // A common strategy, in both cases, is to produce an error when you have internal definitions that refer to each other (see SICP 4.1.6).

            // Arrange
            string expression = @"
(let ((a 1))
  (define (f x)
    (define b (+ a x))
    (define a 5)
    (+ a b))
  
  (f 10))";

            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(16d, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData(1, 1d, false)]
        [InlineData(2, 2d, false)]
        [InlineData(4, 4d, false)]
        [InlineData(6, 6d, false)]
        [InlineData(7, 6d, true)] // Execute all 6 calls to shouldProceed?
        public void Evaluate_AndOperands_CorrectNumberOfTimes(int firstCallToReturnFalse, double expectedCallCount, bool expectedResult)
        {
            // Arrange
            string expression = $@"
(define callCount 0)

(define (shouldProceed?)
    (set! callCount (+ callCount 1))

    (if (>= callCount {firstCallToReturnFalse})
        false
        true))

; and should stop on the first false and not evaluate the rest of the operands.
(and (shouldProceed?) (shouldProceed?) (shouldProceed?) (shouldProceed?) (shouldProceed?) (shouldProceed?))";

            IEvaluationEnvironment? environment = null;

            // Act
            var result = _interpreter.Evaluate(expression, ref environment);
            var callCountResult = _interpreter.Evaluate("callCount", ref environment);

            // Assert
            Assert.Equal(expectedResult, ((BoolDatum)result).Value);
            Assert.Equal(expectedCallCount, ((NumberDatum)callCountResult).Value);
        }

        [Theory]
        [InlineData(1, 1d, true)]
        [InlineData(2, 2d, true)]
        [InlineData(4, 4d, true)]
        [InlineData(6, 6d, true)]
        [InlineData(7, 6d, false)] // Execute all 6 calls to shouldProceed?
        public void Evaluate_OrOperands_CorrectNumberOfTimes(int firstCallToReturnTrue, double expectedCallCount, bool expectedResult)
        {
            // Arrange
            string expression = $@"
(define callCount 0)

(define (shouldProceed?)
    (set! callCount (+ callCount 1))

    (if (>= callCount {firstCallToReturnTrue})
        true
        false))

; or should stop on the first true and not evaluate the rest of the operands.
(or (shouldProceed?) (shouldProceed?) (shouldProceed?) (shouldProceed?) (shouldProceed?) (shouldProceed?))";

            IEvaluationEnvironment? environment = null;

            // Act
            var result = _interpreter.Evaluate(expression, ref environment);
            var callCountResult = _interpreter.Evaluate("callCount", ref environment);

            // Assert
            Assert.Equal(expectedResult, ((BoolDatum)result).Value);
            Assert.Equal(expectedCallCount, ((NumberDatum)callCountResult).Value);
        }

        [Theory]
        [InlineData("(and 1)", 1d)]
        [InlineData("(and 1 2 3)", 3d)]
        [InlineData("(and true true 2)", 2d)]
        [InlineData("(and 1 true 2 true 3)", 3d)]
        [InlineData("(and true true 2 true 3)", 3d)]
        [InlineData("(and true true true true 3)", 3d)]
        [InlineData("(or 1)", 1d)]
        [InlineData("(or false 1)", 1d)]
        [InlineData("(or false false 2)", 2d)]
        [InlineData("(or 1 false 2 false 3)", 1d)]
        [InlineData("(or false false 2 false 3)", 2d)]
        [InlineData("(or false false false false 3)", 3d)]
        public void Evaluate_AndOrReturningNumber_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("(and false)", false)]
        [InlineData("(and true)", true)]
        [InlineData("(and true true)", true)]
        [InlineData("(and false false)", false)]
        [InlineData("(and false 1)", false)]
        [InlineData("(and true false true 3)", false)]
        [InlineData("(and false 2 true 3)", false)]
        [InlineData("(or false)", false)]
        [InlineData("(or true)", true)]
        [InlineData("(or true true)", true)]
        [InlineData("(or false false)", false)]
        [InlineData("(or false true false 3)", true)]
        [InlineData("(or true 2 false 3)", true)]
        public void Evaluate_AndOrReturningBoolean_Correctly(string expression, bool expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((BoolDatum)result).Value);
        }

        [Theory]
        [InlineData("(car (cons 1 2))", 1d)]
        [InlineData("(cdr (cons 1 2))", 2d)]
        [InlineData("(car (cons 1 true))", 1d)]
        [InlineData("(car (car (cons (cons 1 2) (cons 3 4))))", 1d)]
        [InlineData("(cdr (car (cons (cons 1 2) (cons 3 4))))", 2d)]
        [InlineData("(car (cdr (cons (cons 1 2) (cons 3 4))))", 3d)]
        [InlineData("(cdr (cdr (cons (cons 1 2) (cons 3 4))))", 4d)]
        [InlineData("(cdr (cons (cons 1 (cons 2 3)) 4))", 4d)]
        [InlineData("(car (car (cons (cons 1 (cons 2 3)) 4)))", 1d)]
        [InlineData("(car (cdr (car (cons (cons 1 (cons 2 3)) 4))))", 2d)]
        [InlineData("(cdr (cdr (car (cons (cons 1 (cons 2 3)) 4))))", 3d)]
        [InlineData("(car (cons 1 (cons 2 (cons 3 (cons 4 nil)))))", 1d)]
        [InlineData("(car (cdr (cons 1 (cons 2 (cons 3 (cons 4 nil))))))", 2d)]
        [InlineData("(car (cdr (cdr (cons 1 (cons 2 (cons 3 (cons 4 nil)))))))", 3d)]
        [InlineData("(car (cdr (cdr (cdr (cons 1 (cons 2 (cons 3 (cons 4 nil))))))))", 4d)]
        [InlineData("(car (list 1 2 3 4))", 1d)]
        [InlineData("(car (cdr (list 1 2 3 4)))", 2d)]
        [InlineData("(car (cdr (cdr (list 1 2 3 4))))", 3d)]
        [InlineData("(car (cdr (cdr (cdr (list 1 2 3 4)))))", 4d)]
        [InlineData("(car (list 1))", 1d)]
        [InlineData("(define x (cons 1 2)) (set-car! x 3) (car x)", 3d)]
        [InlineData("(define x (cons 1 2)) (set-car! x 3) (cdr x)", 2d)]
        [InlineData("(define x (cons 1 2)) (set-cdr! x 3) (car x)", 1d)]
        [InlineData("(define x (cons 1 2)) (set-cdr! x 3) (cdr x)", 3d)]
        [InlineData("(define x (list 1 2)) (set-car! x 3) (car x)", 3d)]
        [InlineData("(define x (list 1 2)) (set-car! x 3) (cadr x)", 2d)]
        [InlineData("(define x (list 1 2)) (set-cdr! x 3) (car x)", 1d)]
        [InlineData("(define x (list 1 2)) (set-cdr! x 3) (cdr x)", 3d)]
        [InlineData("(define x (list 1 2)) (set-cdr! x (list 3 4)) (car x)", 1d)]
        [InlineData("(define x (list 1 2)) (set-cdr! x (list 3 4)) (cadr x)", 3d)]
        [InlineData("(define x (list 1 2)) (set-cdr! x (list 3 4)) (caddr x)", 4d)]
        public void Evaluate_ListReturningNumber_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);
            result = GetLastResult(result);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("(car (cons true false))", true)]
        [InlineData("(cdr (cons true false))", false)]
        [InlineData("(cdr (cons 1 true))", true)]
        [InlineData("(null? 1)", false)]
        [InlineData("(null? true)", false)]
        [InlineData("(null? \"hello\")", false)]
        [InlineData("(null? (cons 1 2))", false)]
        [InlineData("(null? (list))", true)] // TODO support test other forms of empty list such as '()
        [InlineData("(null? (list 1))", false)]
        [InlineData("(null? (cdr (list 1)))", true)]
        [InlineData("(null? nil)", true)]
        public void Evaluate_ListReturningBoolean_Correctly(string expression, bool expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((BoolDatum)result).Value);
        }

        [Theory]
        [InlineData("(car (cons \"hello\" \"world\"))", "hello")]
        [InlineData("(cdr (cons \"hello\" \"world\"))", "world")]
        public void Evaluate_ListReturningString_Correctly(string expression, string expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((StringDatum)result).Value);
        }

        [Theory]
        [InlineData(@"
(fold-left + 0
           (filter (lambda (x) (<= x 14)) 
                   (map (lambda (x) (+ x 10)) 
                        (list 1 2 3 4 5 6 7 8))))", 50d)]
        [InlineData(@"
(fold-right + 0
           (filter (lambda (x) (<= x 14)) 
                   (map (lambda (x) (+ x 10)) 
                        (list 1 2 3 4 5 6 7 8))))", 50d)]
        [InlineData(@"
(fold-left + 0
           (filter (lambda (x) (>= x 15)) 
                   (map (lambda (x) (+ x 10)) 
                        (list 1 2 3 4 5 6 7 8))))", 66d)]
        [InlineData(@"
(fold-right + 0
           (filter (lambda (x) (>= x 15)) 
                   (map (lambda (x) (+ x 10)) 
                        (list 1 2 3 4 5 6 7 8))))", 66d)]
        [InlineData("(fold-left + 0 (list 1 2 3 4))", 10d)]
        [InlineData("(fold-right + 0 (list 1 2 3 4))", 10d)]
        [InlineData("(fold-left - 0 (list 1 2 3 4))", -10d)]
        [InlineData("(fold-right - 0 (list 1 2 3 4))", -2d)]
        [InlineData("(reduce + (list 1 2 3 4))", 10d)]
        [InlineData("(reduce - (list 1 2 3 4))", -8d)]
        [InlineData("(reduce + (list 1))", 1d)]
        [InlineData("(reduce - (list 1))", 1d)]
        [InlineData("(cadr (list 1 2 3 4 5))", 2d)]
        [InlineData("(car (cddr (list 1 2 3 4 5)))", 3d)]
        [InlineData("(caddr (list 1 2 3 4 5))", 3d)]
        [InlineData("(car (cdddr (list 1 2 3 4 5)))", 4d)]
        [InlineData("(cadddr (list 1 2 3 4 5))", 4d)]
        [InlineData("(car (append (list 1 2) (list 3 4)))", 1d)]
        [InlineData("(cadr (append (list 1 2) (list 3 4)))", 2d)]
        [InlineData("(caddr (append (list 1 2) (list 3 4)))", 3d)]
        [InlineData("(cadddr (append (list 1 2) (list 3 4)))", 4d)]
        [InlineData("(length (append (list 1 2) nil))", 2d)]
        [InlineData("(length (append nil (list 3 4 5)))", 3d)]
        [InlineData("(length (append nil nil))", 0d)]
        [InlineData("(car (reverse (list 1 2 3 4)))", 4d)]
        [InlineData("(cadr (reverse (list 1 2 3 4)))", 3d)]
        [InlineData("(caddr (reverse (list 1 2 3 4)))", 2d)]
        [InlineData("(cadddr (reverse (list 1 2 3 4)))", 1d)]
        [InlineData("(length (reverse (list 1 2 3 4)))", 4d)]
        [InlineData("(car (reverse (list 1)))", 1d)]
        [InlineData("(length (reverse (list 1)))", 1d)]
        [InlineData("(length (reverse (list)))", 0d)]
        [InlineData("(length (reverse nil))", 0d)]
        [InlineData("(length (list))", 0d)]
        [InlineData("(length nil)", 0d)]
        [InlineData("(length (list 1 2 3))", 3d)]
        [InlineData("(length (cons 1 (cons 2 nil)))", 2d)]
        public void Evaluate_LibraryFunctions_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("(define (foo) 1) (foo 1)", 0, false, 1)]
        [InlineData("(define (foo) 1) (foo 1 2)", 0, false, 2)]
        [InlineData("(define (foo x) x) (foo)", 1, false, 0)]
        [InlineData("(define (foo x) x) (foo 1 2)", 1, false, 2)]
        [InlineData("(define (foo x y) (+ x y)) (foo)", 2, false, 0)]
        [InlineData("(define (foo x y) (+ x y)) (foo 1)", 2, false, 1)]
        [InlineData("(define (foo x y) (+ x y)) (foo 1 2 3)", 2, false, 3)]
        [InlineData("(+)", 2, true, 0)]
        [InlineData("(+ 1)", 2, true, 1)]
        [InlineData("(-)", 2, true, 0)]
        [InlineData("(- 1)", 2, true, 1)]
        [InlineData("(*)", 2, true, 0)]
        [InlineData("(* 1)", 2, true, 1)]
        [InlineData("(/)", 2, true, 0)]
        [InlineData("(/ 1)", 2, true, 1)]
        [InlineData("(%)", 2, true, 0)]
        [InlineData("(% 1)", 2, true, 1)]
        [InlineData("(>)", 2, false, 0)]
        [InlineData("(> 1)", 2, false, 1)]
        [InlineData("(> 1 2 3)", 2, false, 3)]
        [InlineData("(>=)", 2, false, 0)]
        [InlineData("(>= 1)", 2, false, 1)]
        [InlineData("(>= 1 2 3)", 2, false, 3)]
        [InlineData("(<)", 2, false, 0)]
        [InlineData("(< 1)", 2, false, 1)]
        [InlineData("(< 1 2 3)", 2, false, 3)]
        [InlineData("(<=)", 2, false, 0)]
        [InlineData("(<= 1)", 2, false, 1)]
        [InlineData("(<= 1 2 3)", 2, false, 3)]
        [InlineData("(=)", 2, false, 0)]
        [InlineData("(= 1)", 2, false, 1)]
        [InlineData("(= 1 2 3)", 2, false, 3)]
        [InlineData("(xor)", 2, true, 0)]
        [InlineData("(xor true)", 2, true, 1)]
        [InlineData("(not)", 1, false, 0)]
        [InlineData("(not true false)", 1, false, 2)]
        [InlineData("(cons)", 2, false, 0)]
        [InlineData("(cons 1)", 2, false, 1)]
        [InlineData("(cons 1 2 3)", 2, false, 3)]
        [InlineData("(car)", 1, false, 0)]
        [InlineData("(car 1 2)", 1, false, 2)]
        [InlineData("(cdr)", 1, false, 0)]
        [InlineData("(cdr 1 2)", 1, false, 2)]
        [InlineData("(null?)", 1, false, 0)]
        [InlineData("(null? 1 2)", 1, false, 2)]
        [InlineData("(set-car!)", 2, false, 0)]
        [InlineData("(set-car! (cons 1 2))", 2, false, 1)]
        [InlineData("(set-car! (cons 1 2) 3 4)", 2, false, 3)]
        [InlineData("(set-cdr!)", 2, false, 0)]
        [InlineData("(set-cdr! (cons 1 2))", 2, false, 1)]
        [InlineData("(set-cdr! (cons 1 2) 3 4)", 2, false, 3)]
        [InlineData("(display)", 1, false, 0)]
        [InlineData("(display 1 2)", 1, false, 2)]
        [InlineData("(debug)", 1, false, 0)]
        [InlineData("(debug 1 2)", 1, false, 2)]
        [InlineData("(trace)", 1, false, 0)]
        [InlineData("(trace 1 2)", 1, false, 2)]
        [InlineData("(newline 1)", 0, false, 1)]
        [InlineData("(newline 1 2)", 0, false, 2)]
        [InlineData("(error)", 1, false, 0)]
        [InlineData("(error 1 2)", 1, false, 2)]
        [InlineData("(now 1)", 0, false, 1)]
        [InlineData("(now 1 2)", 0, false, 2)]
        [InlineData("(utc-now 1)", 0, false, 1)]
        [InlineData("(utc-now 1 2)", 0, false, 2)]
        [InlineData("(make-date)", 3, false, 0)]
        [InlineData("(make-date 1)", 3, false, 1)]
        [InlineData("(make-date 1 2 3 4)", 3, false, 4)]
        [InlineData("(make-datetime)", 7, false, 0)]
        [InlineData("(make-datetime 1)", 7, false, 1)]
        [InlineData("(make-datetime 1 2 3 4 5 6 7 8)", 7, false, 8)]
        [InlineData("(year)", 1, false, 0)]
        [InlineData("(year 1 2)", 1, false, 2)]
        [InlineData("(month)", 1, false, 0)]
        [InlineData("(month 1 2)", 1, false, 2)]
        [InlineData("(day)", 1, false, 0)]
        [InlineData("(day 1 2)", 1, false, 2)]
        [InlineData("(hour)", 1, false, 0)]
        [InlineData("(hour 1 2)", 1, false, 2)]
        [InlineData("(minute)", 1, false, 0)]
        [InlineData("(minute 1 2)", 1, false, 2)]
        [InlineData("(second)", 1, false, 0)]
        [InlineData("(second 1 2)", 1, false, 2)]
        [InlineData("(millisecond)", 1, false, 0)]
        [InlineData("(millisecond 1 2)", 1, false, 2)]
        [InlineData("(utc?)", 1, false, 0)]
        [InlineData("(utc? 1 2)", 1, false, 2)]
        [InlineData("(to-local)", 1, false, 0)]
        [InlineData("(to-local 1 2)", 1, false, 2)]
        [InlineData("(to-utc)", 1, false, 0)]
        [InlineData("(to-utc 1 2)", 1, false, 2)]
        [InlineData("(add-years)", 2, false, 0)]
        [InlineData("(add-years 1)", 2, false, 1)]
        [InlineData("(add-years 1 2 3)", 2, false, 3)]
        [InlineData("(add-months)", 2, false, 0)]
        [InlineData("(add-months 1)", 2, false, 1)]
        [InlineData("(add-months 1 2 3)", 2, false, 3)]
        [InlineData("(add-days)", 2, false, 0)]
        [InlineData("(add-days 1)", 2, false, 1)]
        [InlineData("(add-days 1 2 3)", 2, false, 3)]
        [InlineData("(add-hours)", 2, false, 0)]
        [InlineData("(add-hours 1)", 2, false, 1)]
        [InlineData("(add-hours 1 2 3)", 2, false, 3)]
        [InlineData("(add-minutes)", 2, false, 0)]
        [InlineData("(add-minutes 1)", 2, false, 1)]
        [InlineData("(add-minutes 1 2 3)", 2, false, 3)]
        [InlineData("(add-seconds)", 2, false, 0)]
        [InlineData("(add-seconds 1)", 2, false, 1)]
        [InlineData("(add-seconds 1 2 3)", 2, false, 3)]
        [InlineData("(add-milliseconds)", 2, false, 0)]
        [InlineData("(add-milliseconds 1)", 2, false, 1)]
        [InlineData("(add-milliseconds 1 2 3)", 2, false, 3)]
        [InlineData("(days-diff)", 2, false, 0)]
        [InlineData("(days-diff 1)", 2, false, 1)]
        [InlineData("(days-diff 1 2 3)", 2, false, 3)]
        [InlineData("(hours-diff)", 2, false, 0)]
        [InlineData("(hours-diff 1)", 2, false, 1)]
        [InlineData("(hours-diff 1 2 3)", 2, false, 3)]
        [InlineData("(minutes-diff)", 2, false, 0)]
        [InlineData("(minutes-diff 1)", 2, false, 1)]
        [InlineData("(minutes-diff 1 2 3)", 2, false, 3)]
        [InlineData("(seconds-diff)", 2, false, 0)]
        [InlineData("(seconds-diff 1)", 2, false, 1)]
        [InlineData("(seconds-diff 1 2 3)", 2, false, 3)]
        [InlineData("(milliseconds-diff)", 2, false, 0)]
        [InlineData("(milliseconds-diff 1)", 2, false, 1)]
        [InlineData("(milliseconds-diff 1 2 3)", 2, false, 3)]
        [InlineData("(parse-datetime)", 2, false, 0)]
        [InlineData("(parse-datetime 1)", 2, false, 1)]
        [InlineData("(parse-datetime 1 2 3)", 2, false, 3)]
        [InlineData("(datetime-to-string)", 2, false, 0)]
        [InlineData("(datetime-to-string 1)", 2, false, 1)]
        [InlineData("(datetime-to-string 1 2 3)", 2, false, 3)]
        public void Evaluate_WrongNumberOfArguments_Throws(string expression, int expected, bool orMore, int actual)
        {
            // Arrange
            // Act
            var exception = Evaluate_WrongExpression_Throws<WrongNumberOfArgumentsException>(expression);

            // Assert
            Assert.Equal(expected, exception!.Expected);
            Assert.Equal(orMore, exception.OrMore);
            Assert.Equal(actual, exception.Actual);
        }

        [Theory]
        // +
        [InlineData("(+ \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(+ 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(+ \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))] // maybe we should be able to concat strings like that.
        [InlineData("(+ 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(+ true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(+ nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(+ (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(+ (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(+ - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // -
        [InlineData("(- \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(- 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(- \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(- 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(- true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(- nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(- (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(- (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(- - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // *
        [InlineData("(* \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(* 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(* \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(* 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(* true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(* nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(* (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(* (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(* - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // /
        [InlineData("(/ \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(/ 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(/ \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(/ 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(/ true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(/ nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(/ (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(/ (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(/ - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // %
        [InlineData("(% \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(% 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(% \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(% 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(% true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(% nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(% (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(% (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(% - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // >
        [InlineData("(> \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(> 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(> \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(> 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(> true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(> nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(> (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(> (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(> - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // >=
        [InlineData("(>= \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(>= 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(>= \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(>= 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(>= true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(>= nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(>= (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(>= (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(>= - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // <
        [InlineData("(< \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(< 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(< \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(< 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(< true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(< nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(< (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(< (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(< - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // <=
        [InlineData("(<= \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(<= 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(<= \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(<= 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(<= true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(<= nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(<= (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(<= (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(<= - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        // =
        [InlineData("(= nil nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(= 12 nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(= (cons 1 2) (cons 3 4))", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(= (cons 1 2) 12)", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(= (lambda(x) x) (lambda(x) x))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(= 14 (lambda(x) x))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(= - -)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("(= - 14)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        // car
        [InlineData("(car \"hi\")", typeof(Pair), typeof(StringDatum))]
        [InlineData("(car 1)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("(car true)", typeof(Pair), typeof(BoolDatum))]
        [InlineData("(car nil)", typeof(Pair), typeof(Nil))]
        [InlineData("(car (lambda(x) x))", typeof(Pair), typeof(UserProcedure))]
        [InlineData("(car -)", typeof(Pair), typeof(PrimitiveProcedure))]
        // cdr
        [InlineData("(cdr \"hi\")", typeof(Pair), typeof(StringDatum))]
        [InlineData("(cdr 1)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("(cdr true)", typeof(Pair), typeof(BoolDatum))]
        [InlineData("(cdr nil)", typeof(Pair), typeof(Nil))]
        [InlineData("(cdr (lambda(x) x))", typeof(Pair), typeof(UserProcedure))]
        [InlineData("(cdr -)", typeof(Pair), typeof(PrimitiveProcedure))]
        // set-car!
        [InlineData("(set-car! \"hi\" 1)", typeof(Pair), typeof(StringDatum))]
        [InlineData("(set-car! 1 1)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("(set-car! true 1)", typeof(Pair), typeof(BoolDatum))]
        [InlineData("(set-car! nil 1)", typeof(Pair), typeof(Nil))]
        [InlineData("(set-car! (lambda(x) x) 1)", typeof(Pair), typeof(UserProcedure))]
        [InlineData("(set-car! - 1)", typeof(Pair), typeof(PrimitiveProcedure))]
        // set-cdr!
        [InlineData("(set-cdr! \"hi\" 1)", typeof(Pair), typeof(StringDatum))]
        [InlineData("(set-cdr! 1 1)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("(set-cdr! true 1)", typeof(Pair), typeof(BoolDatum))]
        [InlineData("(set-cdr! nil 1)", typeof(Pair), typeof(Nil))]
        [InlineData("(set-cdr! (lambda(x) x) 1)", typeof(Pair), typeof(UserProcedure))]
        [InlineData("(set-cdr! - 1)", typeof(Pair), typeof(PrimitiveProcedure))]
        // display
        [InlineData("(display nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(display (cons 1 2))", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(display +)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("(display (lambda () 1))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(display (newline))", typeof(IPrimitiveDatum), typeof(Unspecified))]
        // debug
        [InlineData("(debug nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(debug (cons 1 2))", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(debug +)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("(debug (lambda () 1))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(debug (newline))", typeof(IPrimitiveDatum), typeof(Unspecified))]
        // trace
        [InlineData("(trace nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(trace (cons 1 2))", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(trace +)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("(trace (lambda () 1))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(trace (newline))", typeof(IPrimitiveDatum), typeof(Unspecified))]
        // error
        [InlineData("(error nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(error (cons 1 2))", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(error +)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("(error (lambda () 1))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(error (newline))", typeof(IPrimitiveDatum), typeof(Unspecified))]
        // make-date
        [InlineData("(make-date nil 2 3)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(make-date 2022 (cons 1 2) 3)", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(make-date 2022 1 +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(make-date (lambda () 1) 1 2)", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(make-date 2022 (newline) 3)", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(make-date 2022 2 true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(make-date \"hello\" 2 3)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(make-date 2022 (now) 3)", typeof(NumberDatum), typeof(DateTimeDatum))]
        // make-datetime
        [InlineData("(make-datetime 2022 1 2 3 nil 2 3)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(make-datetime 2022 1 2 3 4 (cons 1 2) 3)", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(make-datetime 2022 1 2 3 4 1 +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(make-datetime 2022 1 2 3 (lambda () 1) 1 2)", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(make-datetime 2022 1 2 3 4 (newline) 3)", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(make-datetime 2022 1 2 3 4 2 true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(make-datetime 2022 1 2 3 \"hello\" 2 3)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(make-datetime 2022 1 2 3 4 (now) 3)", typeof(NumberDatum), typeof(DateTimeDatum))]
        // year
        [InlineData("(year nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(year (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(year +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(year (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(year (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(year true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(year \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // month
        [InlineData("(month nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(month (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(month +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(month (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(month (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(month true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(month \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // day
        [InlineData("(day nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(day (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(day +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(day (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(day (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(day true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(day \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // hour
        [InlineData("(hour nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(hour (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(hour +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(hour (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(hour (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(hour true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(hour \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // minute
        [InlineData("(minute nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(minute (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(minute +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(minute (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(minute (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(minute true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(minute \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // second
        [InlineData("(second nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(second (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(second +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(second (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(second (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(second true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(second \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // millisecond
        [InlineData("(millisecond nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(millisecond (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(millisecond +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(millisecond (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(millisecond (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(millisecond true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(millisecond \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // utc?
        [InlineData("(utc? nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(utc? (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(utc? +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(utc? (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(utc? (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(utc? true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(utc? \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // to-local
        [InlineData("(to-local nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(to-local (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(to-local +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(to-local (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(to-local (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(to-local true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(to-local \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // to-utc
        [InlineData("(to-utc nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(to-utc (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(to-utc +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(to-utc (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(to-utc (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(to-utc true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(to-utc \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // add-years
        [InlineData("(add-years nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(add-years (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(add-years + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-years (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(add-years (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(add-years true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(add-years \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(add-years (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(add-years (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(add-years (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-years (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(add-years (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(add-years (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(add-years (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        // add-months
        [InlineData("(add-months nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(add-months (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(add-months + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-months (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(add-months (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(add-months true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(add-months \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(add-months (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(add-months (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(add-months (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-months (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(add-months (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(add-months (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(add-months (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        // add-days
        [InlineData("(add-days nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(add-days (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(add-days + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-days (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(add-days (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(add-days true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(add-days \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(add-days (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(add-days (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(add-days (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-days (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(add-days (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(add-days (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(add-days (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        // add-hours
        [InlineData("(add-hours nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(add-hours (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(add-hours + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-hours (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(add-hours (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(add-hours true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(add-hours \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(add-hours (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(add-hours (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(add-hours (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-hours (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(add-hours (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(add-hours (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(add-hours (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        // add-minutes
        [InlineData("(add-minutes nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(add-minutes (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(add-minutes + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-minutes (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(add-minutes (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(add-minutes true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(add-minutes \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(add-minutes (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(add-minutes (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(add-minutes (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-minutes (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(add-minutes (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(add-minutes (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(add-minutes (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        // add-seconds
        [InlineData("(add-seconds nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(add-seconds (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(add-seconds + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-seconds (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(add-seconds (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(add-seconds true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(add-seconds \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(add-seconds (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(add-seconds (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(add-seconds (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-seconds (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(add-seconds (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(add-seconds (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(add-seconds (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        // add-milliseconds
        [InlineData("(add-milliseconds nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(add-milliseconds (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(add-milliseconds + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-milliseconds (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(add-milliseconds (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(add-milliseconds true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(add-milliseconds \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(add-milliseconds (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(add-milliseconds (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(add-milliseconds (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(add-milliseconds (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(add-milliseconds (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(add-milliseconds (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(add-milliseconds (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        // days-diff
        [InlineData("(days-diff nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(days-diff (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(days-diff + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(days-diff (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(days-diff (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(days-diff true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(days-diff \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(days-diff (now) nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(days-diff (now) (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(days-diff (now) +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(days-diff (now) (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(days-diff (now) (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(days-diff (now) true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(days-diff (now) \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // hours-diff
        [InlineData("(hours-diff nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(hours-diff (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(hours-diff + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(hours-diff (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(hours-diff (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(hours-diff true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(hours-diff \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(hours-diff (now) nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(hours-diff (now) (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(hours-diff (now) +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(hours-diff (now) (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(hours-diff (now) (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(hours-diff (now) true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(hours-diff (now) \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // minutes-diff
        [InlineData("(minutes-diff nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(minutes-diff (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(minutes-diff + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(minutes-diff (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(minutes-diff (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(minutes-diff true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(minutes-diff \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(minutes-diff (now) nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(minutes-diff (now) (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(minutes-diff (now) +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(minutes-diff (now) (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(minutes-diff (now) (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(minutes-diff (now) true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(minutes-diff (now) \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // seconds-diff
        [InlineData("(seconds-diff nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(seconds-diff (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(seconds-diff + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(seconds-diff (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(seconds-diff (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(seconds-diff true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(seconds-diff \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(seconds-diff (now) nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(seconds-diff (now) (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(seconds-diff (now) +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(seconds-diff (now) (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(seconds-diff (now) (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(seconds-diff (now) true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(seconds-diff (now) \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // milliseconds-diff
        [InlineData("(milliseconds-diff nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(milliseconds-diff (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(milliseconds-diff + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(milliseconds-diff (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(milliseconds-diff (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(milliseconds-diff true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(milliseconds-diff \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(milliseconds-diff (now) nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(milliseconds-diff (now) (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(milliseconds-diff (now) +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(milliseconds-diff (now) (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(milliseconds-diff (now) (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(milliseconds-diff (now) true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(milliseconds-diff (now) \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        // parse-datetime
        [InlineData("(parse-datetime nil 1)", typeof(StringDatum), typeof(Nil))]
        [InlineData("(parse-datetime (cons 1 2) 1)", typeof(StringDatum), typeof(Pair))]
        [InlineData("(parse-datetime + 1)", typeof(StringDatum), typeof(PrimitiveProcedure))]
        [InlineData("(parse-datetime (lambda () 1) 1)", typeof(StringDatum), typeof(UserProcedure))]
        [InlineData("(parse-datetime (newline) 1)", typeof(StringDatum), typeof(Unspecified))]
        [InlineData("(parse-datetime true 1)", typeof(StringDatum), typeof(BoolDatum))]
        [InlineData("(parse-datetime \"hello\" nil)", typeof(StringDatum), typeof(Nil))]
        [InlineData("(parse-datetime \"hello\" (cons 1 2))", typeof(StringDatum), typeof(Pair))]
        [InlineData("(parse-datetime \"hello\" +)", typeof(StringDatum), typeof(PrimitiveProcedure))]
        [InlineData("(parse-datetime \"hello\" (lambda () 1))", typeof(StringDatum), typeof(UserProcedure))]
        [InlineData("(parse-datetime \"hello\" (newline))", typeof(StringDatum), typeof(Unspecified))]
        [InlineData("(parse-datetime \"hello\" true)", typeof(StringDatum), typeof(BoolDatum))]
        // datetime-to-string
        [InlineData("(datetime-to-string nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("(datetime-to-string (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("(datetime-to-string + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("(datetime-to-string (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("(datetime-to-string (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("(datetime-to-string true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("(datetime-to-string \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(datetime-to-string (now) nil)", typeof(StringDatum), typeof(Nil))]
        [InlineData("(datetime-to-string (now) (cons 1 2))", typeof(StringDatum), typeof(Pair))]
        [InlineData("(datetime-to-string (now) +)", typeof(StringDatum), typeof(PrimitiveProcedure))]
        [InlineData("(datetime-to-string (now) (lambda () 1))", typeof(StringDatum), typeof(UserProcedure))]
        [InlineData("(datetime-to-string (now) (newline))", typeof(StringDatum), typeof(Unspecified))]
        [InlineData("(datetime-to-string (now) true)", typeof(StringDatum), typeof(BoolDatum))]
        public void Evaluate_WrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            // Arrange
            // Act
            var exception = Evaluate_WrongExpression_Throws<WrongTypeOfArgumentException>(expression);

            // Assert
            Assert.Equal(expected, exception.Expected);
            Assert.Equal(actual, exception.Actual);
        }

        [Theory]
        [InlineData("foo", "foo")]
        [InlineData("(foo)", "foo")]
        [InlineData("(set! ab 32)", "ab")]
        [InlineData("(define a 1) (set! a 2) (set! b 3)", "b")]
        [InlineData("(+ 1 2 ab 3)", "ab")]
        [InlineData("(define (foo x) x) (foo 5) (foo boo)", "boo")]
        [InlineData("(define (foo) (boo)) (foo)", "boo")]
        public void Evaluate_UnknownIdentifier_Throws(string expression, string expectedIdentifierName)
        {
            // Arrange
            // Act
            var exception = Evaluate_WrongExpression_Throws<UnknownIdentifierException>(expression);

            // Assert
            Assert.Equal(expectedIdentifierName, exception.IdentifierName);
        }

        [Theory]
        [InlineData("(error 1)", "1")]
        [InlineData("(error 12.31)", "12.31")]
        [InlineData("(error true)", "true")]
        [InlineData("(error false)", "false")]
        [InlineData("(error \"This is a user error.\")", "This is a user error.")]
        public void Evaluate_ErrorExpression_Throws(string expression, string expectedMessage)
        {
            // Arrange
            // Act
            var exception = Evaluate_WrongExpression_Throws<UserException>(expression);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Theory]
        [InlineData("(1)")]
        [InlineData("(1 1)")]
        [InlineData("(true)")]
        [InlineData("(true 12.12)")]
        [InlineData("(\"hello\")")]
        [InlineData("(\"hello\" \"world\")")]
        [InlineData("(nil)")]
        [InlineData("(nil 1)")]
        [InlineData("((cons 1 2))")]
        [InlineData("((cons 1 2) 1)")]
        [InlineData("((newline))")]
        [InlineData("((newline) 1)")]
        public void Evaluate_ExpressionCallingNonProcedure_Throws(string expression)
        {
            Evaluate_WrongExpression_Throws<NotProcedureException>(expression);
        }

        private TException Evaluate_WrongExpression_Throws<TException>(string expression) where TException : Exception
        {
            // Arrange
            TException? exception = null;

            // Act
            try
            {
                _interpreter.Evaluate(expression);
            }
            catch (TException ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<TException>(exception);

            return exception!;
        }

        private static EvaluationResult GetLastResult(EvaluationResult result)
        {
            if (result is ProgramResult programResult && programResult.Results?.Count > 1)
                return programResult.Results.Last();
            return result;
        }
    }
}
