using RainLisp.Grammar;

namespace RainLispConsole.CodeTextView
{
    internal static class CodeSuggestionsProvider
    {
        public static List<string> GetRainLispSuggestions()
        {
            List<string> list = new();

            LoadRainLispKeywords(list);
            LoadRainLispPrimitives(list);

            return list;
        }

        public static void LoadRainLispKeywords(List<string> list)
            => list.AddRange(GetFields(typeof(Keywords)));

        public static void LoadRainLispPrimitives(List<string> list)
            => list.AddRange(GetFields(typeof(Primitives)));

        private static string[] GetFields(Type type)
        {
            var fieldsInfo = type.GetFields();

            return fieldsInfo.Select(fi => fi.GetValue(null)?.ToString()).Where(s => s != null).ToArray()!;
        }
    }
}
