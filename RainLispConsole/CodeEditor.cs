using RainLisp;
using RainLisp.Evaluation;
using RainLispConsole.CodeTextView;
using Terminal.Gui;

namespace RainLispConsole
{
    class CodeEditor
    {
        private readonly Window _mainWindow;
        private readonly InputTextView _inputTextView;
        private readonly FrameView _inputFrameView;
        private readonly OutputTextView _outputTextView;
        private readonly StatusItem _cursorPosStatusItem;
        private readonly StatusBar _statusBar;

        private readonly IInterpreter _interpreter;
        private readonly List<string> _allowedFileTypes;

        private string? _filePath;
        private string? _recentDirectory;
        private byte[]? _originalWorkingFileBytes;

        public CodeEditor(IInterpreter interpreter)
        {
            _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));

            _allowedFileTypes = new List<string> { Resources.FILE_EXT };

            Application.Init();

            _mainWindow = new() { Title = Resources.TITLE };

            var textViewColorScheme = new ColorScheme()
            {
                Focus = new(Color.White, Color.Black),
            };

            _inputTextView = new()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = textViewColorScheme,
                DesiredCursorVisibility = CursorVisibility.Box,
            };
            _inputTextView.UnwrappedCursorPosition += InputTextViewCursorPositionChanged;
            _inputTextView.Autocomplete.ColorScheme = new()
            {
                Focus = new(Color.Cyan, Color.Gray),
                Normal = new(Color.White, Color.DarkGray)
            };

            _outputTextView = new()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = textViewColorScheme
            };

            _inputFrameView = new FrameView(Resources.CODE_EDITOR)
            {
                Width = Dim.Fill(),
                Height = Dim.Percent(70)
            };

            var outputFrameView = new FrameView(Resources.OUTPUT)
            {
                Y = Pos.Bottom(_inputFrameView),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            _cursorPosStatusItem = new(Key.Null, "", null);
            var statusBarItems = new StatusItem[]
            {
                _cursorPosStatusItem,
                new(Key.CtrlMask | Key.Enter, Resources.EVALUATE, Evaluate),
                new(Key.CtrlMask | Key.F4, Resources.QUIT, Quit),
            };

            _statusBar = new StatusBar(statusBarItems);

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
            _mainWindow.Add(_inputFrameView, outputFrameView);
            _mainWindow.MenuBar = menuBar;
            SetWorkingFile(null);

            Application.Top.Add(menuBar, _mainWindow, _statusBar);

            var inputTextScrollbar = new TextViewScrollbar(_inputTextView);
            var outputTextScrollbar = new TextViewScrollbar(_outputTextView);

            // Redirect the standard output and error to the same output text view.
            var outputWriter = new OutputTextViewWriter(_outputTextView, false);
            var errorWriter = new OutputTextViewWriter(_outputTextView, true);
            Console.SetOut(outputWriter);
            Console.SetError(errorWriter);
        }

        public static void Run()
        {
            Application.Run();
            Application.Shutdown();
        }

        private void InputTextViewCursorPositionChanged(Point point)
        {
            _cursorPosStatusItem.Title = string.Format(Resources.CURSOR_POS_FORMAT, point.Y + 1, point.X + 1);
            _statusBar.SetNeedsDisplay();
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

            _inputTextView.ClearCodeAnalysisCache();
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
            else
                openDialog.DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Application.Run(openDialog);

            if (openDialog.Canceled)
                return;

            _recentDirectory = openDialog.DirectoryPath.ToString();
            string filePath = openDialog.FilePath.ToString()!;

            OpenFile(filePath);
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
            else
                saveDialog.DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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
            => SetWorkingFile(filePath, () => File.WriteAllText(filePath, _inputTextView.Text.ToString()));

        private void OpenFile(string filePath)
            => SetWorkingFile(filePath, () => 
            {
                _inputTextView.ClearCodeAnalysisCache();
                _inputTextView.Text = File.ReadAllText(filePath);
            });

        private void SetWorkingFile(string filePath, Action operation)
        {
            try
            {
                operation();
                SetWorkingFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery(Resources.ERROR, ex.Message, Resources.OK);
            }
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
            _outputTextView.Text = "";
            _outputTextView.ClearErrors();
            IEvaluationEnvironment? environment = null;
            _interpreter.EvaluateAndPrint(_inputTextView.Text.ToString(), ref environment, Print, PrintError);
        }

        private void Print(string result)
            => Console.WriteLine(result);

        private void PrintError(string message, Exception ex, bool unknownError)
        {
            Console.Error.WriteLine(message);

            // Print the entire exception if it is unknown.
            if (unknownError)
                Console.Error.WriteLine(ex.ToString());

            // Or print the exception's message if one provided by a programmer extending the library.
            else if (!string.IsNullOrWhiteSpace(ex.Message) && !ex.Message.StartsWith("Exception of type"))
                Console.Error.WriteLine(ex.Message);
        }
    }
}
