using RainLisp;
using RainLisp.AbstractSyntaxTree;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using System.Linq.Expressions;
using System.Text;

namespace RainLispTests
{
    public class EvaluatorTests
    {
        private readonly Interpreter _interpreter = new();

        [Theory]
        [InlineData("1", 1d)]
        [InlineData("10.54", 10.54)]
        [InlineData("-10.54", -10.54)]
        [InlineData("-0.5", -0.5)]
        [InlineData("+0.5", 0.5)]
        [InlineData("12.1234567890123", 12.1234567890123)]
        [InlineData("+12.1234567890123", 12.1234567890123)]
        [InlineData("-12.1234567890123", -12.1234567890123)]
        public void Evaluate_NumberLiteral_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

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
        [InlineData("(parse-number \"2\")", 2d)]
        [InlineData("(parse-number \"21\")", 21d)]
        [InlineData("(parse-number \"2.5\")", 2.5d)]
        [InlineData("(parse-number \"21.53\")", 21.53d)]
        [InlineData("(parse-number \"21,53\")", 2153d)]
        [InlineData("(parse-number-culture \"2\" \"\")", 2d)]
        [InlineData("(parse-number-culture \"2\" \"en\")", 2d)]
        [InlineData("(parse-number-culture \"21\" \"en\")", 21d)]
        [InlineData("(parse-number-culture \"2.5\" \"en\")", 2.5d)]
        [InlineData("(parse-number-culture \"2.5\" \"\")", 2.5d)]
        [InlineData("(parse-number-culture \"2,5\" \"\")", 25d)]
        [InlineData("(parse-number-culture \"21.53\" \"en\")", 21.53d)]
        [InlineData("(parse-number-culture \"21.53\" \"el\")", 2153d)]
        [InlineData("(parse-number-culture \"21,53\" \"en\")", 2153d)]
        [InlineData("(parse-number-culture \"21,53\" \"el\")", 21.53d)]
        [InlineData("(round 2.5 0)", 3d)]
        [InlineData("(round 2.4 0)", 2d)]
        [InlineData("(round 21.423 2)", 21.42d)]
        [InlineData("(round 21.425 2)", 21.43d)]
        [InlineData("(ceiling 2)", 2d)]
        [InlineData("(ceiling 2.5)", 3d)]
        [InlineData("(ceiling 2.4)", 3d)]
        [InlineData("(ceiling 21.423)", 22d)]
        [InlineData("(ceiling 21.825)", 22d)]
        [InlineData("(floor 2)", 2d)]
        [InlineData("(floor 2.5)", 2d)]
        [InlineData("(floor 2.4)", 2d)]
        [InlineData("(floor 21.423)", 21d)]
        [InlineData("(floor 21.825)", 21d)]
        public void Evaluate_NumericExpression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

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
        [InlineData("(+ 1234.5678 3456.7891)", 4691.3569, false)]
        public void Evaluate_NumericPrimitiveExpression_Correctly(string expression, double expectedResult, bool roundToTwoDecimalPoints = true)
        {
            // Arrange
            // Act
            var result = (NumberDatum)_interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, roundToTwoDecimalPoints ? Math.Round(result.Value, 2, MidpointRounding.AwayFromZero) : result.Value);
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
        [InlineData("(< (now) (add-days (now) 1))", true)]
        [InlineData("(<= (now) (add-days (now) 1))", true)]
        [InlineData("(define dt (now)) (<= dt dt)", true)]
        [InlineData("(> (now) (add-days (now) 1))", false)]
        [InlineData("(>= (now) (add-days (now) 1))", false)]
        [InlineData("(define dt (now)) (>= dt dt)", true)]
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
        [InlineData("(= 12 \"abcd\")", false)]
        [InlineData("(= true 1)", false)]
        [InlineData("(define dt (now)) (= dt dt)", true)]
        [InlineData("(= (make-date 2022 11 19) (make-date 2022 11 19))", true)]
        [InlineData("(= (make-date 2022 11 19) (make-date 2022 11 20))", false)]
        [InlineData("(= 12 true)", false)]
        [InlineData("(= false 12)", false)]
        [InlineData("(= 12 (now))", false)]
        [InlineData("(= (now) 23)", false)]
        [InlineData("(= nil 12)", false)]
        [InlineData("(= 12 (cons 1 2))", false)]
        [InlineData("(= 12 +)", false)]
        [InlineData("(= + 12)", false)]
        [InlineData("(= 12 (quote a))", false)]
        [InlineData("(= 12 'a)", false)]
        [InlineData("(= (quote a) 12)", false)]
        [InlineData("(= 'a 12)", false)]
        [InlineData("(= 12 (lambda() 0))", false)]
        [InlineData("(= (lambda() 0) 12)", false)]
        [InlineData("(= false (now))", false)]
        [InlineData("(= (now) true)", false)]
        [InlineData("(= nil true)", false)]
        [InlineData("(= false (cons 1 2))", false)]
        [InlineData("(= true +)", false)]
        [InlineData("(= + true)", false)]
        [InlineData("(= false (quote a))", false)]
        [InlineData("(= false 'a)", false)]
        [InlineData("(= (quote a) false)", false)]
        [InlineData("(= 'a false)", false)]
        [InlineData("(= true (lambda() 0))", false)]
        [InlineData("(= (lambda() 0) true)", false)]
        // Quotes are unique, therefore their references are equal.
        [InlineData("(= (quote a) (quote a))", true)]
        [InlineData("(= 'a 'a)", true)]
        [InlineData("(= (quote a) 'a)", true)]
        [InlineData("(= 'a (quote a))", true)]
        [InlineData("(= (quote ab) (quote ab))", true)]
        [InlineData("(= 'ab 'ab)", true)]
        [InlineData("(= (quote 12.34) (quote 12.34))", true)]
        [InlineData("(= '12.34 '12.34)", true)]
        [InlineData("(= (quote true) (quote true))", true)]
        [InlineData("(= 'true 'true)", true)]
        [InlineData("(= (quote false) (quote false))", true)]
        [InlineData("(= 'false 'false)", true)]
        [InlineData("(= (quote \"hello\") (quote \"hello\"))", true)]
        [InlineData("(= '\"hello\" '\"hello\")", true)]
        [InlineData("(= (quote a) (quote b))", false)]
        [InlineData("(= 'a 'b)", false)]
        [InlineData("(= 'true 'false)", false)]
        [InlineData("(= '12.34 '12.3)", false)]
        [InlineData("(= (quote \"hello\") (quote \"world\"))", false)]
        [InlineData("(= '\"hello\" '\"world\")", false)]
        // Nil is also unique.
        [InlineData("(= nil nil)", true)]
        [InlineData("(= 12 nil)", false)]
        // Primitive, user procedures and pairs are reference types and are compared as such.
        [InlineData("(= + +)", true)]
        [InlineData("(= - -)", true)]
        [InlineData("(= - +)", false)]
        [InlineData("(= - 14)", false)]
        [InlineData("(= (now) -)", false)]
        [InlineData("(= car car)", true)]
        [InlineData("(= car cdr)", false)]
        [InlineData("(= (lambda () 1) (lambda () 1))", false)]
        [InlineData("(= (lambda(x) x) (lambda(x) x))", false)]
        [InlineData("(= 14 (lambda(x) x))", false)]
        [InlineData("(define (foo) 1) (define f1 foo) (define f2 foo) (= f1 f2)", true)]
        [InlineData("(define (foo) 1) (define (bar) 2) (define f1 foo) (define f2 bar) (= f1 f2)", false)]
        [InlineData("(= (cons 1 2) (cons 3 4))", false)]
        [InlineData("(= (cons 1 2) 12)", false)]
        [InlineData("(define p1 (cons 1 2)) (define p2 (cons 1 2)) (= p1 p2)", false)]
        [InlineData("(define p1 (cons 1 2)) (define p2 (cons 1 2)) (= (car p1) (car p2))", true)]
        [InlineData("(define p1 (cons 1 2)) (define p2 (cons 1 2)) (= (cdr p1) (cdr p2))", true)]
        [InlineData("(define l1 (list 1 2)) (define l2 l1) (= l1 l2)", true)]
        [InlineData("(define l1 (list 1 2)) (define l2 (list 1 2)) (= l1 l2)", false)]
        [InlineData("(define l1 (list 1 2)) (define l2 (list 1 2)) (= (car l1) (car l2))", true)]
        [InlineData("(define l1 (list 1 2)) (define l2 (list 1 2)) (= (cadr l1) (cadr l2))", true)]
        // Logical operators.
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
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((BoolDatum)result).Value);
        }

        [Theory]
        [InlineData("(+ \"\" \"\")", "")]
        [InlineData("(+ \"\" \" \")", " ")]
        [InlineData("(+ \"hello \" \"world\")", "hello world")]
        [InlineData("(+ \"hello\" \" \" \"world\")", "hello world")]
        [InlineData("(+ \"hello\" \" wonderful \" \"world\")", "hello wonderful world")]
        [InlineData("(+ \"hello\" \" magnificent\" \" wonderful\" \" world\")", "hello magnificent wonderful world")]
        [InlineData("(string-length \"\")", 0d)]
        [InlineData("(string-length \"a\")", 1d)]
        [InlineData("(string-length \"abcd\")", 4d)]
        [InlineData("(substring \"hello\" 0 0)", "")]
        [InlineData("(substring \"hello\" 0 1)", "h")]
        [InlineData("(substring \"hello\" 0 2)", "he")]
        [InlineData("(substring \"hello\" 1 3)", "ell")]
        [InlineData("(index-of-string \"hello\" \"ll\" 0)", 2d)]
        [InlineData("(index-of-string \"hello\" \"ll\" 2)", 2d)]
        [InlineData("(index-of-string \"hello\" \"ll\" 3)", -1d)]
        [InlineData("(replace-string \"hello\" \"ll\" \"LL\")", "heLLo")]
        [InlineData("(replace-string \"hello\" \"abc\" \"world\")", "hello")]
        [InlineData("(replace-string \"hello there\" \"there\" \"world\")", "hello world")]
        [InlineData("(to-lower \"HELLO WORLD\")", "hello world")]
        [InlineData("(to-lower \"hello world\")", "hello world")]
        [InlineData("(to-upper \"HELLO WORLD\")", "HELLO WORLD")]
        [InlineData("(to-upper \"hello world\")", "HELLO WORLD")]
        [InlineData("(number-to-string 1 \"f\")", "1.00")]
        [InlineData("(number-to-string 12 \"000.000\")", "012.000")]
        [InlineData("(number-to-string 21.355 \"000.00\")", "021.36")]
        [InlineData("(number-to-string 21.354 \"000.00\")", "021.35")]
        public void Evaluate_StringOperation_Correctly(string expression, object expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((IPrimitiveDatum)result).GetValueAsObject());
        }

        [Theory]
        [InlineData("(utc? (now))", false)]
        [InlineData("(utc? (utc-now))", true)]
        [InlineData("(utc? (to-utc (now)))", true)]
        [InlineData("(utc? (to-local (utc-now)))", false)]
        [InlineData("(year (make-date 2022 12 31))", 2022d)]
        [InlineData("(month (make-date 2022 12 31))", 12d)]
        [InlineData("(day (make-date 2022 12 31))", 31d)]
        [InlineData("(year (make-datetime 2022 12 31 22 34 12 321))", 2022d)]
        [InlineData("(month (make-datetime 2022 12 31 22 34 12 321))", 12d)]
        [InlineData("(day (make-datetime 2022 12 31 22 34 12 321))", 31d)]
        [InlineData("(hour (make-datetime 2022 12 31 22 34 12 321))", 22d)]
        [InlineData("(minute (make-datetime 2022 12 31 22 34 12 321))", 34d)]
        [InlineData("(second (make-datetime 2022 12 31 22 34 12 321))", 12d)]
        [InlineData("(millisecond (make-datetime 2022 12 31 22 34 12 321))", 321d)]
        [InlineData("(year (add-years (make-datetime 2022 12 31 22 34 12 321) 1))", 2023d)]
        [InlineData("(month (add-months (make-datetime 2022 12 31 22 34 12 321) 1))", 1d)]
        [InlineData("(day (add-days (make-datetime 2022 12 31 22 34 12 321) -1))", 30d)]
        [InlineData("(hour (add-hours (make-datetime 2022 12 31 22 34 12 321) 1))", 23d)]
        [InlineData("(minute (add-minutes (make-datetime 2022 12 31 22 34 12 321) 6))", 40d)]
        [InlineData("(second (add-seconds (make-datetime 2022 12 31 22 34 12 321) 8))", 20d)]
        [InlineData("(millisecond (add-milliseconds (make-datetime 2022 12 31 22 34 12 321) 29))", 350d)]
        [InlineData("(days-diff (make-date 2022 1 1) (make-date 2023 12 31))", 729d)]
        [InlineData("(hours-diff (make-datetime 2022 1 1 9 25 30 100) (make-datetime 2023 12 31 23 55 45 300))", 14d)]
        [InlineData("(minutes-diff (make-datetime 2022 1 1 9 25 30 100) (make-datetime 2023 12 31 23 55 45 300))", 30d)]
        [InlineData("(seconds-diff (make-datetime 2022 1 1 9 25 30 100) (make-datetime 2023 12 31 23 55 45 300))", 15d)]
        [InlineData("(milliseconds-diff (make-datetime 2022 1 1 9 25 30 100) (make-datetime 2023 12 31 23 55 45 300))", 200d)]
        [InlineData("(= (parse-datetime \"2022/12/31 21:35:44.321\" \"yyyy/MM/dd HH:mm:ss.fff\") (make-datetime 2022 12 31 21 35 44 321))", true)]
        [InlineData("(datetime-to-string (make-datetime 2022 12 31 21 35 44 321) \"yyyy/MM/dd HH:mm:ss.fff\")", "2022/12/31 21:35:44.321")]
        public void Evaluate_DateTimeExpression_Correctly(string expression, object expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((IPrimitiveDatum)result).GetValueAsObject());
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
            var result = _interpreter.Evaluate(program).Last();

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("")]
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
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.IsType<Unspecified>(result);
            Assert.Equal(Unspecified.GetUnspecified(), result);
        }

        [Theory]
        [InlineData("(list)")]
        [InlineData("(quote ())")]
        [InlineData("'()")]
        public void Evaluate_EmptyList_ReturnsNil(string expression)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.IsType<Nil>(result);
            Assert.Equal(Nil.GetNil(), result);
        }

        [Theory]
        // Linear recursive processes, growing the stack.
        [InlineData(@"
(define (factorial num)
  (if (= num 1)
      1
      (* num (factorial (- num 1)))))

(factorial 5)", 120d)]

        [InlineData(@"
(define (factorial num)
  (if (= num 1)
      1
      (* num (factorial (- num 1)))))

(factorial 6)", 720d)]

        // Linear iterative processes, maintaining the stack size to one.
        [InlineData(@"
(define (factorial number)
  (define (iter num acc)
    (if (= num 1)
        acc
        (iter (- num 1) (* num acc))))

  (iter number 1))

(factorial 5)", 120d)]

        [InlineData(@"
(define (factorial number)
  (define (iter num acc)
    (if (= num 1)
        acc
        (iter (- num 1) (* num acc))))

  (iter number 1))

(factorial 6)", 720d)]

        // Linear iterative processes as deferred procedures.
        [InlineData(@"
(define (factorial number)
  (define (iter num proc)
    (if (= num 1)
        proc
        (iter (- num 1) (lambda () (* num (proc))))))

  (iter number (lambda () 1)))

(define factorial5 (factorial 5))
(factorial5)", 120d)]

        [InlineData(@"
(define (factorial number)
  (define (iter num proc)
    (if (= num 1)
        proc
        (iter (- num 1) (lambda () (* num (proc))))))

  (iter number (lambda () 1)))

(define factorial6 (factorial 6))
(factorial6)", 720d)]

        // Linear iterative processes in continuation passing style.
        [InlineData(@"
(define (=c n1 n2 proc)
  (proc (= n1 n2)))

(define (-c n1 n2 proc)
  (proc (- n1 n2)))

(define (*c n1 n2 proc)
  (proc (* n1 n2)))

(define (factorial num)
  (define (iter-cps n proc)
    (=c n 1
        (lambda (exit)
          (if exit
              (proc 1)
              (-c n 1
                  (lambda (prev-num)
                    (iter-cps prev-num
                              (lambda (prev-num-fact)
                                (*c n prev-num-fact proc)))))))))
  (iter-cps num (lambda (x) x)))

(factorial 5)", 120d)]

        [InlineData(@"
(define (=c n1 n2 proc)
  (proc (= n1 n2)))

(define (-c n1 n2 proc)
  (proc (- n1 n2)))

(define (*c n1 n2 proc)
  (proc (* n1 n2)))

(define (factorial num)
  (define (iter-cps n proc)
    (=c n 1
        (lambda (exit)
          (if exit
              (proc 1)
              (-c n 1
                  (lambda (prev-num)
                    (iter-cps prev-num
                              (lambda (prev-num-fact)
                                (*c n prev-num-fact proc)))))))))
  (iter-cps num (lambda (x) x)))

(factorial 6)", 720d)]
        public void Evaluate_RecursiveExpression_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("(fibonacci 1)", 1d)]
        [InlineData("(fibonacci 2)", 1d)]
        [InlineData("(fibonacci 3)", 2d)]
        [InlineData("(fibonacci 4)", 3d)]
        [InlineData("(fibonacci 5)", 5d)]
        [InlineData("(fibonacci 6)", 8d)]
        [InlineData("(fibonacci 7)", 13)]
        [InlineData("(fibonacci 8)", 21d)]
        [InlineData("(fibonacci 9)", 34d)]
        [InlineData("(fibonacci 10)", 55d)]
        [InlineData("(fibonacci 11)", 89d)]
        [InlineData("(fibonacci 12)", 144d)]
        public void Evaluate_FibonacciAsTreeRecursiveProcess_Correctly(string call, double expectedResult)
        {
            // Arrange
            string expression = $@"
(define (fibonacci n)
  (cond ((= n 0) 0)
        ((= n 1) 1)
        (else (+ (fibonacci (- n 1))
                 (fibonacci (- n 2))))))
{call}";

            // Act
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        [InlineData("(fibonacci 1)", 1d)]
        [InlineData("(fibonacci 2)", 1d)]
        [InlineData("(fibonacci 3)", 2d)]
        [InlineData("(fibonacci 4)", 3d)]
        [InlineData("(fibonacci 5)", 5d)]
        [InlineData("(fibonacci 6)", 8d)]
        [InlineData("(fibonacci 7)", 13)]
        [InlineData("(fibonacci 8)", 21d)]
        [InlineData("(fibonacci 9)", 34d)]
        [InlineData("(fibonacci 10)", 55d)]
        [InlineData("(fibonacci 11)", 89d)]
        [InlineData("(fibonacci 12)", 144d)]
        public void Evaluate_FibonacciAsIterativeProcess_Correctly(string call, double expectedResult)
        {
            // Arrange
            string expression = $@"
(define (fibonacci n)
  (define (iter n1 n2 index)
    (if (= index n)
        n1
        (iter n2 (+ n1 n2) (+ index 1))))

  (iter 0 1 0))

{call}";

            // Act
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Fact]
        public void Evaluate_MessagePassingStyle_Correctly()
        {
            // Arrange
            string code = @"
(define (op-squared op x y)

  (define (square n) 
    (* n n))

  (define proc (cond ((= op '+) +)
                     ((= op '-) -)
                     ((= op '*) *)
                     ((= op '/) /)
                     (else (error ""Unsupported operation.""))))

  (square (proc x y)))

(op-squared '+ 8 4)
(op-squared '- 8 4)
(op-squared '* 8 4)
(op-squared '/ 8 4)";

            // Act
            var results = _interpreter.Evaluate(code).ToArray();

            // Assert
            Assert.Equal(144, ((NumberDatum)results[1]).Value);
            Assert.Equal(16, ((NumberDatum)results[2]).Value);
            Assert.Equal(1024, ((NumberDatum)results[3]).Value);
            Assert.Equal(4, ((NumberDatum)results[4]).Value);
        }

        [Fact]
        public void Evaluate_Metaprogramming_Correctly()
        {
            // Arrange
            string code = @"
; Procedure that builds code that counts.
(define (build-counting-code count)
  (define (iter quote-list cnt)
    (if (= cnt count)
        (append quote-list '(a)) ; In the end, return a.
        (iter (append quote-list '((set! a (+ a 1)))) (+ cnt 1)))) ; Append successive assignments to a, incremented by one.

  ; Start with a lambda that defines variable a that is set to 0.
  (iter '(lambda () (define a 0)) 0))

(define code (build-counting-code 4))

; A list of quote symbols.
code

; Evaluating code, gives a lambda as defined above.
(define count-proc (eval code))

(count-proc)";
            string expectedCodeBuilt = "(lambda () (define a 0) (set! a (+ a 1)) (set! a (+ a 1)) (set! a (+ a 1)) (set! a (+ a 1)) a)";

            // Act
            var results = _interpreter.Evaluate(code).ToArray();
            string actualCodeBuilt = results[2].AcceptVisitor(new EvaluationResultPrintVisitor());

            // Assert
            Assert.Equal(expectedCodeBuilt, actualCodeBuilt);
            Assert.Equal(4, ((NumberDatum)results[4]).Value);
        }

        [Fact]
        public void Evaluate_EncapsulatedProgram_Correctly()
        {
            // Arrange
            string code = @"
(define (make-account amount)

  ; Data.
  (define balance amount)

  ; Local procedures that operate on data.
  (define (deposit amount)
    (set! balance (+ balance amount)))

  (define (withdraw amount)
    (if (< balance amount)
        (error ""Insufficient funds."")
        (set! balance (- balance amount))))

  (define (get-balance)
    balance)

  ; A lambda is returned that captures and gives access to the internal procedures.
  (lambda (op)
    (cond ((= op 'deposit) deposit)
          ((= op 'withdraw) withdraw)
          ((= op 'balance) get-balance)
          (else (error ""Unknown operation."")))))

; Optional interface procedures that allow us to operate on different accounts and hide the message passing details.
(define (get-balance account)
  ((account 'balance)))

(define (withdraw account amount)
  ((account 'withdraw) amount))

(define (deposit account amount)
  ((account 'deposit) amount))

; Ready to try it! Create two accounts and make transactions.
(define lisa-account (make-account 300))
(define bob-account (make-account 320))

(withdraw lisa-account 100)
(withdraw bob-account 200)
(deposit lisa-account 42.50)

(get-balance lisa-account)
(get-balance bob-account)";

            // Act
            var results = _interpreter.Evaluate(code)
                .Where(res => res is NumberDatum)
                .Cast<NumberDatum>()
                .Select(num => num.Value)
                .ToArray();

            // Assert
            Assert.Equal(242.5, results[0]);
            Assert.Equal(120, results[1]);
        }

        [Fact]
        public void Evaluate_DataDirectedProgram_DispatchesOnType()
        {
            // Arrange
            string code = @"
(define (make-table)
  (list '*table*))

; 1D table.
(define (lookup key table)
  (let ((record (assoc key (cdr table))))
    (if record
        (cdr record)
        false)))

(define (assoc key records)
  (cond ((null? records) false)
        ((= key (caar records)) (car records))
        (else (assoc key (cdr records)))))

(define (insert! key value table)
  (let ((record (assoc key (cdr table))))
    (if record
        (set-cdr! record value)
        (set-cdr! table 
                  (cons (cons key value) (cdr table))))))

; 2D table.
(define (lookup-2d key1 key2 table)
  (let ((subtable (assoc key1 (cdr table))))
    (if subtable
        (lookup key2 subtable)
        false)))

(define (insert-2d! key1 key2 value table)
  (let ((subtable (assoc key1 (cdr table))))
    (if subtable
        (insert! key2 value subtable)

        (set-cdr! table
                  (cons (list key1 (cons key2 value))
                        (cdr table))))))

(define dispatch-table (make-table))

(define (put key1 key2 value)
  (insert-2d! key1 key2 value dispatch-table))

(define (get key1 key2)
  (lookup-2d key1 key2 dispatch-table))

; Tagging helpers.
(define (set-tag tag object)
  (cons tag object))

(define (get-tag object)
  (car object))

(define (contents object)
  (cdr object))

; User packages.
; Cat type.
(define (install-cat-package)
  (define type 'cat)
  ; constructor and getters.
  (define (make name age) (cons name age))
  (define (get-name cat) (car cat))
  (define (get-age cat) (cdr cat))
  ; cat specific operations.
  (define (sound) ""meow"")
  (define (likes) ""fish"")
  (define (kind) ""cat"")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Return a user procedure that creates a cat and attaches the appropriate tag to it.
  (lambda (name age) (set-tag type (make name age))))

; Dog type.
(define (install-dog-package)
  (define type 'dog)
  ; constructor and getters.
  (define (make name age) (cons name age))
  (define (get-name dog) (car dog))
  (define (get-age dog) (cdr dog))
  ; dog specific operations.
  (define (sound) ""woof"")
  (define (likes) ""meat"")
  (define (kind) ""dog"")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Return a user procedure that creates a dog and attaches the appropriate tag to it.
  (lambda (name age) (set-tag type (make name age))))

; Package interface helpers.
(define (pet-operation pet operation)
  ; Retrieve the right procedure based on the pet's type.
  (let ((proc (get operation (get-tag pet))))
    (if proc
        proc
        (error ""Unknown operation.""))))

(define (sound pet)
  ((pet-operation pet 'sound)))

(define (likes pet)
  ((pet-operation pet 'likes)))

(define (kind pet)
  ((pet-operation pet 'kind)))

(define (get-name pet)
  ((pet-operation pet 'get-name) (contents pet)))

(define (get-age pet)
  ((pet-operation pet 'get-age) (contents pet)))

; Code that does not need to change no matter how many user packages are installed.
(define (present pet)
  (display (+ ""My pet's name is "" (get-name pet) "". ""))
  (display (+ ""It's a "" (kind pet) "", ""))
  (display (+ (number-to-string (get-age pet) """") "" years of age, ""))
  (display (+ ""it says \"""" (sound pet) ""\"" ""))
  (display (+ ""and likes "" (likes pet) "".\n\n"")))

(define make-cat (install-cat-package))
(define make-dog (install-dog-package))

; Present my two pets to the world.
(define my-cat (make-cat ""Ruby"" 8))
(define my-dog (make-dog ""August"" 16))

(present my-cat)
(present my-dog)";

            string expectedStdOut = @"
My pet's name is Ruby. It's a cat, 8 years of age, it says ""meow"" and likes fish.
My pet's name is August. It's a dog, 16 years of age, it says ""woof"" and likes meat.";

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            Console.SetOut(stringWriter);

            // Act
            _ = _interpreter.Evaluate(code).ToArray();

            // Assert
            Assert.Equal(expectedStdOut.Replace("\r", "").Replace("\n", ""), sb.ToString().Replace("\r", "").Replace("\n", ""));
        }

        [Fact]
        public void Print_ProgramResults_InTheRightOrder()
        {
            // Arrange
            string expression = @"
(define (foo)
	1
	2
	(display ""Should be 1st"") ; display writes the string value to the standard output.
	(newline)
	""Should be 2nd"")  ; whereas a string result is printed in its string representation.
	
(foo)
""Should be 3rd""
""Should be 4th""
(display ""Should be 5th"")";

            string expectedOutput = @"
Should be 1st
""Should be 2nd""
""Should be 3rd""
""Should be 4th""
Should be 5th";

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            Console.SetOut(stringWriter);

            // Act
            IEvaluationEnvironment? environment = null;
            _interpreter.EvaluateAndPrint(expression, ref environment, value => stringWriter.WriteLine(value), (value, ex, unknownError) => stringWriter.WriteLine(ex.Message));

            // Assert
            Assert.Equal(expectedOutput.Replace("\r", "").Replace("\n", ""), sb.ToString().Replace("\r", "").Replace("\n", ""));
        }

        [Fact]
        public void Evaluate_ProgramDefinitionsAndExpressions_InTheRightOrder()
        {
            // Arrange
            string program = @"
(define a 1)
(set! a (+ a 1))
(define b a)
a
b";
            // Act
            var results = _interpreter.Evaluate(program).ToArray();

            // Assert
            Assert.Equal(2, ((NumberDatum)results[3]).Value);
            Assert.Equal(2, ((NumberDatum)results[4]).Value);
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
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression, ref environment).Last();
            var callCountResult = _interpreter.Evaluate("callCount", ref environment).Last();

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
            var result = _interpreter.Evaluate(expression, ref environment).Last();
            var callCountResult = _interpreter.Evaluate("callCount", ref environment).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

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
        [InlineData("(null? (list))", true)]
        [InlineData("(null? '())", true)]
        [InlineData("(null? (list 1))", false)]
        [InlineData("(null? (cdr (list 1)))", true)]
        [InlineData("(null? nil)", true)]
        [InlineData("(pair? 1)", false)]
        [InlineData("(pair? true)", false)]
        [InlineData("(pair? \"hello\")", false)]
        [InlineData("(pair? (cons 1 2))", true)]
        [InlineData("(pair? (cons 1 nil))", true)]
        [InlineData("(pair? (cons nil 1))", true)]
        [InlineData("(pair? (list))", false)]
        [InlineData("(pair? '())", false)]
        [InlineData("(pair? (list 1))", true)]
        [InlineData("(pair? (cdr (list 1)))", false)]
        [InlineData("(pair? nil)", false)]
        public void Evaluate_ListReturningBoolean_Correctly(string expression, bool expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

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
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((StringDatum)result).Value);
        }

        [Theory]
        [InlineData("(quote 1)", "1")]
        [InlineData("'1", "1")]
        [InlineData("(quote 1234.5678)", "1234.5678")]
        [InlineData("'1234.5678", "1234.5678")]
        [InlineData("(quote +1234.5678)", "+1234.5678")]
        [InlineData("'+1234.5678", "+1234.5678")]
        [InlineData("(quote -1234.5678)", "-1234.5678")]
        [InlineData("'-1234.5678", "-1234.5678")]
        [InlineData("(quote true)", "true")]
        [InlineData("'true", "true")]
        [InlineData("(quote false)", "false")]
        [InlineData("'false", "false")]
        [InlineData("(quote \"hello world\")", "\"hello world\"")]
        [InlineData("'\"hello world\"", "\"hello world\"")]
        [InlineData("(quote \"hello \\n \\r \\t \\\" \\\\ world\")", "\"hello \\n \\r \\t \\\" \\\\ world\"")]
        [InlineData("'\"hello \\n \\r \\t \\\" \\\\ world\"", "\"hello \\n \\r \\t \\\" \\\\ world\"")]
        [InlineData("(quote a)", "a")]
        [InlineData("'a", "a")]
        [InlineData("' a", "a")]
        [InlineData("(quote abc)", "abc")]
        [InlineData("'abc", "abc")]
        [InlineData("(quote abc!@#$)", "abc!@#$")]
        [InlineData("'abc!@#$", "abc!@#$")]
        [InlineData("(quote ())", "()")]
        [InlineData("'()", "()")]
        [InlineData("(quote (a bc def))", "(a bc def)")]
        [InlineData("'(a bc def)", "(a bc def)")]
        [InlineData("'  (a bc def)", "(a bc def)")]
        [InlineData("(quote (foo ab 12.34 true \"hi\"))", "(foo ab 12.34 true \"hi\")")]
        [InlineData("'(foo ab 12.34 true \"hi\")", "(foo ab 12.34 true \"hi\")")]
        [InlineData("(car (quote (foo ab 12.34 true \"hi\")))", "foo")]
        [InlineData("(car '(foo ab 12.34 true \"hi\"))", "foo")]
        [InlineData("(cadr (quote (foo ab 12.34 true \"hi\")))", "ab")]
        [InlineData("(cadr '(foo ab 12.34 true \"hi\"))", "ab")]
        [InlineData("(caddr (quote (foo ab 12.34 true \"hi\")))", "12.34")]
        [InlineData("(caddr '(foo ab 12.34 true \"hi\"))", "12.34")]
        [InlineData("(cadddr (quote (foo ab 12.34 true \"hi\")))", "true")]
        [InlineData("(cadddr '(foo ab 12.34 true \"hi\"))", "true")]
        [InlineData("(cadr (cdddr (quote (foo ab 12.34 true \"hi\"))))", "\"hi\"")]
        [InlineData("(cadr (cdddr '(foo ab 12.34 true \"hi\")))", "\"hi\"")]
        [InlineData("(car (quote (a bc def)))", "a")]
        [InlineData("(car '(a bc def))", "a")]
        [InlineData("(cadr (quote (a bc def)))", "bc")]
        [InlineData("(cadr '(a bc def))", "bc")]
        [InlineData("(caddr (quote (a bc def)))", "def")]
        [InlineData("(caddr '(a bc def))", "def")]
        [InlineData("(quote (ab (c (c1 c2) (d)) ef (gh ik) lm no))", "(ab (c (c1 c2) (d)) ef (gh ik) lm no)")]
        [InlineData("'(ab (c (c1 c2) (d)) ef (gh ik) lm no)", "(ab (c (c1 c2) (d)) ef (gh ik) lm no)")]
        [InlineData("(car (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no)))", "ab")]
        [InlineData("(car '(ab (c (c1 c2) (d)) ef (gh ik) lm no))", "ab")]
        [InlineData("(cadr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no)))", "(c (c1 c2) (d))")]
        [InlineData("(cadr '(ab (c (c1 c2) (d)) ef (gh ik) lm no))", "(c (c1 c2) (d))")]
        [InlineData("(car (cadr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no))))", "c")]
        [InlineData("(car (cadr '(ab (c (c1 c2) (d)) ef (gh ik) lm no)))", "c")]
        [InlineData("(car (cadr (cadr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no)))))", "c1")]
        [InlineData("(car (cadr (cadr '(ab (c (c1 c2) (d)) ef (gh ik) lm no))))", "c1")]
        [InlineData("(cadr (cadr (cadr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no)))))", "c2")]
        [InlineData("(cadr (cadr (cadr '(ab (c (c1 c2) (d)) ef (gh ik) lm no))))", "c2")]
        [InlineData("(car (caddr (cadr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no)))))", "d")]
        [InlineData("(car (caddr (cadr '(ab (c (c1 c2) (d)) ef (gh ik) lm no))))", "d")]
        [InlineData("(caddr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no)))", "ef")]
        [InlineData("(caddr '(ab (c (c1 c2) (d)) ef (gh ik) lm no))", "ef")]
        [InlineData("(cadddr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no)))", "(gh ik)")]
        [InlineData("(cadddr '(ab (c (c1 c2) (d)) ef (gh ik) lm no))", "(gh ik)")]
        [InlineData("(cadr (cdddr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no))))", "lm")]
        [InlineData("(cadr (cdddr '(ab (c (c1 c2) (d)) ef (gh ik) lm no)))", "lm")]
        [InlineData("(caddr (cdddr (quote (ab (c (c1 c2) (d)) ef (gh ik) lm no))))", "no")]
        [InlineData("(caddr (cdddr '(ab (c (c1 c2) (d)) ef (gh ik) lm no)))", "no")]
        [InlineData("(quote quote)", "quote")]
        [InlineData("'quote", "quote")]
        [InlineData("(quote set!)", "set!")]
        [InlineData("'set!", "set!")]
        [InlineData("(quote define)", "define")]
        [InlineData("'define", "define")]
        [InlineData("(quote if)", "if")]
        [InlineData("'if", "if")]
        [InlineData("(quote cond)", "cond")]
        [InlineData("'cond", "cond")]
        [InlineData("(quote else)", "else")]
        [InlineData("'else", "else")]
        [InlineData("(quote begin)", "begin")]
        [InlineData("'begin", "begin")]
        [InlineData("(quote lambda)", "lambda")]
        [InlineData("'lambda", "lambda")]
        [InlineData("(quote let)", "let")]
        [InlineData("'let", "let")]
        [InlineData("(quote and)", "and")]
        [InlineData("'and", "and")]
        [InlineData("(quote or)", "or")]
        [InlineData("'or", "or")]
        [InlineData("(quote (if true 1 0))", "(if true 1 0)")]
        [InlineData("'(if true 1 0)", "(if true 1 0)")]
        [InlineData("(quote (cond ((< a 0) -1) ((> a 5) 6) (else 1)))", "(cond ((< a 0) -1) ((> a 5) 6) (else 1))")]
        [InlineData("'(cond ((< a 0) -1) ((> a 5) 6) (else 1))", "(cond ((< a 0) -1) ((> a 5) 6) (else 1))")]
        [InlineData("(quote (quote abc))", "(quote abc)")]
        [InlineData("'(quote abc)", "(quote abc)")]
        [InlineData("''abc", "(quote abc)")]
        [InlineData("(let ((a 1) (b 2)) (+ a b) 'define)", "define")]
        [InlineData("((lambda () 'define))", "define")]
        [InlineData("((lambda () (define a 1) 'define))", "define")]
        [InlineData("(define (foo) 'define) (foo)", "define")]
        [InlineData("(define (foo) (define a 1) 'define) (foo)", "define")]
        [InlineData("(let ((a 1) (b 2)) 'define)", "define")]
        [InlineData("(let ((a 1) (b 2)) (define c 3) 'define)", "define")]
        public void Evaluate_Quote_Correctly(string expression, string expectedResult)
        {
            // Arrange
            IEvaluationEnvironment? environment = null;
            string? actualResult = null;
            string? error = null;

            // Act
            _interpreter.EvaluateAndPrint(expression, ref environment, str => actualResult = str, (str, ex, unknownError) => error = str);

            // Assert
            Assert.Null(error);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("'12.35", 12.35)]
        [InlineData("'true", true)]
        [InlineData("'(+ 1 2 3 4)", 10d)]
        [InlineData("(list 'car (list 'list '1 'true '\"hi\"))", 1d)]
        [InlineData("'(car (list 1 true \"hi\"))", 1d)]
        [InlineData("(list 'cadr (list 'list '1 'true '\"hi\"))", true)]
        [InlineData("'(cadr (list 1 true \"hi\"))", true)]
        [InlineData("(list 'caddr (list 'list '1 'true '\"hi\"))", "hi")]
        [InlineData("'(caddr (list 1 true \"hi\"))", "hi")]
        [InlineData("'(car (car (cadddr (list 1 2 (list 3 4) (list (list 5 6) 7)))))", 5d)]
        [InlineData("'(cadr (car (cadddr (list 1 2 (list 3 4) (list (list 5 6) 7)))))", 6d)]
        [InlineData("'(let ((a 2) (b 3)) (define c 4) (set! c (+ c 1)) (+ a b c))", 10d)]
        public void Evaluate_EvalQuoteSymbols_Correctly(string quotedExpression, object expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate($"(eval {quotedExpression})").Last();

            // Assert
            Assert.Equal(expectedResult, ((IPrimitiveDatum)result).GetValueAsObject());
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
        [InlineData("(length (flatmap (lambda(x) (list x (+ x 10))) (list 1 2)))", 4d)]
        [InlineData("(car (flatmap (lambda(x) (list x (+ x 10))) (list 1 2)))", 1d)]
        [InlineData("(cadr (flatmap (lambda(x) (list x (+ x 10))) (list 1 2)))", 11d)]
        [InlineData("(caddr (flatmap (lambda(x) (list x (+ x 10))) (list 1 2)))", 2d)]
        [InlineData("(cadddr (flatmap (lambda(x) (list x (+ x 10))) (list 1 2)))", 12d)]
        public void Evaluate_LibraryFunctions_Correctly(string expression, double expectedResult)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Theory]
        // 2 levels.
        [InlineData("(caar sequence)", 2, 1)]
        [InlineData("(cdar sequence)", 2, 2)]
        [InlineData("(cadr sequence)", 2, 3)]
        [InlineData("(cddr sequence)", 2, 4)]
        // 3 levels.
        [InlineData("(caaar sequence)", 3, 1)]
        [InlineData("(cdaar sequence)", 3, 2)]
        [InlineData("(cadar sequence)", 3, 3)]
        [InlineData("(cddar sequence)", 3, 4)]
        [InlineData("(caadr sequence)", 3, 5)]
        [InlineData("(cdadr sequence)", 3, 6)]
        [InlineData("(caddr sequence)", 3, 7)]
        [InlineData("(cdddr sequence)", 3, 8)]
        // 4 levels.
        [InlineData("(caaaar sequence)", 4, 1)]
        [InlineData("(cdaaar sequence)", 4, 2)]
        [InlineData("(cadaar sequence)", 4, 3)]
        [InlineData("(cddaar sequence)", 4, 4)]
        [InlineData("(caadar sequence)", 4, 5)]
        [InlineData("(cdadar sequence)", 4, 6)]
        [InlineData("(caddar sequence)", 4, 7)]
        [InlineData("(cdddar sequence)", 4, 8)]
        [InlineData("(caaadr sequence)", 4, 9)]
        [InlineData("(cdaadr sequence)", 4, 10)]
        [InlineData("(cadadr sequence)", 4, 11)]
        [InlineData("(cddadr sequence)", 4, 12)]
        [InlineData("(caaddr sequence)", 4, 13)]
        [InlineData("(cdaddr sequence)", 4, 14)]
        [InlineData("(cadddr sequence)", 4, 15)]
        [InlineData("(cddddr sequence)", 4, 16)]
        public void Evaluate_CarCdrFlavors_Correctly(string expression, int levels, double expectedResult)
        {
            // Arrange
            string code = levels switch
            {
                2 => "(define sequence (cons (cons 1 2) (cons 3 4)))",
                3 => "(define sequence (cons (cons (cons 1 2) (cons 3 4)) (cons (cons 5 6) (cons 7 8))))",
                4 => "(define sequence (cons (cons (cons (cons 1 2) (cons 3 4)) (cons (cons 5 6) (cons 7 8))) (cons (cons (cons 9 10) (cons 11 12)) (cons (cons 13 14) (cons 15 16)))))",
                _ => throw new ArgumentOutOfRangeException(nameof(levels)),
            };
            code += expression;

            // Act
            var result = _interpreter.Evaluate(code).Last();

            // Assert
            Assert.Equal(expectedResult, ((NumberDatum)result).Value);
        }

        [Fact]
        public void Evaluate_ProgramStructure_Correctly()
        {
            // Arrange
            const double NUMBER = 12;
            var program = new Program
            {
                DefinitionsAndExpressions = new List<Node> { new NumberLiteral(NUMBER) }
            };

            // Act
            var result = (NumberDatum)_interpreter.Evaluate(program).Last();

            // Assert
            Assert.Equal(NUMBER, result.Value);
        }

        [Theory]
        [InlineData("(delay 1)", 1)]
        [InlineData("(cdr (cons-stream 1 2))", 2)]
        public void Evaluate_DelayedExpression_ReturnsLambda(string expression, double expectedDelayed)
        {
            // Arrange
            // Act
            var result = _interpreter.Evaluate(expression).Last() as MemoizedUserProcedure;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Body.Expressions.Count);
            Assert.Equal(expectedDelayed, ((NumberLiteral)result!.Body.Expressions[0]).Value);
        }

        [Theory]
        [InlineData("(force (delay 1))", 1)]
        [InlineData("(force (cdr (cons-stream 1 2)))", 2)]
        [InlineData("(cdr-stream (cons-stream 1 3))", 3)]
        public void Evaluate_ForcedDelayedExpression_ReturnsNumberDatum(string expression, double expected)
        {
            // Arrange
            // Act
            var result = (NumberDatum)_interpreter.Evaluate(expression).Last();

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(@"
(define x 1)

(define (foo)
    (set! x (+ x 1))
    (set! x (+ x 1)))
x
(foo)
x
(foo)
x")]
        [InlineData(@"
(define x 1)

(define (foo)
    (set! x (+ x 1))
    (set! x (+ x 1)))
x
(force (delay (foo)))
x
(force (delay (foo)))
x")]
        public void Evaluate_ExpressionTwice_EvaluatesTwice(string code)
        {
            // Arrange
            // Act
            var results = _interpreter.Evaluate(code).ToList();

            // Assert
            Assert.True(results[2] is NumberDatum { Value: 1 });
            Assert.True(results[4] is NumberDatum { Value: 3 });
            Assert.True(results[6] is NumberDatum { Value: 5 });
        }

        [Theory]
        [InlineData(@"
(define x 1)

(define (foo)
    (set! x (+ x 1))
    (set! x (+ x 1)))

(define delayed (delay (foo)))
x
(force delayed)
x
(force delayed)
x")]
        [InlineData(@"
(define x 1)
x
(define delayed
  (delay
    (begin
        (set! x (+ x 1))
        (set! x (+ x 1)))))
x
(force delayed)
x
(force delayed)
x")]
        [InlineData(@"
(define x 1)

(define (foo)
    (set! x (+ x 1))
    (set! x (+ x 1)))

(define delayedPair (cons-stream x (foo)))
x
(cdr-stream delayedPair)
x
(cdr-stream delayedPair)
x")]
        public void Evaluate_ForcedMemoizedProcedureTwice_EvaluatesOnce(string code)
        {
            // Arrange
            // Act
            var results = _interpreter.Evaluate(code).ToList();

            // Assert
            Assert.True(results[3] is NumberDatum { Value: 1 });
            Assert.True(results[5] is NumberDatum { Value: 3 });
            Assert.True(results[7] is NumberDatum { Value: 3 });
        }

        [Fact]
        public void Evaluate_ConsStream_PairOfEvaluatedAndDelayedExpressions()
        {
            // Arrange
            const string CODE = "(cons-stream 0 7)";

            // Act
            var result = _interpreter.Evaluate(CODE).First() as Pair;
            var first = result!.First as NumberDatum;
            var second = result!.Second as MemoizedUserProcedure;

            // Assert
            Assert.Equal(0, first!.Value);
            Assert.Equal(1, second!.Body.Expressions.Count);
            Assert.True(second!.Body.Expressions[0] is NumberLiteral { Value: 7 });
        }

        [Fact]
        public void Evaluate_MakeRangeStream_PairOfEvaluatedAndDelayedExpressions()
        {
            // Arrange
            const string CODE = "(make-range-stream 1 5)";

            // Act
            var result = _interpreter.Evaluate(CODE).First() as Pair;
            var first = result!.First as NumberDatum;
            var second = result!.Second as MemoizedUserProcedure;

            // Assert
            Assert.Equal(1, first!.Value);
            Assert.Equal(1, second!.Body.Expressions.Count);
            Assert.True(second!.Body.Expressions[0] is Application { Operator: Identifier { Name: "make-range-stream" } });
        }
    }
}
