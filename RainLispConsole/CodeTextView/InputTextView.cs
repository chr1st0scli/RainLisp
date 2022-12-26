using RainLisp.Grammar;
using System.Text.RegularExpressions;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace RainLispConsole.CodeTextView
{
    internal class InputTextView : TextView
    {
        private readonly Attribute _white;
        private readonly Attribute _green;
        private readonly Attribute _magenta;
        private readonly HashSet<string> _keywords;
        private readonly Dictionary<List<Rune>, LineInfo> _linesAnalysis;

        public InputTextView(List<string> suggestions)
        {
            _white = Driver.MakeAttribute(Color.White, Color.Black);
            _green = Driver.MakeAttribute(Color.Green, Color.Black);
            _magenta = Driver.MakeAttribute(Color.Magenta, Color.Black);
            _keywords = new(suggestions);
            _linesAnalysis = new();

            Autocomplete.AllSuggestions = suggestions;
        }

        protected override void SetNormalColor(List<Rune> line, int idx)
        {
            string lineText = new(line.Select(r => (char)r).ToArray());

            // If the line has not been analyzed or the line text has changed, perform the analysis and cache it.
            if (!_linesAnalysis.TryGetValue(line, out LineInfo lineInfo) || lineInfo.Text != lineText)
            {
                lineInfo = AnalyzeLine(lineText);
                _linesAnalysis[line] = lineInfo;
            }

            if (IsInString(idx, lineInfo))
                Driver.SetAttribute(_magenta);
            else if (IsInKeyword(idx, lineInfo))
                Driver.SetAttribute(_green);
            else
                Driver.SetAttribute(_white);
        }

        private static LineInfo AnalyzeLine(string lineText)
        {
            var strings = Regex.Matches(lineText, "\"(\\\\.|[^\"\\\\])*\"")
                                .Select(m => new StringInfo() { Start = m.Index, End = m.Index + m.Length - 1 }).ToArray();

            int startIndex = 0;

            var words = Regex.Split(lineText, $"[\\s{Delimiters.LPAREN}{Delimiters.RPAREN}]")
                .Select(word =>
                {
                    int start = lineText.IndexOf(word, startIndex);
                    int end = start + word.Length - 1;
                    startIndex = end + 1;

                    return new WordInfo { Word = word, Start = lineText.IndexOf(word), End = start + word.Length - 1 };
                }).ToArray();

            var lineInfo = new LineInfo() { Text = lineText, Words = words, Strings = strings };

            return lineInfo;
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
