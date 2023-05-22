using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RainLisp;
using RainLisp.Evaluation;
using RainLisp.Evaluation.Results;
using RainLisp.Grammar;
using System.Reflection;

namespace RainLispTests
{
    public class EnvironmentTests
    {
        private readonly Interpreter interpreter = new(installLispLibraries: false) { EnvironmentFactory = new TestableEnvironmentFactory() };

        [Theory]
        [InlineData(0, "(define a 0)")]
        [InlineData(1, "(define a 0) (define b 1)")]
        [InlineData(2, "(define a 0) (define b 1) (define ab 10.32)")]
        [InlineData(3, "(define a 2) (define ab 4) (set! a (+ a ab))")]
        [InlineData(4, "(define a 2) ((lambda () (set! a 4))) a")]
        [InlineData(4, "(define a 2) ((lambda () (set! a 4) a))")]
        [InlineData(5, "(define a 2) ((lambda () (define b 7) (+ a b)))")]
        [InlineData(6, "(define a 2) ((lambda () (define b 7) (begin (set! a 3) (set! b 9) (+ a b))))")]
        [InlineData(7, "(define a 2) ((lambda () (define b 7) (set! a (+ a b)))) a")]
        [InlineData(8, "(define a 1) ((lambda () (define b 7) ((lambda () (define c 9) c)))) a")]
        [InlineData(9, "(define a 1) ((lambda () (define b 7) ((lambda () (define c 9) (begin (set! a 2) (set! b 3) (set! c 4)))))) a")]
        [InlineData(10, "(define a 1) ((lambda () (define a 2) (define b 7) ((lambda () (define b 8) (define c 9) c))))")]
        public void Evaluate_ValidExpression_GivesExpectedEnvironment(int environmentIndex, string expression)
        {
            // Arrange
            // Serialize the global environment's private fields in JSON to compare its internal structure.
            var jsonSerializer = new JsonSerializer
            {
                ContractResolver = new TestableEnvironmentContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
            IEvaluationEnvironment? environment = null;

            // Act
            // Force enumeration to evaluate everything.
            _ = interpreter.Evaluate(expression, ref environment).Last();
            // Check the effect of the evaluation on the global environment.
            var environmentJObject = JObject.FromObject(environment, jsonSerializer);

            // Remove the circular references of all previous environments.
            foreach (var token in environmentJObject.SelectTokens("$.._previousEnvironment").ToList())
                token.Parent.Remove();

            // Remove _quoteSymbols.
            foreach (var token in environmentJObject.SelectTokens("$.._quoteSymbols").ToList())
                token.Parent.Remove();

            // Remove the primitives from the definitions output, because we are not interested to check them.
            var fields = typeof(Primitives).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                string? primitiveProcedureName = field?.GetRawConstantValue()?.ToString();
                environmentJObject["actualEnvironment"]["_definitions"][primitiveProcedureName].Parent.Remove();
            }

            // Flatten the identifier values out of PrimitiveDatum.
            foreach (var definitionToken in environmentJObject.SelectTokens("$.._definitions").ToList())
            {
                foreach (var identifierToken in definitionToken.Children())
                {
                    var identifierValue = identifierToken.First().First().First();
                    identifierToken.First().Replace(identifierValue);
                }

                // Rename _definitions to definitions to match the test files.
                definitionToken.Parent.Replace(new JProperty("definitions", definitionToken));
            }

            string actualEnvironment = environmentJObject.ToString();
            // Files are saved in Windows format. Load them as platform independent.
            string expectedEnvironment = Utils.ReadAllTextOnAnyPlatform(Path.Combine("Environments", $"{environmentIndex:00}.json"));

            // Assert
            Assert.Equal(expectedEnvironment, actualEnvironment);
        }

        [Theory]
        [InlineData("(quote (123 true \"hi\" abc))", "123")]
        [InlineData("(quote (123 true \"hi\" abc))", "true")]
        [InlineData("(quote (123 true \"hi\" abc))", "\"hi\"")]
        [InlineData("(quote (123 true \"hi\" abc))", "abc")]
        [InlineData("(quote (ab cd (ef gh) ik))", "ab")]
        [InlineData("(quote (ab cd (ef gh) ik))", "cd")]
        [InlineData("(quote (ab cd (ef gh) ik))", "ef")]
        [InlineData("(quote (ab cd (ef gh) ik))", "gh")]
        [InlineData("(quote (ab cd (ef gh) ik))", "ik")]
        public void Evalute_QuoteExpression_GivesUniqueQuoteSymbols(string quoteInstall, string quoteToQuery)
        {
            // Test the fact that quote symbols are unique throughout the entire application's life time.

            // Arrange
            string foo = $"(define (foo) {quoteInstall}) (foo) (quote {quoteToQuery})";
            string bar = $"(define (bar) {quoteInstall}) (bar) (quote {quoteToQuery})";
            IEvaluationEnvironment? environment = null;

            // Act
            var fooResult = interpreter.Evaluate(foo, ref environment).Last() as QuoteSymbol;
            var barResult = interpreter.Evaluate(bar, ref environment).Last() as QuoteSymbol;

            environment!.TryGetQuoteSymbol(quoteToQuery, out var storedQuoteSymbol);

            // Assert
            Assert.True(ReferenceEquals(fooResult, barResult));
            Assert.True(ReferenceEquals(storedQuoteSymbol, barResult));
        }
    }
}
