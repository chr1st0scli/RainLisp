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
        private string? _recentDirectory;
        private byte[]? _originalWorkingFileBytes;

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

            var statusBarItems = new StatusItem[]
            {
                new(Key.CtrlMask | Key.Enter, Resources.EVALUATE, Evaluate),
                new(Key.CtrlMask | Key.F4, Resources.QUIT, Quit),
            };

            var statusBar = new StatusBar(statusBarItems);

            var fileMenuItems = new MenuItem[]
            {
                new() { Title = Resources.NEW, Action = New },
                new() { Title = Resources.OPEN, Action = Open },
                new() { Title = Resources.SAVE, Action = Save },
                new() { Title = Resources.SAVE_AS, Action = SaveAs }
            };

            var helpMenuItems = new MenuItem[]
            {
                new(Key.F1) { Title = Resources.VIEW_HELP, Action = ViewHelp },
                new() { Title = Resources.ABOUT, Action = About }
            };

            var menuBarItems = new MenuBarItem[]
            {
                new(Resources.FILE, fileMenuItems),
                new(Resources.HELP, helpMenuItems),
            };

            var menuBar = new MenuBar(menuBarItems);

            _inputFrameView.Add(_inputTextView);
            outputFrameView.Add(_outputTextView);
            Add(menuBar, _inputFrameView, outputFrameView, statusBar);
            MenuBar = menuBar;
            SetWorkingFile(null);
        }

        private bool ProceedAndLosePossibleChanges()
        {
            if (_inputTextView.Text == _originalWorkingFileBytes)
                return true;

            return MessageBox.Query(Resources.CONFIRMATION, Resources.LOSE_UNSAVED_CHANGES, Resources.NO, Resources.YES) == 1;
        }

        private void Quit()
        {
            if (ProceedAndLosePossibleChanges())
                Application.RequestStop();
        }

        private void New()
        {
            if (!ProceedAndLosePossibleChanges())
                return;

            _inputTextView.Text = "";
            SetWorkingFile(null); 
        }

        private void Open()
        {
            if (!ProceedAndLosePossibleChanges())
                return;

            var openDialog = new OpenDialog(Resources.OPEN, Resources.OPEN_FILE, _allowedFileTypes);

            if (_recentDirectory != null)
                openDialog.DirectoryPath = _recentDirectory;

            Application.Run(openDialog);

            if (openDialog.Canceled)
                return;

            _recentDirectory = openDialog.DirectoryPath.ToString();
            string? filePath = openDialog.FilePath.ToString();

            if (_inputTextView.LoadFile(filePath))
                SetWorkingFile(filePath);
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

            if (_recentDirectory != null)
                saveDialog.DirectoryPath = _recentDirectory;

            Application.Run(saveDialog);

            if (saveDialog.Canceled)
                return null;

            _recentDirectory = saveDialog.DirectoryPath.ToString();

            string? filePath = saveDialog.FilePath.ToString();

            if (File.Exists(filePath) && MessageBox.Query(Resources.CONFIRMATION, Resources.OVERWRITE_FILE, Resources.NO, Resources.YES) == 0)
                return null;

            return filePath;
        }

        private void SaveFile(string filePath)
        {
            File.WriteAllText(filePath, _inputTextView.Text.ToString());
            SetWorkingFile(filePath);
        }

        private void SetWorkingFile(string? filePath)
        {
            _filePath = filePath;
            _originalWorkingFileBytes = _inputTextView.Text.ToByteArray();
            _inputFrameView.Title = Resources.CODE_EDITOR + " - " + (Path.GetFileName(filePath) ?? Resources.UNTITLED);
        }

        private void ViewHelp()
            => MessageBox.Query(Resources.HELP, Resources.HELP_CONTENTS, Resources.OK);

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
