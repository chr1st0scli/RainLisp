using System.Text;

namespace RainLispConsole
{
    internal class OutputTextViewWriter : TextWriter
    {
        private readonly OutputTextView _textView;
        private readonly bool _errorMode;

        public OutputTextViewWriter(OutputTextView textView, bool errorMode)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _errorMode = errorMode;
        }

        public override Encoding Encoding => Console.OutputEncoding;

        public override void Write(string? value)
        {
            _textView.ErrorMode = _errorMode;
            _textView.Text += value;
        }

        public override void WriteLine()
        {
            _textView.Text += Environment.NewLine;
        }

        public override void WriteLine(string? value)
        {
            _textView.ErrorMode = _errorMode;
            _textView.Text += value + Environment.NewLine;
        }
    }
}
