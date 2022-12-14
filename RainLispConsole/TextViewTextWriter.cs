using System.Text;
using Terminal.Gui;

namespace RainLispConsole
{
    internal class TextViewTextWriter : TextWriter
    {
        private readonly TextView _textView;

        public TextViewTextWriter(TextView textView)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
        }

        public override Encoding Encoding => Console.OutputEncoding;

        public override void Write(string? value)
        {
            _textView.Text += value;
        }

        public override void WriteLine()
        {
            _textView.Text += Environment.NewLine;
        }

        public override void WriteLine(string? value)
        {
            _textView.Text += value + Environment.NewLine;
        }
    }
}
