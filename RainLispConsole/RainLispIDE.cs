﻿using RainLisp;
using RainLisp.Evaluation;
using Terminal.Gui;

namespace RainLispConsole
{
    class RainLispIDE : Window
    {
        private readonly TextView _inputTextView;
        private readonly FrameView _inputFrameView;
        private readonly TextView _outputTextView;
        private readonly Interpreter _interpreter;
        private readonly List<string> _allowedFileTypes;

        private string? _filePath;
        private bool _saved;

        public RainLispIDE()
        {
            _interpreter = new();
            _allowedFileTypes = new List<string> { Resources.FILE_EXT };

            Title = Resources.TITLE;

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

            _inputFrameView = new FrameView(Resources.CODE_EDITOR)
            {
                Y = 1,  // Leave a row for the menu.
                Width = Dim.Fill(),
                Height = Dim.Percent(70)
            };

            var outputFrameView = new FrameView(Resources.OUTPUT)
            {
                Y = Pos.Bottom(_inputFrameView) + 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),   // Leave a row for the status bar.
            };

            StatusItem[] statusBarItems = new[]
            {
                new StatusItem(Key.CtrlMask | Key.Enter, Resources.EVALUATE, Evaluate),
                new StatusItem(Key.CtrlMask | Key.q, Resources.QUIT, () => Application.RequestStop()),
            };

            var statusBar = new StatusBar(statusBarItems);

            MenuItem[] fileMenuItems = new[]
            {
                new MenuItem(Key.CtrlMask | Key.n) { Title = Resources.NEW, Action = New },
                new MenuItem(Key.CtrlMask | Key.o) { Title = Resources.OPEN, Action = Open },
                new MenuItem(Key.CtrlMask | Key.s) { Title = Resources.SAVE, Action = Save },
                new MenuItem(Key.CtrlMask | Key.s) { Title = Resources.SAVE_AS, Action = SaveAs }
            };

            MenuItem[] helpMenuItems = new[]
            {
                new MenuItem(Key.F1) { Title = Resources.VIEW_HELP, Action = ViewHelp },
                new MenuItem() { Title = Resources.ABOUT, Action = About }
            };

            MenuBarItem[] menuBarItems = new[]
            {
                new MenuBarItem(Resources.FILE, fileMenuItems),
                new MenuBarItem(Resources.HELP, helpMenuItems),
            };

            var menuBar = new MenuBar(menuBarItems);

            _inputFrameView.Add(_inputTextView);
            outputFrameView.Add(_outputTextView);
            Add(menuBar, _inputFrameView, outputFrameView, statusBar);
            MenuBar = menuBar;
            SetOpenedFile(null);
        }

        private void New()
        {
            _inputTextView.Text = "";
            SetOpenedFile(null);
        }

        private void Open()
        {
            var openDialog = new OpenDialog(Resources.OPEN, Resources.OPEN_FILE, _allowedFileTypes);
            Application.Run(openDialog);

            if (openDialog.Canceled)
                return;

            string? filePath = openDialog.FilePath.ToString();
            _inputTextView.Text = File.ReadAllText(filePath!);
            SetOpenedFile(filePath);
        }

        private void Save()
        {
            string? filePath = _filePath ?? OpenSaveDialog();
            if (filePath == null)
                return;

            SaveFile(filePath);
        }

        private void SaveAs()
        {
            string? filePath = OpenSaveDialog();
            if (filePath == null)
                return;

            SaveFile(filePath);
        }

        private string? OpenSaveDialog()
        {
            var saveDialog = new SaveDialog(Resources.SAVE, Resources.SAVE_FILE, _allowedFileTypes);
            Application.Run(saveDialog);

            if (saveDialog.Canceled)
                return null;

            return saveDialog.FilePath.ToString();
        }

        private void SaveFile(string filePath)
        {
            File.WriteAllText(filePath, _inputTextView.Text.ToString());
            SetOpenedFile(filePath);
        }

        private void SetOpenedFile(string? filePath)
        {
            _filePath = filePath;
            _saved = filePath != null ? true : false;
            _inputFrameView.Title = Resources.CODE_EDITOR + " - " + (Path.GetFileName(filePath) ?? Resources.ASTERISK);
        }

        private void ViewHelp()
        {

        }

        private void About()
            => MessageBox.Query(Resources.ABOUT, Resources.INFO, Resources.OK);

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