using RainLisp;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Parsing;
using RainLisp.Tokenization;
using static RainLispTests.Utils;

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
            void PrintError(string message, Exception ex, bool unknownError) => exception = ex;

            // Act
            _interpreter.EvaluateAndPrint(expression, ref environment, str => { }, PrintError);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType(expectedExceptionType, exception);
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
        [InlineData("({0} 1)", 0, 1)]
        [InlineData("({0} 1 2)", 0, 2)]
        public void Evaluate_CallExpectingZeroWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[] { "newline", "now", "utc-now" }, expression, expected, false, actual);
        }

        [Theory]
        [InlineData("({0})", 1, 0)]
        [InlineData("({0} 1 2)", 1, 2)]
        public void Evaluate_CallExpectingOneWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[]
                {
                    "not", "car", "cdr", "null?", "display", "debug", "trace", "error", "string-length", "to-lower", "to-upper",
                    "year", "month", "day", "hour", "minute", "second", "millisecond", "utc?", "to-local", "to-utc", "parse-number",
                    "eval", "pair?", "ceiling", "floor"
                }, expression, expected, false, actual);
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
                    "add-years", "add-months", "add-days", "add-hours", "add-minutes", "add-seconds", "add-milliseconds", "number-to-string", "parse-number-culture",
                    "days-diff", "hours-diff", "minutes-diff", "seconds-diff", "milliseconds-diff", "parse-datetime", "datetime-to-string", "round"
                }, expression, expected, false, actual);
        }

        [Theory]
        [InlineData("({0})", 2, 0)]
        [InlineData("({0} 1)", 2, 1)]
        public void Evaluate_CallExpectingTwoOrMoreWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[] { "+", "-", "*", "/", "%", "xor" }, expression, expected, true, actual);
        }

        [Theory]
        [InlineData("({0})", 3, 0)]
        [InlineData("({0} 1)", 3, 1)]
        [InlineData("({0} 1 2)", 3, 2)]
        [InlineData("({0} 1 2 3 4)", 3, 4)]
        public void Evaluate_CallExpectingThreeWithWrongNumberOfArguments_Throws(string expression, int expected, int actual)
        {
            Evaluate_CallsWithWrongNumberOfArguments_Throws(new[] { "substring", "index-of-string", "replace-string" }, expression, expected, false, actual);
        }

        [Theory]
        // make-date
        [InlineData("(make-date nil 2 3)", typeof(Nil), typeof(NumberDatum))]
        [InlineData("(make-date 2022 (cons 1 2) 3)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("(make-date 2022 1 +)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("(make-date (lambda () 1) 1 2)", typeof(UserProcedure), typeof(NumberDatum))]
        [InlineData("(make-date 2022 (newline) 3)", typeof(Unspecified), typeof(NumberDatum))]
        [InlineData("(make-date 2022 2 true)", typeof(BoolDatum), typeof(NumberDatum))]
        [InlineData("(make-date \"hello\" 2 3)", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("(make-date 2022 (now) 3)", typeof(DateTimeDatum), typeof(NumberDatum))]
        // make-datetime
        [InlineData("(make-datetime nil 1 1 2 3 2 3)", typeof(Nil), typeof(NumberDatum))]
        [InlineData("(make-datetime 2022 (cons 1 2) 1 2 3 4 3)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("(make-datetime 2022 1 2 3 4 1 +)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("(make-datetime 2022 1 2 3 (lambda () 1) 1 2)", typeof(UserProcedure), typeof(NumberDatum))]
        [InlineData("(make-datetime 2022 1 2 3 4 (newline) 3)", typeof(Unspecified), typeof(NumberDatum))]
        [InlineData("(make-datetime 2022 1 2 3 4 2 true)", typeof(BoolDatum), typeof(NumberDatum))]
        [InlineData("(make-datetime 2022 1 2 3 \"hello\" 2 3)", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("(make-datetime 2022 1 2 3 4 (now) 3)", typeof(DateTimeDatum), typeof(NumberDatum))]
        // substring
        [InlineData("(substring 1 2 3)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(substring true 2 3)", typeof(BoolDatum), typeof(StringDatum))]
        [InlineData("(substring (now) 2 3)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(substring \"hello\" nil 3)", typeof(Nil), typeof(NumberDatum))]
        [InlineData("(substring \"hello\" (newline) 3)", typeof(Unspecified), typeof(NumberDatum))]
        [InlineData("(substring \"hello\" (cons 1 2) 3)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("(substring \"hello\" 1 \"world\")", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("(substring \"hello\" 1 +)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("(substring \"hello\" 1 (lambda () 1))", typeof(UserProcedure), typeof(NumberDatum))]
        // index-of-string
        [InlineData("(index-of-string 1 2 3)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(index-of-string true 2 3)", typeof(BoolDatum), typeof(StringDatum))]
        [InlineData("(index-of-string (now) 2 3)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(index-of-string \"hello\" nil 3)", typeof(Nil), typeof(StringDatum))]
        [InlineData("(index-of-string \"hello\" (newline) 3)", typeof(Unspecified), typeof(StringDatum))]
        [InlineData("(index-of-string \"hello\" (cons 1 2) 3)", typeof(Pair), typeof(StringDatum))]
        [InlineData("(index-of-string \"hello\" 1 \"world\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(index-of-string \"hello\" \"el\" +)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("(index-of-string \"hello\" \"el\" (lambda () 1))", typeof(UserProcedure), typeof(NumberDatum))]
        // replace-string
        [InlineData("(replace-string 1 2 3)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(replace-string true 2 3)", typeof(BoolDatum), typeof(StringDatum))]
        [InlineData("(replace-string (now) 2 3)", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("(replace-string \"hello\" nil 3)", typeof(Nil), typeof(StringDatum))]
        [InlineData("(replace-string \"hello\" (newline) 3)", typeof(Unspecified), typeof(StringDatum))]
        [InlineData("(replace-string \"hello\" (cons 1 2) 3)", typeof(Pair), typeof(StringDatum))]
        [InlineData("(replace-string \"hello\" 1 \"world\")", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("(replace-string \"hello\" \"el\" +)", typeof(PrimitiveProcedure), typeof(StringDatum))]
        [InlineData("(replace-string \"hello\" \"el\" (lambda () 1))", typeof(UserProcedure), typeof(StringDatum))]
        // Various
        [InlineData("(length 1)", typeof(NumberDatum), typeof(Pair))]
        [InlineData("(length (filter-stream (lambda(x) (> x 10)) (make-range-stream 1 50)))", typeof(MemoizedUserProcedure), typeof(Pair))]
        public void Evaluate_WrongTypeOfArgument_Throws(string expression, Type actual, Type expected)
        {
            // Arrange
            // Act
            var exception = Evaluate_WrongExpression_Throws<WrongTypeOfArgumentException>(expression);

            // Assert
            Assert.Single(exception.Expected);
            Assert.Equal(expected, exception.Expected[0]);
            Assert.Equal(actual, exception.Actual);
        }

        [Theory]
        [InlineData("({0} \"hi\" 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 \"hi\")", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("({0} 1 false)", typeof(BoolDatum), typeof(NumberDatum))]
        [InlineData("({0} true 1)", typeof(BoolDatum), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} \"hello\" true)", typeof(BoolDatum), typeof(StringDatum))]
        [InlineData("({0} true true)", typeof(BoolDatum), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} nil 1)", typeof(Nil), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 nil)", typeof(Nil), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" nil)", typeof(Nil), typeof(StringDatum))]
        [InlineData("({0} nil nil)", typeof(Nil), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 (cons 3 4))", typeof(Pair), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" (cons 3 4))", typeof(Pair), typeof(StringDatum))]
        [InlineData("({0} (cons 1 2) (cons 3 4))", typeof(Pair), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} (lambda(x) x) 1)", typeof(UserProcedure), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 (lambda(x) x))", typeof(UserProcedure), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" (lambda(x) x))", typeof(UserProcedure), typeof(StringDatum))]
        [InlineData("({0} (lambda(x) x) (lambda(x) x))", typeof(UserProcedure), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} - 1)", typeof(PrimitiveProcedure), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 -)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" -)", typeof(PrimitiveProcedure), typeof(StringDatum))]
        [InlineData("({0} - -)", typeof(PrimitiveProcedure), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} (now) 1)", typeof(DateTimeDatum), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 (now))", typeof(DateTimeDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" (now))", typeof(DateTimeDatum), typeof(StringDatum))]
        [InlineData("({0} (now) (now))", typeof(DateTimeDatum), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified), typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 (newline))", typeof(Unspecified), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" (newline))", typeof(Unspecified), typeof(StringDatum))]
        [InlineData("({0} (newline) (newline))", typeof(Unspecified), typeof(NumberDatum), typeof(StringDatum))]
        public void Evaluate_CallExpectingNumbersOrStringsWithWrongTypeOfArgument_Throws(string expression, Type actual, params Type[] expected)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "+" }, expression, actual, expected);
        }

        [Theory]
        [InlineData("({0} \"hi\")", typeof(StringDatum))]
        [InlineData("({0} false)", typeof(BoolDatum))]
        [InlineData("({0} true)", typeof(BoolDatum))]
        [InlineData("({0} nil)", typeof(Nil))]
        [InlineData("({0} (cons 1 2))", typeof(Pair))]
        [InlineData("({0} (lambda(x) x))", typeof(UserProcedure))]
        [InlineData("({0} -)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (now))", typeof(DateTimeDatum))]
        [InlineData("({0} (newline))", typeof(Unspecified))]
        public void Evaluate_CallExpectingNumberWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "ceiling", "floor" }, expression, actual, typeof(NumberDatum));
        }

        [Theory]
        [InlineData("({0} \"hi\" 1)", typeof(StringDatum))]
        [InlineData("({0} 1 \"hi\")", typeof(StringDatum))]
        [InlineData("({0} \"hi\" \"there\")", typeof(StringDatum))]
        [InlineData("({0} 1 false)", typeof(BoolDatum))]
        [InlineData("({0} true 1)", typeof(BoolDatum))]
        [InlineData("({0} true true)", typeof(BoolDatum))]
        [InlineData("({0} nil 1)", typeof(Nil))]
        [InlineData("({0} 1 nil)", typeof(Nil))]
        [InlineData("({0} nil nil)", typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair))]
        [InlineData("({0} 1 (cons 3 4))", typeof(Pair))]
        [InlineData("({0} (cons 1 2) (cons 3 4))", typeof(Pair))]
        [InlineData("({0} (lambda(x) x) 1)", typeof(UserProcedure))]
        [InlineData("({0} 1 (lambda(x) x))", typeof(UserProcedure))]
        [InlineData("({0} (lambda(x) x) (lambda(x) x))", typeof(UserProcedure))]
        [InlineData("({0} - 1)", typeof(PrimitiveProcedure))]
        [InlineData("({0} 1 -)", typeof(PrimitiveProcedure))]
        [InlineData("({0} - -)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (now) 1)", typeof(DateTimeDatum))]
        [InlineData("({0} 1 (now))", typeof(DateTimeDatum))]
        [InlineData("({0} (now) (now))", typeof(DateTimeDatum))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified))]
        [InlineData("({0} 1 (newline))", typeof(Unspecified))]
        [InlineData("({0} (newline) (newline))", typeof(Unspecified))]
        public void Evaluate_CallExpectingNumbersWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "-", "*", "/", "%", "round" }, expression, actual, typeof(NumberDatum));
        }

        [Theory]
        [InlineData("({0} \"hi\" 1)", typeof(StringDatum), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 \"hi\")", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hi\" \"there\")", typeof(StringDatum), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 false)", typeof(BoolDatum), typeof(NumberDatum))]
        [InlineData("({0} true 1)", typeof(BoolDatum), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} true true)", typeof(BoolDatum), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} nil 1)", typeof(Nil), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 nil)", typeof(Nil), typeof(NumberDatum))]
        [InlineData("({0} nil nil)", typeof(Nil), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 (cons 3 4))", typeof(Pair), typeof(NumberDatum))]
        [InlineData("({0} (cons 1 2) (cons 3 4))", typeof(Pair), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} (lambda(x) x) 1)", typeof(UserProcedure), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 (lambda(x) x))", typeof(UserProcedure), typeof(NumberDatum))]
        [InlineData("({0} (lambda(x) x) (lambda(x) x))", typeof(UserProcedure), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} - 1)", typeof(PrimitiveProcedure), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 -)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("({0} - -)", typeof(PrimitiveProcedure), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} (now) 1)", typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 (now))", typeof(DateTimeDatum), typeof(NumberDatum))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified), typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 (newline))", typeof(Unspecified), typeof(NumberDatum))]
        [InlineData("({0} (newline) (newline))", typeof(Unspecified), typeof(NumberDatum), typeof(DateTimeDatum))]
        public void Evaluate_CallExpectingNumbersOrDateTimesWithWrongTypeOfArgument_Throws(string expression, Type actual, params Type[] expected)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { ">", ">=", "<", "<=" }, expression, actual, expected);
        }

        [Theory]
        [InlineData("({0} \"hi\")", typeof(StringDatum))]
        [InlineData("({0} 1)", typeof(NumberDatum))]
        [InlineData("({0} true)", typeof(BoolDatum))]
        [InlineData("({0} (now))", typeof(DateTimeDatum))]
        [InlineData("({0} nil)", typeof(Nil))]
        [InlineData("({0} (newline))", typeof(Unspecified))]
        [InlineData("({0} (lambda(x) x))", typeof(UserProcedure))]
        [InlineData("({0} -)", typeof(PrimitiveProcedure))]
        public void Evaluate_CallExpectingPairWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "car", "cdr" }, expression, actual, typeof(Pair));
        }

        [Theory]
        [InlineData("({0} \"hi\" 1)", typeof(StringDatum))]
        [InlineData("({0} 1 1)", typeof(NumberDatum))]
        [InlineData("({0} true 1)", typeof(BoolDatum))]
        [InlineData("({0} (now) 1)", typeof(DateTimeDatum))]
        [InlineData("({0} nil 1)", typeof(Nil))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified))]
        [InlineData("({0} (lambda(x) x) 1)", typeof(UserProcedure))]
        [InlineData("({0} - 1)", typeof(PrimitiveProcedure))]
        public void Evaluate_CallExpectingPairAndAnythingWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "set-car!", "set-cdr!" }, expression, actual, typeof(Pair));
        }

        [Theory]
        [InlineData("({0} nil)", typeof(Nil))]
        [InlineData("({0} (newline))", typeof(Unspecified))]
        [InlineData("({0} (cons 1 2))", typeof(Pair))]
        [InlineData("({0} +)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1))", typeof(UserProcedure))]
        public void Evaluate_CallExpectingPrimitiveWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "display", "debug", "trace", "error" }, expression, actual, typeof(IPrimitiveDatum));
        }

        [Theory]
        [InlineData("({0} nil)", typeof(Nil))]
        [InlineData("({0} (cons 1 2))", typeof(Pair))]
        [InlineData("({0} +)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1))", typeof(UserProcedure))]
        [InlineData("({0} (newline))", typeof(Unspecified))]
        [InlineData("({0} true)", typeof(BoolDatum))]
        [InlineData("({0} \"hello\")", typeof(StringDatum))]
        public void Evaluate_CallExpectingDateTimeWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "year", "month", "day", "hour", "minute", "second", "millisecond", "utc?", "to-local", "to-utc" }, expression, actual, typeof(DateTimeDatum));
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(Nil), typeof(DateTimeDatum))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair), typeof(DateTimeDatum))]
        [InlineData("({0} + 1)", typeof(PrimitiveProcedure), typeof(DateTimeDatum))]
        [InlineData("({0} (lambda () 1) 1)", typeof(UserProcedure), typeof(DateTimeDatum))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified), typeof(DateTimeDatum))]
        [InlineData("({0} true 1)", typeof(BoolDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 1)", typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(StringDatum), typeof(DateTimeDatum))]
        [InlineData("({0} (now) nil)", typeof(Nil), typeof(NumberDatum))]
        [InlineData("({0} (now) (cons 1 2))", typeof(Pair), typeof(NumberDatum))]
        [InlineData("({0} (now) +)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("({0} (now) (lambda () 1))", typeof(UserProcedure), typeof(NumberDatum))]
        [InlineData("({0} (now) (newline))", typeof(Unspecified), typeof(NumberDatum))]
        [InlineData("({0} (now) true)", typeof(BoolDatum), typeof(NumberDatum))]
        [InlineData("({0} (now) \"hello\")", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("({0} (now) (now))", typeof(DateTimeDatum), typeof(NumberDatum))]
        public void Evaluate_CallExpectingDateTimeAndNumberWithWrongTypeOfArgument_Throws(string expression, Type actual, params Type[] expected)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "add-years", "add-months", "add-days", "add-hours", "add-minutes", "add-seconds", "add-milliseconds" }, expression, actual, expected);
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair))]
        [InlineData("({0} + 1)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1) 1)", typeof(UserProcedure))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified))]
        [InlineData("({0} true 1)", typeof(BoolDatum))]
        [InlineData("({0} 1 (now))", typeof(NumberDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(StringDatum))]
        [InlineData("({0} (now) nil)", typeof(Nil))]
        [InlineData("({0} (now) (cons 1 2))", typeof(Pair))]
        [InlineData("({0} (now) +)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (now) (lambda () 1))", typeof(UserProcedure))]
        [InlineData("({0} (now) (newline))", typeof(Unspecified))]
        [InlineData("({0} (now) true)", typeof(BoolDatum))]
        [InlineData("({0} (now) 1)", typeof(NumberDatum))]
        [InlineData("({0} (now) \"hello\")", typeof(StringDatum))]
        public void Evaluate_CallExpecting2DateTimesWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "days-diff", "hours-diff", "minutes-diff", "seconds-diff", "milliseconds-diff" }, expression, actual, typeof(DateTimeDatum));
        }

        [Theory]
        [InlineData("({0} nil)", typeof(Nil))]
        [InlineData("({0} (cons 1 2))", typeof(Pair))]
        [InlineData("({0} +)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1))", typeof(UserProcedure))]
        [InlineData("({0} (newline))", typeof(Unspecified))]
        [InlineData("({0} true)", typeof(BoolDatum))]
        [InlineData("({0} (now))", typeof(DateTimeDatum))]
        [InlineData("({0} 1)", typeof(NumberDatum))]
        public void Evaluate_CallExpectingStringWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "string-length", "to-lower", "to-upper", "parse-number" }, expression, actual, typeof(StringDatum));
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(Nil))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair))]
        [InlineData("({0} + 1)", typeof(PrimitiveProcedure))]
        [InlineData("({0} (lambda () 1) 1)", typeof(UserProcedure))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified))]
        [InlineData("({0} true 1)", typeof(BoolDatum))]
        [InlineData("({0} (now) 1)", typeof(DateTimeDatum))]
        [InlineData("({0} 1 \"hello\")", typeof(NumberDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(NumberDatum))]
        [InlineData("({0} \"hello\" nil)", typeof(Nil))]
        [InlineData("({0} \"hello\" (cons 1 2))", typeof(Pair))]
        [InlineData("({0} \"hello\" +)", typeof(PrimitiveProcedure))]
        [InlineData("({0} \"hello\" (lambda () 1))", typeof(UserProcedure))]
        [InlineData("({0} \"hello\" (newline))", typeof(Unspecified))]
        [InlineData("({0} \"hello\" true)", typeof(BoolDatum))]
        public void Evaluate_CallExpecting2StringsWithWrongTypeOfArgument_Throws(string expression, Type actual)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "parse-datetime", "parse-number-culture" }, expression, actual, typeof(StringDatum));
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(Nil), typeof(DateTimeDatum))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair), typeof(DateTimeDatum))]
        [InlineData("({0} + 1)", typeof(PrimitiveProcedure), typeof(DateTimeDatum))]
        [InlineData("({0} (lambda () 1) 1)", typeof(UserProcedure), typeof(DateTimeDatum))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified), typeof(DateTimeDatum))]
        [InlineData("({0} true 1)", typeof(BoolDatum), typeof(DateTimeDatum))]
        [InlineData("({0} 1 1)", typeof(NumberDatum), typeof(DateTimeDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(StringDatum), typeof(DateTimeDatum))]
        [InlineData("({0} (now) nil)", typeof(Nil), typeof(StringDatum))]
        [InlineData("({0} (now) (cons 1 2))", typeof(Pair), typeof(StringDatum))]
        [InlineData("({0} (now) +)", typeof(PrimitiveProcedure), typeof(StringDatum))]
        [InlineData("({0} (now) (lambda () 1))", typeof(UserProcedure), typeof(StringDatum))]
        [InlineData("({0} (now) (newline))", typeof(Unspecified), typeof(StringDatum))]
        [InlineData("({0} (now) true)", typeof(BoolDatum), typeof(StringDatum))]
        [InlineData("({0} (now) 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} (now) (now))", typeof(DateTimeDatum), typeof(StringDatum))]
        public void Evaluate_CallExpectingDateTimeAndStringWithWrongTypeOfArgument_Throws(string expression, Type actual, params Type[] expected)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "datetime-to-string" }, expression, actual, expected);
        }

        [Theory]
        [InlineData("({0} nil 1)", typeof(Nil), typeof(NumberDatum))]
        [InlineData("({0} (cons 1 2) 1)", typeof(Pair), typeof(NumberDatum))]
        [InlineData("({0} + 1)", typeof(PrimitiveProcedure), typeof(NumberDatum))]
        [InlineData("({0} (lambda () 1) 1)", typeof(UserProcedure), typeof(NumberDatum))]
        [InlineData("({0} (newline) 1)", typeof(Unspecified), typeof(NumberDatum))]
        [InlineData("({0} true 1)", typeof(BoolDatum), typeof(NumberDatum))]
        [InlineData("({0} \"hello\" 1)", typeof(StringDatum), typeof(NumberDatum))]
        [InlineData("({0} (now) 1)", typeof(DateTimeDatum), typeof(NumberDatum))]
        [InlineData("({0} 1 nil)", typeof(Nil), typeof(StringDatum))]
        [InlineData("({0} 1 (cons 1 2))", typeof(Pair), typeof(StringDatum))]
        [InlineData("({0} 1 +)", typeof(PrimitiveProcedure), typeof(StringDatum))]
        [InlineData("({0} 1 (lambda () 1))", typeof(UserProcedure), typeof(StringDatum))]
        [InlineData("({0} 1 (newline))", typeof(Unspecified), typeof(StringDatum))]
        [InlineData("({0} 1 true)", typeof(BoolDatum), typeof(StringDatum))]
        [InlineData("({0} 1 1)", typeof(NumberDatum), typeof(StringDatum))]
        [InlineData("({0} 1 (now))", typeof(DateTimeDatum), typeof(StringDatum))]
        public void Evaluate_CallExpectingNumberAndStringWithWrongTypeOfArgument_Throws(string expression, Type actual, params Type[] expected)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "number-to-string" }, expression, actual, expected);
        }

        [Theory]
        [InlineData("({0} 1)", typeof(NumberDatum), typeof(QuoteSymbol), typeof(Pair))]
        [InlineData("({0} true)", typeof(BoolDatum), typeof(QuoteSymbol), typeof(Pair))]
        [InlineData("({0} \"\")", typeof(StringDatum), typeof(QuoteSymbol), typeof(Pair))]
        [InlineData("({0} (now))", typeof(DateTimeDatum), typeof(QuoteSymbol), typeof(Pair))]
        [InlineData("({0} nil)", typeof(Nil), typeof(QuoteSymbol), typeof(Pair))]   // Empty list is not acceptable.
        [InlineData("({0} '())", typeof(Nil), typeof(QuoteSymbol), typeof(Pair))]   // Empty list is not acceptable.
        [InlineData("({0} (cons 10 'b))", typeof(NumberDatum), typeof(QuoteSymbol))]
        [InlineData("({0} (cons 'a 'b))", typeof(QuoteSymbol), typeof(Pair), typeof(Nil))] // 'b should be a pair or nil so that we are dealing with a list.
        [InlineData("({0} (list 'a 'b (cons 'c true)))", typeof(BoolDatum), typeof(Pair), typeof(Nil))] // true should be a pair or nil so that we are dealing with a list.
        [InlineData("({0} (list 'ab 10))", typeof(NumberDatum), typeof(QuoteSymbol))]
        [InlineData("({0} (list 'ab true 'cd))", typeof(BoolDatum), typeof(QuoteSymbol))]
        [InlineData("({0} (list 'ab (list 12 'ef) 'gh))", typeof(NumberDatum), typeof(QuoteSymbol))]
        [InlineData("({0} (list 'ab (list 'cd 12) 'gh))", typeof(NumberDatum), typeof(QuoteSymbol))]
        [InlineData("({0} (list false (list 'cd 12) 'gh))", typeof(BoolDatum), typeof(QuoteSymbol))]
        [InlineData("({0} (list 'ab (list 'cd 'ef) (list (list 'gh \"\")) 'ik))", typeof(StringDatum), typeof(QuoteSymbol))]
        public void Evaluate_EvalPrimitiveWithWrongTypeOfArgument_Throws(string expression, Type actual, params Type[] expected)
        {
            Evaluate_CallsWithWrongExpression_Throws(new[] { "eval" }, expression, actual, expected);
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
        [InlineData("(parse-datetime \"\" \"yyyy-MM-dd\")")]
        [InlineData("(parse-datetime \"2022/12/31\" \"\")")]
        [InlineData("(parse-datetime \"2022/12/31\" \"yyyy-MM-dd\")")]
        [InlineData("(datetime-to-string (now) \"q\")")]
        [InlineData("(number-to-string 1 \"q\")")]
        [InlineData("(number-to-string 1 \"i\")")]
        [InlineData("(substring \"hello\" -1 3)")]
        [InlineData("(substring \"hello\" 7 3)")]
        [InlineData("(substring \"hello\" 0 -1)")]
        [InlineData("(substring \"hello\" 0 7)")]
        [InlineData("(index-of-string \"hello\" \"el\" -1)")]
        [InlineData("(index-of-string \"hello\" \"el\" 6)")]
        [InlineData("(replace-string \"hello\" \"\" \"world\")")]
        [InlineData("(parse-number \"\")")]
        [InlineData("(parse-number \"a\")")]
        [InlineData("(parse-number-culture \"\" \"EN\")")]
        [InlineData("(parse-number-culture \"a\" \"EN\")")]
        [InlineData("(parse-number-culture \"1\" \"WQXY\")")]
        [InlineData("(round 2.5 -1)")]
        [InlineData("(round 2.5 29)")]
        public void Evaluate_ExpressionWithInvalidValue_Throws(string expression)
        {
            Evaluate_WrongExpression_Throws<InvalidValueException>(expression);
        }

        [Theory]
        [MemberData(nameof(GetCallStackData))]
        public void Evaluate_ExpressionWithError_ErrorWithCorrectCallStack(string expression, IDebugInfo[] expectedCallStack)
        {
            // Arrange
            EvaluationException? exception = null;

            // Act
            try
            {
                // Force enumeration to evaluate everything.
                _ = _interpreter.Evaluate(expression).Last();
            }
            catch (EvaluationException ex)
            {
                exception = ex;
            }
            var actualCallStack = exception!.CallStack;

            // Assert
            Assert.Equal(expectedCallStack.Length, exception.CallStack!.Count);
            for (int i = 0; i < expectedCallStack.Length; i++)
            {
                Assert.Equal(expectedCallStack[i].Line, actualCallStack![i].Line);
                Assert.Equal(expectedCallStack[i].Position, actualCallStack[i].Position);
            }
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

        private void Evaluate_CallsWithWrongExpression_Throws(string[] procedureNames, string expression, Type actual, params Type[] expected)
        {
            // Arrange
            // Act
            foreach (string procName in procedureNames)
            {
                var exception = Evaluate_WrongExpression_Throws<WrongTypeOfArgumentException>(string.Format(expression, procName));

                // Assert
                Assert.Equal(expected.Length, exception.Expected.Length);

                for (int i = 0; i < exception.Expected.Length; i++)
                    Assert.Equal(expected[i], exception.Expected[i]);

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
                // Force enumeration to evaluate everything.
                _ = _interpreter.Evaluate(expression).Last();
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

        private static TheoryData<string, IDebugInfo[]> GetCallStackData()
        {
            var data = new TheoryData<string, IDebugInfo[]>
            {
                { "(+ 12)", new[] { new TestDebugInfo(1, 2) } },    // WrongNumberOfArgumentsException
                { "(round \"hello\" 2)", new[] { new TestDebugInfo(1, 2) } },   // WrongTypeOfArgumentException
                { "a", new[] { new TestDebugInfo(1, 1) } },   // UnknownIdentifierException
                { "(set! a 3)", new[] { new TestDebugInfo(1, 2) } },   // UnknownIdentifierException
                { "(1)", new[] { new TestDebugInfo(1, 2) } },   // NotProcedureException
                { "(error \"user error\")", new[] { new TestDebugInfo(1, 2) } },   // UserException
                { "(index-of-string \"hello world\" \"lo wo\" -1)", new[] { new TestDebugInfo(1, 2) } },   // InvalidValueException
            };

            string code = @"
(begin 1
    2
    3
    (+))";
            data.Add(code, new[] { new TestDebugInfo(PickLine(5), 6) });

            code = @"
(if true
    (+)
    (-))";
            data.Add(code, new[] { new TestDebugInfo(PickLine(3), 6) });

            code = @"
(if false
    (+)
    (-))";
            data.Add(code, new[] { new TestDebugInfo(PickLine(4), 6) });

            code = @"
(let ((a 1) 
      (b (+ 1))) 
     (+ a))";
            data.Add(code, new[] { new TestDebugInfo(PickLine(3), 11), new TestDebugInfo(PickLine(2), 2) });

            code = @"
(let ((a 1) 
      (b (+ 1 2))) 
     (+ a))";
            data.Add(code, new[] { new TestDebugInfo(PickLine(4), 7), new TestDebugInfo(PickLine(2), 2) });

            code = @"
(define (get-lambda)
    (lambda ()
        (+)))

((get-lambda))";
            data.Add(code, new[] { new TestDebugInfo(PickLine(4), 10), new TestDebugInfo(PickLine(6), 2) });

            code = @"
((lambda () 
    (+ 3)) 43)";
            data.Add(code, new[] { new TestDebugInfo(PickLine(2), 2) });

            code = @"
((lambda () 
    (+ 3)))";
            data.Add(code, new[] { new TestDebugInfo(PickLine(3), 6), new TestDebugInfo(PickLine(2), 2) });

            // Application of an identifier is two calls in the evaluation stack, identifier evaluation and then application.
            code = "(a)";
            data.Add(code, new[] { new TestDebugInfo(1, 2), new TestDebugInfo(1, 2) });

            code = @"
(define (foo)
    (boo))

(define (boo)
    (foo-bar))

(define (foo-bar)
    (+))

(foo)";
            data.Add(code, new[] { new TestDebugInfo(PickLine(9), 6), new TestDebugInfo(PickLine(6), 6), new TestDebugInfo(PickLine(3), 6), new TestDebugInfo(PickLine(11), 2) });

            return data;
        }
    }
}
