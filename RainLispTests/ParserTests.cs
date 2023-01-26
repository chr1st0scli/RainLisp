using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RainLisp;
using RainLisp.Parsing;
using RainLisp.Tokenization;

namespace RainLispTests
{
    public class ParserTests
    {
        private readonly ITokenizer _tokenizer;
        private readonly Parser _parser;

        public ParserTests()
        {
            _tokenizer = new Tokenizer();
            _parser = new Parser();
        }

        [Theory]
        [InlineData(0, "1")]
        [InlineData(1, "1.5")]
        [InlineData(2, "true")]
        [InlineData(3, "false")]
        [InlineData(4, "foo")]
        [InlineData(5, "(quote a)")]
        [InlineData(6, "(set! a 1)")]
        [InlineData(7, "(define a 1)")]
        [InlineData(8, "(define (a) 1)")]
        [InlineData(9, "(define (a b) 1)")]
        [InlineData(10, "(define (a b c) (+ b c))")]
        [InlineData(11, "(define (foo x) (define (innerfoo y) (+ x y)) (innerfoo 1))")]
        [InlineData(12, "(if true 1)")]
        [InlineData(13, "(if true 1 0)")]
        [InlineData(14, "(begin 1)")]
        [InlineData(15, "(begin 1 2 3)")]
        [InlineData(16, "(lambda () 1)")]
        [InlineData(17, "(lambda (x) x)")]
        [InlineData(18, "(lambda (x y) 1)")]
        [InlineData(19, "(lambda (x y) (+ x y))")]
        [InlineData(20, "(foo)")]
        [InlineData(21, "(foo 1)")]
        [InlineData(22, "(foo 1 2 3)")]
        [InlineData(23, "((lambda () 1))")]
        [InlineData(24, "((lambda (x) x) 1)")]
        [InlineData(25, "(cond (true 5))")]
        [InlineData(26, "(cond (true 5) (false 10))")]
        [InlineData(27, "(cond (true 5) (false 10) (else -1))")]
        [InlineData(28, "(cond ((<= a 5) 5) ((<= a 10) 10) (else -1))")]
        [InlineData(29, "(cond (true 1 2) (false 3 4) (else 5 6))")]
        [InlineData(30, "(let ((a 1)) a)")]
        [InlineData(31, "(let ((a 1) (b 2)) (+ a b))")]
        [InlineData(32, "(let ((a 1) (b 2)) (define c 4) (+ a b c))")]
        [InlineData(33, "(let ((a 1) (b 2)) (define c 4) (define d 5) (+ a b c d))")]
        [InlineData(34, "(define (a) 1 2)")]
        [InlineData(35, "(define (a b) 1 2 3)")]
        [InlineData(36, "(define (a b c) 1 2 (+ b c))")]
        [InlineData(37, "(define (foo x) (define (innerfoo y) 1 (+ x y)) 2 (innerfoo 1))")]
        [InlineData(38, "(lambda () 1 2)")]
        [InlineData(39, "(lambda (x) 1 x)")]
        [InlineData(40, "(lambda (x y) 1 2)")]
        [InlineData(41, "(lambda (x y) x y (+ x y))")]
        [InlineData(42, "((lambda () 1 2))")]
        [InlineData(43, "((lambda (x) 2 x) 1)")]
        [InlineData(44, "(let ((a 1)) 1 a)")]
        [InlineData(45, "(let ((a 1) (b 2)) 1 (+ a b))")]
        [InlineData(46, "(let ((a 1) (b 2)) (define c 4) 1 a b c (+ a b c))")]
        [InlineData(47, "(let ((a 1) (b 2)) (define c 4) (define d 5) 1 a b c d (+ a b c d))")]
        [InlineData(48, "(and 1 2 3 4)")]
        [InlineData(49, "(or 1 2 3 4)")]
        [InlineData(50, "(define (foo x y) (define bar 3) (+ x y bar)) (let ((a 1) (b 2)) (define c 4) (+ (foo a b) c))")]
        public void Parse_ValidExpression_GivesExpectedAST(int astIndex, string expression)
        {
            static void RemoveProperty(JObject jObj, string propertyName)
            {
                foreach (var token in jObj.SelectTokens($"$..{propertyName}").ToList())
                    token.Parent.Remove();
            }

            // Arrange
            var tokens = _tokenizer.Tokenize(expression);
            string expectedAst = File.ReadAllText($"AbstractSyntaxTrees\\{astIndex:00}.json");

            // Act
            var programJObj = JObject.FromObject(_parser.Parse(tokens));

            // Remove debug info data. We are only interested in testing the correctness of the AST structure in this context.
            RemoveProperty(programJObj, nameof(IDebugInfo.Line));
            RemoveProperty(programJObj, nameof(IDebugInfo.Position));
            RemoveProperty(programJObj, nameof(IDebugInfo.HasDebugInfo));

            string ast = programJObj.ToString(Formatting.Indented);

            // Assert
            Assert.Equal(expectedAst, ast);
        }

        public enum ParsingError { Definition, Expression, MissingSymbol }

        [Theory]
        // Invalid definitions.
        [InlineData("(define)", 1, 8, ParsingError.Definition)]
        [InlineData("(define 10)", 1, 9, ParsingError.Definition)]
        [InlineData("(define 10 20)", 1, 9, ParsingError.Definition)]
        [InlineData("(define (foo) (define 1 1) 1)", 1, 23, ParsingError.Definition)]
        [InlineData("(define (foo)\n(define 1 1) 1)", 2, 9, ParsingError.Definition)]
        // Missing expressions.
        [InlineData("(begin 1", 1, 9, ParsingError.Expression)]
        [InlineData("(begin)", 1, 7, ParsingError.Expression)]
        [InlineData("(cond (true 1) (false 2) (else)", 1, 31, ParsingError.Expression)]
        [InlineData("(cond\n\t(true 1)\n\t(false 2)\n\t(else)", 4, 7, ParsingError.Expression)]
        [InlineData("(define (foo a))", 1, 16, ParsingError.Expression)]
        [InlineData("(define (foo) (define a 1))", 1, 27, ParsingError.Expression)]
        [InlineData("(define a)", 1, 10, ParsingError.Expression)]
        [InlineData("(foo", 1, 5, ParsingError.Expression)]
        [InlineData("(if)", 1, 4, ParsingError.Expression)]
        [InlineData("(if true)", 1, 9, ParsingError.Expression)]
        [InlineData("(if true 1", 1, 11, ParsingError.Expression)]
        [InlineData("(lambda ())", 1, 11, ParsingError.Expression)]
        [InlineData("(lambda ()\n)", 2, 1, ParsingError.Expression)]
        [InlineData("(lambda () 1", 1, 13, ParsingError.Expression)]
        [InlineData("(lambda ()\n1", 2, 2, ParsingError.Expression)]
        [InlineData("(let ((a)", 1, 9, ParsingError.Expression)]
        [InlineData("(let ((a 1))", 1, 13, ParsingError.Expression)]
        [InlineData("(let ((a 1)) 1", 1, 15, ParsingError.Expression)]
        [InlineData("(let ((a 1) (b)) 1", 1, 15, ParsingError.Expression)]
        [InlineData("(let ((a 1)\n(b)) 1", 2, 3, ParsingError.Expression)]
        [InlineData("(let ((a 1)\n(b 2)) 1", 2, 9, ParsingError.Expression)]
        [InlineData("(set! a)", 1, 8, ParsingError.Expression)]
        [InlineData("(and)", 1, 5, ParsingError.Expression)]    // In traditional Lisp this would return true.
        [InlineData("(or)", 1, 4, ParsingError.Expression)]     // In traditional Lisp this would return false.
        // Missing specific symbols.
        [InlineData("(quote)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(quote a", 1, 9, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(set!)", 1, 6, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! a 1", 1, 10, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(define a 1", 1, 12, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(define (1) 1)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (foo 1)", 1, 14, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (foo) [ define (boo) 1) (boo))", 1, 15, ParsingError.MissingSymbol, TokenType.LParen)]
        [InlineData("(define (foo) \n[ define (boo) 1) (boo))", 2, 1, ParsingError.MissingSymbol, TokenType.LParen)]
        [InlineData("(if true 1 0", 1, 13, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(if true 1\n\t0", 2, 3, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(lambda 1", 1, 9, ParsingError.MissingSymbol, TokenType.LParen)]
        [InlineData("(lambda (1) 1)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(cond 1)", 1, 7, ParsingError.MissingSymbol, TokenType.LParen)]
        [InlineData("(cond (true 1)", 1, 15, ParsingError.MissingSymbol, TokenType.LParen)] // Since there is no ), looking for (.
        [InlineData("(cond (true 1) (false 2)", 1, 25, ParsingError.MissingSymbol, TokenType.LParen)] // Since there is no ), looking for (.
        [InlineData("(cond (true 1)\n(false 2)", 2, 10, ParsingError.MissingSymbol, TokenType.LParen)] // Since there is no ), looking for (.
        [InlineData("(cond (true 1) (false 2) (else 3)", 1, 34, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(cond (true 1)\n(false 2)\n(else 3)", 3, 9, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(let)", 1, 5, ParsingError.MissingSymbol, TokenType.LParen)]
        [InlineData("(let ()", 1, 7, ParsingError.MissingSymbol, TokenType.LParen)]
        [InlineData("(let (()", 1, 8, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(let ((a 1", 1, 11, ParsingError.MissingSymbol, TokenType.RParen)]
        [InlineData("(let ((a 1)", 1, 12, ParsingError.MissingSymbol, TokenType.LParen)] // Since there is no ), looking for (.
        [InlineData("(let ((a 1) (b 21)", 1, 19, ParsingError.MissingSymbol, TokenType.LParen)] // Since there is no ), looking for (.
        [InlineData("(let ((a 1)\n(b 21)", 2, 7, ParsingError.MissingSymbol, TokenType.LParen)] // Since there is no ), looking for (.
        // User cannot redefine or set special forms of the language.
        [InlineData("(define true 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define false 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define quote 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define set! 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define define 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define if 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define cond 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define else 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define begin 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define lambda 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define and 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define or 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(define let 0)", 1, 9, ParsingError.Definition)]
        [InlineData("(set! true 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! false 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! quote 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! set! 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! define 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! if 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! cond 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! else 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! begin 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! lambda 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! and 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! or 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(set! let 0)", 1, 7, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (true) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (false) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (quote) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (set!) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (define) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (if) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (cond) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (else) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (begin) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (lambda) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (and) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (or) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        [InlineData("(define (let) 0)", 1, 10, ParsingError.MissingSymbol, TokenType.Identifier)]
        public void Parse_InvalidExpression_Throws(string expression, uint expectedLine, uint expectedPosition, ParsingError expectedError, TokenType? expectedMissingToken = null)
        {
            // Arrange
            ParsingException? exception = null;
            var expectedMissingSymbols = expectedError switch
            {
                ParsingError.Definition => new[] { TokenType.Identifier, TokenType.LParen },
                ParsingError.Expression => new[] { TokenType.Number, TokenType.String, TokenType.Boolean, TokenType.Identifier, TokenType.LParen },
                ParsingError.MissingSymbol => new[] { expectedMissingToken!.Value },
                _ => throw new NotImplementedException()
            };

            // Act
            var tokens = _tokenizer.Tokenize(expression);
            try { _parser.Parse(tokens); }
            catch (ParsingException ex) { exception = ex; }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ParsingException>(exception);
            Assert.Equal(expectedLine, exception!.Line);
            Assert.Equal(expectedPosition, exception.Position);

            if (expectedError == ParsingError.MissingSymbol)
            {
                Assert.Single(exception.MissingSymbols);
                Assert.Equal(expectedMissingToken, exception.MissingSymbols[0]);
            }
            else
            {
                Assert.Equal(expectedMissingSymbols.Length, exception.MissingSymbols.Length);
                Assert.Equal(exception.MissingSymbols.Intersect(expectedMissingSymbols).ToArray().Length, expectedMissingSymbols.Length);
            }
        }
    }
}
