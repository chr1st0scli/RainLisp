using System.Text;
using Terminal.Gui;

namespace RainLispConsole
{
    internal class OutputTextViewWriter : TextWriter
    {
        private readonly OutputTextView _textView;
        private readonly bool _errorMode;

        private const string COMPONENT_STDOUT_BEACON = "\u001b[3J";

        public OutputTextViewWriter(OutputTextView textView, bool errorMode)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _errorMode = errorMode;
        }

        public override Encoding Encoding => Console.OutputEncoding;

        public override void Write(string? value)
        {
            // Terminal.Gui writes this string to the standard output for some reason, whenever the window resizes.
            // Prevent it from being displayed.
            if (value == COMPONENT_STDOUT_BEACON)
                return;

            if (_errorMode)
                _textView.RegisterError(value);

            _textView.Text += value;
            ScrollToBottom();
        }

        public override void WriteLine()
        {
            _textView.Text += Environment.NewLine;
            ScrollToBottom();
        }

        public override void WriteLine(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (_errorMode)
                _textView.RegisterError(value);

            _textView.Text += value + Environment.NewLine;
            ScrollToBottom();
        }

        private void ScrollToBottom() => _textView.CursorPosition = new Point(0, _textView.Lines - 1);
    }
}
