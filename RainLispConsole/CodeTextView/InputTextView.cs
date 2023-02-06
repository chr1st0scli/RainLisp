using RainLisp.Grammar;
using System.Text.RegularExpressions;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace RainLispConsole.CodeTextView
{
    internal class InputTextView : TextView
    {
        private readonly Attribute _white;
        private readonly Attribute _keywordColor;
        private readonly Attribute _stringColor;
        private readonly Attribute _commentColor;
        private readonly HashSet<string> _keywords;
        private readonly Dictionary<List<Rune>, LineInfo> _linesAnalysis;

        public InputTextView()
        {
            _white = Driver.MakeAttribute(Color.White, Color.Black);
            _keywordColor = Driver.MakeAttribute(Color.Magenta, Color.Black);
            _stringColor = Driver.MakeAttribute(Color.Brown, Color.Black);
            _commentColor = Driver.MakeAttribute(Color.Green, Color.Black);

            var suggestions = CodeSuggestionsProvider.GetRainLispSuggestions();
            _keywords = new(suggestions);
            _linesAnalysis = new();

            Autocomplete.AllSuggestions = suggestions;
            Autocomplete.MaxWidth = suggestions.Max(s => s.Length);
        }

        public void ClearCodeAnalysisCache()
            => _linesAnalysis.Clear();

        protected override void SetNormalColor(List<Rune> line, int idx)
        {
            string lineText = new(line.Select(r => (char)r).ToArray());

            // If the line has not been analyzed or the line text has changed, perform the analysis and cache it.
            if (!_linesAnalysis.TryGetValue(line, out LineInfo lineInfo) || lineInfo.Text != lineText)
            {
                lineInfo = AnalyzeLine(lineText);
                _linesAnalysis[line] = lineInfo;
            }

            if (IsInComment(idx, lineInfo))
                Driver.SetAttribute(_commentColor);

            else if (IsInString(idx, lineInfo))
                Driver.SetAttribute(_stringColor);

            else if (IsInKeyword(idx, lineInfo))
                Driver.SetAttribute(_keywordColor);

            else
                Driver.SetAttribute(_white);
        }

        private static LineInfo AnalyzeLine(string lineText)
        {
            // A string starts with a " followed by any escaped character or (i.e. if not escaped) anything but a " or \ and terminated with a ".
            var strings = Regex.Matches(lineText, "\"(\\\\.|[^\"\\\\])*\"")
                                .Select(m => new StringInfo() { Start = m.Index, End = m.Index + m.Length - 1 })
                                .ToArray();

            // Search for the first comment start character outside of a string.
            var commentMatch = Regex.Matches(lineText, Delimiters.COMMENT.ToString())
                .Where(m => !strings.Any(si => si.Start <= m.Index && si.End >= m.Index))
                .FirstOrDefault();

            int startIndex = 0;
            string textForWordAnalysis = commentMatch?.Index > 0 ? lineText.Substring(0, commentMatch.Index) : lineText;

            // Identify words separated by white space characters or delimeters.
            var words = Regex.Split(textForWordAnalysis, $"[\\s{Delimiters.LPAREN}{Delimiters.RPAREN}{Delimiters.COMMENT}]")
                .Where(word => !string.IsNullOrEmpty(word))
                .Select(word =>
                {
                    int start = textForWordAnalysis.IndexOf(word, startIndex);
                    int end = start + word.Length - 1;
                    startIndex = end + 1;

                    return new WordInfo { Word = word, Start = start, End = end };
                }).ToArray();

            return new LineInfo() { Text = lineText, CommentStart = commentMatch?.Index ?? -1, Words = words, Strings = strings };
        }

        private static bool IsInString(int idx, LineInfo lineInfo)
        {
            foreach (var stringInfo in lineInfo.Strings)
            {
                if (idx >= stringInfo.Start && idx <= stringInfo.End)
                    return true;
            }

            return false;
        }

        private static bool IsInComment(int idx, LineInfo lineInfo)
            => lineInfo.CommentStart > -1 && idx >= lineInfo.CommentStart;

        private bool IsInKeyword(int idx, LineInfo lineInfo)
        {
            foreach (var wordInfo in lineInfo.Words)
            {
                if (idx >= wordInfo.Start && idx <= wordInfo.End)
                    return _keywords.Contains(wordInfo.Word);
            }

            return false;
        }
    }
}
