using RainLisp;
using RainLisp.Evaluation;
using Terminal.Gui;

namespace RainLispConsole
{
    class RainLispIDE : Window
    {
        private readonly TextView _inputTextView;
        private readonly TextView _outputTextView;
        private readonly Interpreter _interpreter;

        public RainLispIDE()
        {
            _interpreter = new();

            Title = "Rainλisp";

            var textViewColorScheme = new ColorScheme()
            {
                Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            };

            _inputTextView = new()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = textViewColorScheme
            };

            _outputTextView = new()
            {
                ReadOnly = true,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = textViewColorScheme
            };

            var inputFrameView = new FrameView("Code Editor")
            {
                Y = 1,  // Leave a row for the menu.
                Width = Dim.Fill(),
                Height = Dim.Percent(70)
            };

            var outputFrameView = new FrameView("Output")
            {
                Y = Pos.Bottom(inputFrameView) + 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),   // Leave a row for the status bar.
            };

            StatusItem[] statusBarItems = new[]
            {
                new StatusItem(Key.CtrlMask | Key.Enter, "Ctrl-Enter Evaluate", Evaluate),
                new StatusItem(Key.CtrlMask | Key.q, "Ctrl-Q Quit", () => Application.RequestStop()),
            };
            var statusBar = new StatusBar(statusBarItems);

            MenuItem[] fileMenuItems = new[]
            {
                new MenuItem(Key.CtrlMask | Key.n) { Title = "New", Action = New },
                new MenuItem(Key.CtrlMask | Key.o) { Title = "Open", Action = Open },
                new MenuItem(Key.CtrlMask | Key.s) { Title = "Save", Action = Save }
            };

            MenuItem[] helpMenuItems = new[]
            {
                new MenuItem(Key.F1) { Title = "View Help", Action = ViewHelp },
                new MenuItem() { Title = "About", Action = About }
            };

            MenuBarItem[] menuBarItems = new[]
            {
                new MenuBarItem("File", fileMenuItems),
                new MenuBarItem("Help", helpMenuItems),
            };

            var menuBar = new MenuBar(menuBarItems);

            inputFrameView.Add(_inputTextView);
            outputFrameView.Add(_outputTextView);
            Add(menuBar, inputFrameView, outputFrameView, statusBar);
            MenuBar = menuBar;
        }

        private void New()
        {

        }

        private void Open()
        {

        }

        private void Save()
        {

        }

        private void ViewHelp()
        {

        }

        private void About()
        {
            MessageBox.Query("About", Resources.ABOUT, "OK");
        }

        private void Evaluate()
        {
            IEvaluationEnvironment? environment = null;
            _interpreter.EvaluateAndPrint(_inputTextView.Text.ToString(), ref environment, Print, PrintError);
        }

        private void Print(string result)
            => _outputTextView.Text = result;

        private void PrintError(string message, Exception ex)
        {
            _outputTextView.Text = message;

            // Print the entire exception if it is unknown.
            if (message == ErrorMessages.UNKNOWN_ERROR)
                _outputTextView.Text = ex.ToString();

            // Or print the exception's message if one provided by a programmer extending the library.
            else if (!string.IsNullOrWhiteSpace(ex.Message) && !ex.Message.StartsWith("Exception of type"))
                _outputTextView.Text = ex.Message;
        }
    }
}
