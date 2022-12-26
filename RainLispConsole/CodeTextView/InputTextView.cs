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
        private readonly HashSet<string> _keywords;
        private readonly Dictionary<List<Rune>, LineInfo?> _linesAnalysis;

        public InputTextView(List<string> suggestions)
        {
            _white = Driver.MakeAttribute(Color.White, Color.Black);
            _green = Driver.MakeAttribute(Color.Green, Color.Black);
            _keywords = new(suggestions);
            _linesAnalysis = new();

            Autocomplete.AllSuggestions = suggestions;
        }

        protected override void SetNormalColor(List<System.Rune> line, int idx)
        {
            string lineText = new(line.Select(r => (char)r).ToArray());

            _linesAnalysis.TryGetValue(line, out LineInfo? lineInfo);
            // If the line has not been analyzed or the line text has changed, perform the analysis and cache it.
            if (lineInfo == null || lineInfo.Value.Text != lineText)
            {
                int startIndex = 0;
                
                var words = Regex.Split(lineText, $"[\\s{Delimiters.LPAREN}{Delimiters.RPAREN}]")
                    .Select(word =>
                    {
                        int start = lineText.IndexOf(word, startIndex);
                        int end = start + word.Length - 1;
                        startIndex = end + 1;

                        return new WordInfo { Word = word, Start = lineText.IndexOf(word), End = start + word.Length - 1 };
                    }).ToArray();

                lineInfo = new() { Text = lineText, Words = words };
                _linesAnalysis[line] = lineInfo;
            }

            // Identify if the current index is on a word that could be a keyword.
            string? candidateKeyword = null;
            foreach (var wordInfo in lineInfo.Value.Words)
            {
                if (idx >= wordInfo.Start && idx <= wordInfo.End)
                {
                    candidateKeyword = wordInfo.Word;
                    break;
                }
            }

            if (candidateKeyword != null && _keywords.Contains(candidateKeyword))
                Driver.SetAttribute(_green);
            else
                Driver.SetAttribute(_white);
        }
    }
}
