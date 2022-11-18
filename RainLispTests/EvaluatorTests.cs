﻿using RainLisp;
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
        [InlineData("(make-date)", 3, false, 0)]
        [InlineData("(make-date 1)", 3, false, 1)]
        [InlineData("(make-date 1 2 3 4)", 3, false, 4)]
        [InlineData("(make-datetime)", 7, false, 0)]
        [InlineData("(make-datetime 1)", 7, false, 1)]
        [InlineData("(make-datetime 1 2 3 4 5 6 7 8)", 7, false, 8)]
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
        [InlineData("({0})", 2, 0)]
        [InlineData("({0} 1)", 2, 1)]
        public void Evaluate_CallExpectingTwoOrMoreWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[] { "+", "-", "*", "/", "%", "xor" }, expression, expected, true, actual);
        }

        [Theory]
        [InlineData("({0})", 2, 0)]
        [InlineData("({0} 1)", 2, 1)]
        [InlineData("({0} 1 2 3)", 2, 3)]
        public void Evaluate_CallExpectingTwoWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[]
                {
                    ">", ">=", "<", "<=", "=", "cons", "set-car!", "set-cdr!",
                    "add-years", "add-months", "add-days", "add-hours", "add-minutes", "add-seconds", "add-milliseconds",
                    "days-diff", "hours-diff", "minutes-diff", "seconds-diff", "milliseconds-diff", "parse-datetime", "datetime-to-string"
                }, expression, expected, false, actual);
        }

        [Theory]
        [InlineData("({0})", 1, 0)]
        [InlineData("({0} 1 2)", 1, 2)]
        public void Evaluate_CallExpectingOneWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[]
                {
                    "not", "car", "cdr", "null?", "display", "debug", "trace", "error",
                    "year", "month", "day", "hour", "minute", "second", "millisecond", "utc?", "to-local", "to-utc"
                }, expression, expected, false, actual);
        }

        [Theory]
        [InlineData("({0} 1)", 0, 1)]
        [InlineData("({0} 1 2)", 0, 2)]
        public void Evaluate_CallExpectingZeroWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[] { "newline", "now", "utc-now" }, expression, expected, false, actual);
        }

        [Theory]
        // =
        [InlineData("(= nil nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(= 12 nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("(= (cons 1 2) (cons 3 4))", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(= (cons 1 2) 12)", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("(= (lambda(x) x) (lambda(x) x))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(= 14 (lambda(x) x))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        [InlineData("(= - -)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("(= - 14)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("(= (now) -)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
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
        [InlineData("(make-datetime nil 1 1 2 3 2 3)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("(make-datetime 2022 (cons 1 2) 1 2 3 4 3)", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(make-datetime 2022 1 2 3 4 1 +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("(make-datetime 2022 1 2 3 (lambda () 1) 1 2)", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("(make-datetime 2022 1 2 3 4 (newline) 3)", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("(make-datetime 2022 1 2 3 4 2 true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("(make-datetime 2022 1 2 3 \"hello\" 2 3)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(make-datetime 2022 1 2 3 4 (now) 3)", typeof(NumberDatum), typeof(DateTimeDatum))]
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
        [InlineData("({0} \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 \"hi\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} \"hi\" \"there\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 false)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("({0} true 1)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("({0} true true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("({0} nil 1)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("({0} 1 nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("({0} nil nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(NumberDatum), typeof(Pair))]
        [InlineData("({0} 1 (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("({0} (cons 1 2) (cons 3 4))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("({0} (lambda(x) x) 1)", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("({0} 1 (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("({0} (lambda(x) x) (lambda(x) x))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("({0} - 1)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} 1 -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} - -)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (now) 1)", typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 (now))", typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} (now) (now))", typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} (newline) 1)", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("({0} 1 (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("({0} (newline) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        public void Evaluate_CallExpectingNumbersWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            // maybe we should be able to concat strings with +.
            Evaluate_CallsWithWrongExpression_Throws(new[] { "+", "-", "*", "/", "%", ">", ">=", "<", "<=" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} \"hi\")", typeof(Pair), typeof(StringDatum))]
        [InlineData("({0} 1)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("({0} true)", typeof(Pair), typeof(BoolDatum))]
        [InlineData("({0} (now))", typeof(Pair), typeof(DateTimeDatum))]
        [InlineData("({0} nil)", typeof(Pair), typeof(Nil))]
        [InlineData("({0} (newline))", typeof(Pair), typeof(Unspecified))]
        [InlineData("({0} (lambda(x) x))", typeof(Pair), typeof(UserProcedure))]
        [InlineData("({0} -)", typeof(Pair), typeof(PrimitiveProcedure))]
        public void Evaluate_CallExpectingPairWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            // maybe we should be able to concat strings with +.
            Evaluate_CallsWithWrongExpression_Throws(new[] { "car", "cdr" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} \"hi\" 1)", typeof(Pair), typeof(StringDatum))]
        [InlineData("({0} 1 1)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("({0} true 1)", typeof(Pair), typeof(BoolDatum))]
        [InlineData("({0} (now) 1)", typeof(Pair), typeof(DateTimeDatum))]
        [InlineData("({0} nil 1)", typeof(Pair), typeof(Nil))]
        [InlineData("({0} (newline) 1)", typeof(Pair), typeof(Unspecified))]
        [InlineData("({0} (lambda(x) x) 1)", typeof(Pair), typeof(UserProcedure))]
        [InlineData("({0} - 1)", typeof(Pair), typeof(PrimitiveProcedure))]
        public void Evaluate_CallExpectingPairAndAnythingWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            // maybe we should be able to concat strings with +.
            Evaluate_CallsWithWrongExpression_Throws(new[] { "set-car!", "set-cdr!" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} nil)", typeof(IPrimitiveDatum), typeof(Nil))]
        [InlineData("({0} (newline))", typeof(IPrimitiveDatum), typeof(Unspecified))]
        [InlineData("({0} (cons 1 2))", typeof(IPrimitiveDatum), typeof(Pair))]
        [InlineData("({0} +)", typeof(IPrimitiveDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1))", typeof(IPrimitiveDatum), typeof(UserProcedure))]
        public void Evaluate_CallExpectingPrimitiveWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            // maybe we should be able to concat strings with +.
            Evaluate_CallsWithWrongExpression_Throws(new[] { "display", "debug", "trace", "error" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("({0} (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("({0} +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("({0} (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("({0} true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("({0} \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        public void Evaluate_CallExpectingDateTimeWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "year", "month", "day", "hour", "minute", "second", "millisecond", "utc?", "to-local", "to-utc" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("({0} + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("({0} (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("({0} true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("({0} 1 1)", typeof(DateTimeDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("({0} (now) nil)", typeof(NumberDatum), typeof(Nil))]
        [InlineData("({0} (now) (cons 1 2))", typeof(NumberDatum), typeof(Pair))]
        [InlineData("({0} (now) +)", typeof(NumberDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (now) (lambda () 1))", typeof(NumberDatum), typeof(UserProcedure))]
        [InlineData("({0} (now) (newline))", typeof(NumberDatum), typeof(Unspecified))]
        [InlineData("({0} (now) true)", typeof(NumberDatum), typeof(BoolDatum))]
        [InlineData("({0} (now) \"hello\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} (now) (now))", typeof(NumberDatum), typeof(DateTimeDatum))]
        public void Evaluate_CallExpectingDateTimeAndNumberWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "add-years", "add-months", "add-days", "add-hours", "add-minutes", "add-seconds", "add-milliseconds" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("({0} + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("({0} (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("({0} true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("({0} 1 (now))", typeof(DateTimeDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("({0} (now) nil)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("({0} (now) (cons 1 2))", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("({0} (now) +)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (now) (lambda () 1))", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("({0} (now) (newline))", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("({0} (now) true)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("({0} (now) 1)", typeof(DateTimeDatum), typeof(NumberDatum))]
        [InlineData("({0} (now) \"hello\")", typeof(DateTimeDatum), typeof(StringDatum))]
        public void Evaluate_CallExpecting2DateTimesWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "days-diff", "hours-diff", "minutes-diff", "seconds-diff", "milliseconds-diff" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(StringDatum), typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(StringDatum), typeof(Pair))]
        [InlineData("({0} + 1)", typeof(StringDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1) 1)", typeof(StringDatum), typeof(UserProcedure))]
        [InlineData("({0} (newline) 1)", typeof(StringDatum), typeof(Unspecified))]
        [InlineData("({0} true 1)", typeof(StringDatum), typeof(BoolDatum))]
        [InlineData("({0} 1 \"hello\")", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" nil)", typeof(StringDatum), typeof(Nil))]
        [InlineData("({0} \"hello\" (cons 1 2))", typeof(StringDatum), typeof(Pair))]
        [InlineData("({0} \"hello\" +)", typeof(StringDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} \"hello\" (lambda () 1))", typeof(StringDatum), typeof(UserProcedure))]
        [InlineData("({0} \"hello\" (newline))", typeof(StringDatum), typeof(Unspecified))]
        [InlineData("({0} \"hello\" true)", typeof(StringDatum), typeof(BoolDatum))]
        public void Evaluate_CallExpecting2StringsWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "parse-datetime" }, expression, expected, actual);
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(DateTimeDatum), typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(DateTimeDatum), typeof(Pair))]
        [InlineData("({0} + 1)", typeof(DateTimeDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1) 1)", typeof(DateTimeDatum), typeof(UserProcedure))]
        [InlineData("({0} (newline) 1)", typeof(DateTimeDatum), typeof(Unspecified))]
        [InlineData("({0} true 1)", typeof(DateTimeDatum), typeof(BoolDatum))]
        [InlineData("({0} 1 1)", typeof(DateTimeDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("({0} (now) nil)", typeof(StringDatum), typeof(Nil))]
        [InlineData("({0} (now) (cons 1 2))", typeof(StringDatum), typeof(Pair))]
        [InlineData("({0} (now) +)", typeof(StringDatum), typeof(PrimitiveProcedure))]
        [InlineData("({0} (now) (lambda () 1))", typeof(StringDatum), typeof(UserProcedure))]
        [InlineData("({0} (now) (newline))", typeof(StringDatum), typeof(Unspecified))]
        [InlineData("({0} (now) true)", typeof(StringDatum), typeof(BoolDatum))]
        [InlineData("({0} (now) 1)", typeof(StringDatum), typeof(NumberDatum))]
        public void Evaluate_CallExpectingDateTimeAndStringWithWrongTypeOfArgument_Throws(string expression, Type expected, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "datetime-to-string" }, expression, expected, actual);
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
        [InlineData("((now) 1)")]
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

        [Theory]
        [InlineData("(make-date 0 1 1)")]
        [InlineData("(make-date 10000 1 1)")]
        [InlineData("(make-date 2022 0 1)")]
        [InlineData("(make-date 2022 13 1)")]
        [InlineData("(make-date 2022 1 0)")]
        [InlineData("(make-date 2022 1 32)")]
        [InlineData("(make-datetime 2022 1 1 -1 1 1 1)")]
        [InlineData("(make-datetime 2022 1 1 24 1 1 1)")]
        [InlineData("(make-datetime 2022 1 1 1 -1 1 1)")]
        [InlineData("(make-datetime 2022 1 1 1 60 1 1)")]
        [InlineData("(make-datetime 2022 1 1 1 1 -1 1)")]
        [InlineData("(make-datetime 2022 1 1 1 1 60 1)")]
        [InlineData("(make-datetime 2022 1 1 1 1 1 -1)")]
        [InlineData("(make-datetime 2022 1 1 1 1 1 1000)")]
        [InlineData("(to-local (now))")] // now is not UTC to be made local.
        [InlineData("(to-utc (utc-now))")] // utc-now is not local to be made UTC.
        [InlineData("(add-years (make-date 2022 1 1) 8000)")]
        [InlineData("(add-years (make-date 2022 1 1) -3000)")]
        [InlineData("(add-months (make-date 2022 1 1) 96000)")]
        [InlineData("(add-months (make-date 2022 1 1) -36000)")]
        [InlineData("(add-days (make-date 2022 1 1) 2980000)")]
        [InlineData("(add-days (make-date 2022 1 1) -1080000)")]
        [InlineData("(add-hours (make-date 2022 1 1) 71520000)")]
        [InlineData("(add-hours (make-date 2022 1 1) -25920000)")]
        [InlineData("(add-minutes (make-date 2022 1 1) 4291200000)")]
        [InlineData("(add-minutes (make-date 2022 1 1) -1555200000)")]
        [InlineData("(add-seconds (make-date 2022 1 1) 257472000000)")]
        [InlineData("(add-seconds (make-date 2022 1 1) -93312000000)")]
        [InlineData("(add-milliseconds (make-date 2022 1 1) 257472000000000)")]
        [InlineData("(add-milliseconds (make-date 2022 1 1) -93312000000000)")]
        public void Evaluate_ExpressionWithInvalidValue_Throws(string expression)
        {
            Evaluate_WrongExpression_Throws<InvalidValueException>(expression);
        }

        private void Evaluate_CallsWithWrongNumberOfArguments_Throws(string[] procedureNames, string expression, int expected, bool orMore, int actual)
        {
            // Arrange
            // Act
            foreach (string procName in procedureNames)
            {
                var exception = Evaluate_WrongExpression_Throws<WrongNumberOfArgumentsException>(string.Format(expression, procName));

                // Assert
                Assert.Equal(expected, exception!.Expected);
                Assert.Equal(orMore, exception.OrMore);
                Assert.Equal(actual, exception.Actual);
            }
        }

        private void Evaluate_CallsWithWrongExpression_Throws(string[] procedureNames, string expression, Type expected, Type actual)
        {
            // Arrange
            // Act
            foreach (string procName in procedureNames)
            {
                var exception = Evaluate_WrongExpression_Throws<WrongTypeOfArgumentException>(string.Format(expression, procName));

                // Assert
                Assert.Equal(expected, exception.Expected);
                Assert.Equal(actual, exception.Actual);
            }
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
