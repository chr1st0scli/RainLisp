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
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("\"\"", "")]
        [InlineData("\" \"", " ")]
        [InlineData("\"\\n\\n\"", "\n\n")]
        [InlineData("\"hello world\"", "hello world")]
        public void Evaluate_Literal_Correctly(string expression, object expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((PrimitiveDatum)result).Value);
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

            // Assert
            Assert.Equal(expectedResult, (double)((PrimitiveDatum)result).Value);
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
            Assert.Equal(expectedResult, Math.Round((double)((PrimitiveDatum)result).Value, 2, MidpointRounding.AwayFromZero));
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
        [InlineData("(= 32 23)", false)]
        [InlineData("(= true true)", true)]
        [InlineData("(= false false)", true)]
        [InlineData("(= true false)", false)]
        [InlineData("(= \"abc\" \"abcd\")", false)]
        [InlineData("(= \"abcd\" \"abcd\")", true)]
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
            Assert.Equal(expectedResult, (bool)((PrimitiveDatum)result).Value);
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
            Assert.Equal(expectedResult, (double)((PrimitiveDatum)result).Value);
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
            Assert.Equal(expectedResult, (double)((PrimitiveDatum)result).Value);
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
            Assert.Equal(expectedResult, (double)((PrimitiveDatum)result).Value);
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
            Assert.Equal(16d, (double)((PrimitiveDatum)result).Value);
        }

        [Theory]
        [InlineData(1, 1, false)]
        [InlineData(2, 2, false)]
        [InlineData(4, 4, false)]
        [InlineData(6, 6, false)]
        [InlineData(7, 6, true)] // Execute all 6 calls to shouldProceed?
        public void Evaluate_AndOperands_CorrectNumberOfTimes(int firstCallToReturnFalse, int expectedCallCount, bool expectedResult)
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
            Assert.Equal(expectedResult, ((PrimitiveDatum)result).Value);
            Assert.Equal(expectedCallCount, (double)((PrimitiveDatum)callCountResult).Value);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(2, 2, true)]
        [InlineData(4, 4, true)]
        [InlineData(6, 6, true)]
        [InlineData(7, 6, false)] // Execute all 6 calls to shouldProceed?
        public void Evaluate_OrOperands_CorrectNumberOfTimes(int firstCallToReturnTrue, int expectedCallCount, bool expectedResult)
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
            Assert.Equal(expectedResult, ((PrimitiveDatum)result).Value);
            Assert.Equal(expectedCallCount, (double)((PrimitiveDatum)callCountResult).Value);
        }

        [Theory]
        [InlineData("(and 1)", 1d)]
        [InlineData("(and false)", false)]
        [InlineData("(and true)", true)]
        [InlineData("(and true true)", true)]
        [InlineData("(and false false)", false)]
        [InlineData("(and false 1)", false)]
        [InlineData("(and 1 2 3)", 3d)]
        [InlineData("(and true true 2)", 2d)]
        [InlineData("(and 1 true 2 true 3)", 3d)]
        [InlineData("(and true true 2 true 3)", 3d)]
        [InlineData("(and true true true true 3)", 3d)]
        [InlineData("(and true false true 3)", false)]
        [InlineData("(and false 2 true 3)", false)]
        [InlineData("(or 1)", 1d)]
        [InlineData("(or false)", false)]
        [InlineData("(or true)", true)]
        [InlineData("(or true true)", true)]
        [InlineData("(or false false)", false)]
        [InlineData("(or false 1)", 1d)]
        [InlineData("(or false false 2)", 2d)]
        [InlineData("(or 1 false 2 false 3)", 1d)]
        [InlineData("(or false false 2 false 3)", 2d)]
        [InlineData("(or false false false false 3)", 3d)]
        [InlineData("(or false true false 3)", true)]
        [InlineData("(or true 2 false 3)", true)]
        public void Evaluate_AndOr_Correctly(string expression, object expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((PrimitiveDatum)result).Value);
        }

        [Theory]
        [InlineData("(car (cons 1 2))", 1d)]
        [InlineData("(cdr (cons 1 2))", 2d)]
        [InlineData("(car (cons true false))", true)]
        [InlineData("(cdr (cons true false))", false)]
        [InlineData("(car (cons \"hello\" \"world\"))", "hello")]
        [InlineData("(cdr (cons \"hello\" \"world\"))", "world")]
        [InlineData("(car (cons 1 true))", 1d)]
        [InlineData("(cdr (cons 1 true))", true)]
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
        [InlineData("(null? 1)", false)]
        [InlineData("(null? true)", false)]
        [InlineData("(null? \"hello\")", false)]
        [InlineData("(null? (cons 1 2))", false)]
        [InlineData("(null? (list))", true)] // TODO support test other forms of empty list such as '()
        [InlineData("(null? (list 1))", false)]
        [InlineData("(car (list 1))", 1d)]
        [InlineData("(null? (cdr (list 1)))", true)]
        public void Evaluate_Lists_Correctly(string expression, object expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((PrimitiveDatum)result).Value);
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
        public void Evaluate_LibraryFunctions_Correctly(string expression, object expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression);

            // Assert
            Assert.Equal(expectedResult, ((PrimitiveDatum)result).Value);
        }

        [Theory]
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
        public void Evaluate_WrongNumberOfArguments_Throws(string expression, int expected, bool orMore, int actual)
        {
            // Arrange
            WrongNumberOfArgumentsException? exception = null;

            // Act
            try
            {
                _interpreter.Evaluate(expression);
            }
            catch (WrongNumberOfArgumentsException ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<WrongNumberOfArgumentsException>(exception);
            Assert.Equal(expected, exception!.Expected);
            Assert.Equal(orMore, exception.OrMore);
            Assert.Equal(actual, exception.Actual);
        }
    }
}
