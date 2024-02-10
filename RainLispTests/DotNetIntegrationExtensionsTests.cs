using RainLisp;
using RainLisp.AbstractSyntaxTree;
using RainLisp.DotNetIntegration;
using RainLisp.Evaluation;
using RainLisp.Parsing;
using RainLisp.Tokenization;

namespace RainLispTests
{
    public class DotNetIntegrationExtensionsTests
    {
        private readonly IInterpreter _interpreter;

        public DotNetIntegrationExtensionsTests()
        {
            _interpreter = new Interpreter();
        }

        [Fact]
        public void Evaluate_SingleExpression_Correctly()
        {
            // Arrange
            const string RAIN_LISP_CODE = @"(define dt (utc-now))
(display ""Generating log file name at "")
(display dt)
(newline)
(+ ""system-"" (datetime-to-string dt ""yyyy-MM-dd_HH-mm-ss-fff"") "".log"")";

            // Act
            var result = _interpreter.EvaluateNow(RAIN_LISP_CODE);
            string logFileName = result.String();

            // Assert
            Assert.True(!string.IsNullOrEmpty(logFileName));
        }

        [Fact]
        public void Evaluate_ConsecutiveExpressions_Correctly()
        {
            // Arrange
            const string RAIN_LISP_CODE = @"
(define (get-monthly-ratio month)
    (cond ((= month 1) 12.42)
          ((= month 2) 31.71)
          (else 9.32)))";

            // Act
            IEvaluationEnvironment? environment = null;
            _ = _interpreter.EvaluateNow(RAIN_LISP_CODE, ref environment);

            var result = _interpreter.EvaluateNow("(get-monthly-ratio 2)", ref environment);
            double ratio = result.Number();

            // Assert
            Assert.Equal(31.71, ratio);
        }

        [Fact]
        public void Evaluate_CodeAndAST_Correctly()
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
            // program is the abstract syntax tree which can be cached. The tokenization and parsing don't need to be repeated again to create the get-monthly-ratio procedure.
            var program = parser.Parse(tokens);

            // Evaluate to create get-monthly-ratio and keep it in the given environment.
            IEvaluationEnvironment? environment = null;
            // We evaluate the abstract syntax tree instead of source code.
            _ = _interpreter.EvaluateNow(program, ref environment);

            // For the actual call to get-monthly-ratio, we are taking a different approach.
            // Since it is just a simple procedure call (application), we build the abstract syntax tree ourselves, effectively treating code as data.
            var procedureCallProgram = new Program
            {
                DefinitionsAndExpressions = new List<Node>
                {
                    // Call to get-monthly-ratio with an argument of 2 which is February.
                    new Application(new Identifier("get-monthly-ratio"), new List<Expression> { new NumberLiteral(2) })
                }
            };

            // We evaluate the manually built abstract syntax tree
            var result = _interpreter.EvaluateNow(procedureCallProgram, ref environment);

            double ratio = result.Number();

            // Assert
            Assert.Equal(31.71, ratio);
        }

        [Fact]
        public void Evaluate_VariousExpressions_Correctly()
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
            _ = _interpreter.EvaluateNow(RAIN_LISP_CODE, ref environment);

            // Calculate the payroll details for an unmarried employee that gets a 5000 gross income.
            _ = _interpreter.EvaluateNow("(define payroll (calculate-payroll 5000 false))", ref environment);

            // Get the calculated payroll details.
            double tax = _interpreter.EvaluateNow("(get-tax payroll)", ref environment).Number();
            double insurance = _interpreter.EvaluateNow("(get-insurance payroll)", ref environment).Number();
            double netIncome = _interpreter.EvaluateNow("(get-net-income payroll)", ref environment).Number();
            bool isMarried = _interpreter.EvaluateNow("(get-marital-status payroll)", ref environment).Bool();
            DateTime payDate = _interpreter.EvaluateNow("(get-paydate payroll)", ref environment).DateTime();

            // Assert
            var now = DateTime.UtcNow;
            Assert.Equal(930, tax);
            Assert.Equal(695, insurance);
            Assert.Equal(3375, netIncome);
            Assert.False(isMarried);
            Assert.Equal(new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1), payDate);
        }
    }
}
