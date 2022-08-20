using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RainLisp;
using RainLisp.Environment;
using RainLisp.Grammar;
using System.Reflection;

namespace RainLispTests
{
    public class EnvironmentTests
    {
        private readonly Interpreter interpreter = new(environmentFactory: new TestableEnvironmentFactory());

        [Theory]
        [InlineData(0, "(define a 0)")]
        [InlineData(1, "(define a 0) (define b 1)")]
        [InlineData(2, "(define a 0) (define b 1) (define ab 10.32)")]
        [InlineData(3, "(define a 2) (define ab 4) (set! a (+ a ab))")]
        [InlineData(4, "(define a 2) ((lambda () (set! a 4))) a")] // the fact that body can have a single expression does not allow this to be evaluated "(define a 2) ((lambda () (set! a 4) a))"
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
                ContractResolver = new PrivateFieldsContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
            IEvaluationEnvironment? environment = null;

            // Act
            interpreter.Evaluate(expression, ref environment);
            // Check the effect of the evaluation on the global environment.
            var environmentJObject = JObject.FromObject(environment, jsonSerializer);

            // Remove the circular references of all previous environments.
            foreach (var token in environmentJObject.SelectTokens("$..previousEnvironment").ToList())
                token.Parent.Remove();

            // Remove the primitives from the definitions output, because we are not interested to check them.
            var fields = typeof(Primitives).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                string? primitiveProcedureName = field?.GetRawConstantValue()?.ToString();
                environmentJObject["actualEnvironment"]["definitions"][primitiveProcedureName].Parent.Remove();
            }

            string actualEnvironment = environmentJObject.ToString();
            string expectedEnvironment = File.ReadAllText($"Environments\\{environmentIndex:00}.json");

            // Assert
            Assert.Equal(expectedEnvironment, actualEnvironment);
        }
    }
}
