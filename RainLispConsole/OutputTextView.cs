using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace RainLispConsole
{
    internal class OutputTextView : TextView
    {
        private readonly Attribute _white;
        private readonly Attribute _red;

        public OutputTextView()
        {
            _white = Driver.MakeAttribute(Color.White, Color.Black);
            _red = Driver.MakeAttribute(Color.Red, Color.Black);
            ReadOnly = true;
        }

        public bool ErrorMode { get; set; }

        protected override void SetReadOnlyColor(List<Rune> line, int idx)
        {
            Driver.SetAttribute(ErrorMode ? _red : _white);
        }
    }
}
