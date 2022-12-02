// See https://aka.ms/new-console-template for more information
using RainLisp;
using RainLisp.Evaluation;
using RainLispConsole;
using Terminal.Gui;

Console.WriteLine(Resources.LOGO);

Application.Run<RainLispWindow>();
Application.Shutdown();

class RainLispWindow : Window
{
    private readonly TextView _inputTextView;
    private readonly TextView _outputTextView;
    private readonly StatusBar _statusBar;
    private readonly Interpreter _interpreter;
    private IEvaluationEnvironment? _environment;

    public RainLispWindow()
    {
        _interpreter = new();

        Title = "RainLisp";

        _inputTextView = new() 
        { 
            Width = Dim.Fill(), 
            Height = Dim.Percent(50) 
        };

        _outputTextView = new() 
        {
            ReadOnly = true,
            Y = Pos.Bottom(_inputTextView) + 1, 
            Width = Dim.Fill(), 
            Height = Dim.Height(_inputTextView) - 2 
        };

        StatusItem[] statusBarItems = new[]
        {
            new StatusItem(Key.CtrlMask | Key.Enter, "Ctrl-Enter Evaluate", Evaluate),
            new StatusItem(Key.ShiftMask | Key.F5, "Ctrl-R Reset", Reset),
            new StatusItem(Key.CtrlMask | Key.q, "Ctrl-Q Quit", () => Application.RequestStop()),
        };
        _statusBar = new StatusBar(statusBarItems);

        Add(_inputTextView, _outputTextView, _statusBar);
    }

    private void Evaluate()
    {
        string? expression = _inputTextView.Text.ToString();
        _inputTextView.Text = string.Empty;
        _interpreter.EvaluateAndPrint(expression, ref _environment, Print, PrintError);
    }

    private void Reset()
    {
        _environment = null;
        _outputTextView.Text = "Environment has been reset.";
    }

    private void Print(string result)
        => AppendToOutput(result);

    private void PrintError(string message, Exception ex)
    {
        AppendToOutput(message);

        // Print the entire exception if it is unknown.
        if (message == ErrorMessages.UNKNOWN_ERROR)
            AppendToOutput(ex.ToString());

        // Or print the exception's message if one provided by a programmer extending the library.
        else if (!string.IsNullOrWhiteSpace(ex.Message) && !ex.Message.StartsWith("Exception of type"))
            AppendToOutput(ex.Message);
    }

    private void AppendToOutput(string line)
        => _outputTextView.Text += Environment.NewLine + line;
}