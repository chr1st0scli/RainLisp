using RainLisp;
using RainLisp.Evaluation;
using RainLisp.Grammar;
using System.Reflection;

namespace RainLispConsole.CodeTextView
{
    internal static class CodeSuggestionsProvider
    {
        public static List<string> GetRainLispSuggestions()
        {
            List<string> list = new();

            LoadRainLispKeywords(list);
            LoadRainLispProcedureNames(list);

            return list;
        }

        private static void LoadRainLispKeywords(List<string> list)
        {
            var fieldsInfo = typeof(Keywords).GetFields(BindingFlags.Public | BindingFlags.Static);

            IEnumerable<string> keywords = fieldsInfo.Select(fi => fi.GetValue(null)?.ToString()).Where(s => s != null)!;

            list.AddRange(keywords);
        }

        private static void LoadRainLispProcedureNames(List<string> list)
        {
            var interpreter = new Interpreter();
            IEvaluationEnvironment? environment = null;
            interpreter.Evaluate(new RainLisp.AbstractSyntaxTree.Program(), ref environment);

            if (environment != null)
                list.AddRange(environment.GetIdentifierNames());
        }
    }
}
