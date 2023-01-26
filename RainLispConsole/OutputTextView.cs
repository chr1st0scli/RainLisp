using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace RainLispConsole
{
    internal class OutputTextView : TextView
    {
        private readonly Attribute _white;
        private readonly Attribute _red;
        private readonly IList<string> _errorLines;
        private readonly HashSet<List<Rune>> _errorLinesCache;
        private readonly char[] _lineSeparators = { '\r', '\n' };

        public OutputTextView()
        {
            _white = Driver.MakeAttribute(Color.White, Color.Black);
            _red = Driver.MakeAttribute(Color.Red, Color.Black);
            ReadOnly = true;
            _errorLines = new List<string>();
            _errorLinesCache = new();
        }

        public void RegisterError(string? error)
        {
            if (string.IsNullOrEmpty(error))
                return;

            var errorLines = error.Split(_lineSeparators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string errorLine in errorLines)
                _errorLines.Add(errorLine);
        }

        public void ClearErrors()
        {
            _errorLines.Clear();
            _errorLinesCache.Clear();
        }

        protected override void SetReadOnlyColor(List<Rune> line, int idx)
        {
            // If the output contains an error message in red and the user uses the display
            // function to print the same line, the latter will also be printed in red!
            // Unfortunately, TextView does not allow for a better handling!

            bool isError = _errorLinesCache.Contains(line);
            // If the line is not registered in the cache as an error, register it in case it is.
            if (!isError)
            {
                string lineText = new(line.Select(r => (char)r.Value).ToArray());
                isError = _errorLines.Contains(lineText);

                if (isError)
                    _errorLinesCache.Add(line);
            }

            Driver.SetAttribute(isError ? _red : _white);
        }
    }
}
