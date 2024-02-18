using RainLisp;
using RainLisp.AbstractSyntaxTree;
using RainLisp.DotNetIntegration;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Parsing;
using RainLisp.Tokenization;

namespace RainLispTests
{
    public class DotNetIntegrationExtensionsTests
    {
        private readonly IInterpreter _interpreter;

        public DotNetIntegrationExtensionsTests()
            => _interpreter = new Interpreter();

        [Fact]
        public void EvaluateNow_SingleExpression_Correctly()
        {
            // Arrange
            const string RAIN_LISP_CODE = @"
(define dt (utc-now))
(display ""Generating log file name at "")
(display dt)
(newline)
(+ ""system-"" (datetime-to-string dt ""yyyy-MM-dd_HH-mm-ss-fff"") "".log"")";

            // Act
            var result = _interpreter.Execute(RAIN_LISP_CODE);
            string logFileName = result.String();

            // Assert
            Assert.True(!string.IsNullOrEmpty(logFileName));
        }

        [Fact]
        public void EvaluateNow_ConsecutiveExpressions_Correctly()
        {
            // Arrange
            const string RAIN_LISP_CODE = @"
(define (get-monthly-ratio month)
    (cond ((= month 1) 12.42)
          ((= month 2) 31.71)
          (else 9.32)))";

            // Act
            IEvaluationEnvironment? environment = null;
            _ = _interpreter.Execute(RAIN_LISP_CODE, ref environment);

            var result = _interpreter.Execute("(get-monthly-ratio 2)", ref environment);
            double ratio = result.Number();

            // Assert
            Assert.Equal(31.71, ratio);
        }

        [Fact]
        public void EvaluateNow_AST_Correctly()
        {
            // Arrange
            const double NUMBER = 28;

            var program = new Program
            {
                DefinitionsAndExpressions = new List<Node>
                {
                    new NumberLiteral(NUMBER)
                }
            };

            // Act
            var result = _interpreter.Execute(program);

            // Assert
            Assert.Equal(NUMBER, result.Number());
        }

        [Fact]
        public void EvaluateNow_CodeAndAST_Correctly()
        {
            // Arrange
            const string RAIN_LISP_CODE = @"
(define (get-monthly-ratio month)
    (cond ((= month 1) 12.42)
          ((= month 2) 31.71)
          (else 9.32)))";

            // Act
            // Perform lexical analysis.
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(RAIN_LISP_CODE);

            // Perform syntax analysis.
            var parser = new Parser();
            var program = parser.Parse(tokens);

            // Evaluate to create get-monthly-ratio and keep it in the given environment.
            IEvaluationEnvironment? environment = null;
            _ = _interpreter.Execute(program, ref environment);

            var procedureCallProgram = new Program
            {
                DefinitionsAndExpressions = new List<Node>
                {
                    // Call to get-monthly-ratio with an argument of 2 which is February.
                    new Application(new Identifier("get-monthly-ratio"), new List<Expression> { new NumberLiteral(2) })
                }
            };

            var result = _interpreter.Execute(procedureCallProgram, ref environment);
            double ratio = result.Number();

            // Assert
            Assert.Equal(31.71, ratio);
        }

        [Fact]
        public void EvaluateNow_VariousExpressions_Correctly()
        {
            // Arrange
            const string RAIN_LISP_CODE = @"
; Constructs a data structure with payroll analysis data.
(define (make-payroll-analysis tax insurance net-income married pay-date)
    (list (cons 'tax tax)
          (cons 'insurance insurance)
          (cons 'net-income net-income)
          (cons 'married married)
          (cons 'pay-date pay-date)))

; Getters of individual payroll analysis data.
(define (get-tax payroll-analysis)
    (cdar payroll-analysis))

(define (get-insurance payroll-analysis)
    (cdadr payroll-analysis))

(define (get-net-income payroll-analysis)
    (cdaddr payroll-analysis))

(define (get-marital-status payroll-analysis)
    (cdaddr (cdr payroll-analysis)))

(define (get-paydate payroll-analysis)
    (cdaddr (cddr payroll-analysis)))

; Procedure that calculates and returns the payroll analysis based on the employee's monthly gross income and wether or not they are married.
(define (calculate-payroll monthly-gross-income married)

    ; Set the rates.
    (define tax-rate (if married 18.4 18.6))
    (define insurance-rate 13.9)
    (define net-rate (- 100 tax-rate insurance-rate))

    ; Calculate amounts.
    (define tax (round (* (/ tax-rate 100)
                          monthly-gross-income)
                       2))

    (define insurance (round (* (/ insurance-rate 100)
                                monthly-gross-income)
                             2))

    (define net (round (* (/ net-rate 100)
                          monthly-gross-income)
                       2))

    (define now (utc-now))
    ; Pay on the last day of the current month.
    (define pay-date (add-days
                        (add-months (make-date (year now) (month now) 1) 
                                    1)
                        -1))
    
    (make-payroll-analysis tax insurance net married pay-date))";

            // Act
            // Install the necessary RainLisp code for payroll calculation.
            IEvaluationEnvironment? environment = null;
            _ = _interpreter.Execute(RAIN_LISP_CODE, ref environment);

            // Calculate the payroll details for an unmarried employee that gets a 5000 gross income.
            _ = _interpreter.Execute("(define payroll (calculate-payroll 5000 false))", ref environment);

            // Get the calculated payroll details.
            double tax = _interpreter.Execute("(get-tax payroll)", ref environment).Number();
            double insurance = _interpreter.Execute("(get-insurance payroll)", ref environment).Number();
            double netIncome = _interpreter.Execute("(get-net-income payroll)", ref environment).Number();
            bool isMarried = _interpreter.Execute("(get-marital-status payroll)", ref environment).Bool();
            DateTime payDate = _interpreter.Execute("(get-paydate payroll)", ref environment).DateTime();

            // Assert
            var now = DateTime.UtcNow;
            Assert.Equal(930, tax);
            Assert.Equal(695, insurance);
            Assert.Equal(3375, netIncome);
            Assert.False(isMarried);
            Assert.Equal(new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1), payDate);
        }

        [Theory]
        [InlineData(typeof(StringDatum))]
        [InlineData(typeof(NumberDatum))]
        [InlineData(typeof(BoolDatum))]
        [InlineData(typeof(DateTimeDatum))]
        public void EvaluateNow_UnexcpectedExpression_Throws(Type type)
        {
            // Arrange
            const string RAIN_LISP_CODE = "nil";

            // Act
            var result = _interpreter.Execute(RAIN_LISP_CODE);

            Action<EvaluationResult> extensionMethod = type switch
            {
                Type t when t == typeof(StringDatum) => res => res.String(),
                Type t when t == typeof(NumberDatum) => res => res.Number(),
                Type t when t == typeof(BoolDatum) => res => res.Bool(),
                Type t when t == typeof(DateTimeDatum) => res => res.DateTime(),
                _ => throw new InvalidOperationException("Unexpected primitive type.")
            };

            void CallExtensionMethod() => extensionMethod(result);

            // Assert
            Assert.Throws<InvalidOperationException>(CallExtensionMethod);
        }
    }
}
