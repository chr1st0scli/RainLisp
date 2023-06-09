// See https://aka.ms/new-console-template for more information
using RainLisp;
using RainLisp.Evaluation;
using RainLispConsole;

// By convention, a 0 process exit code represents a success, whereas any other is a failure.
int exitCode = 0;
var interpreter = new Interpreter();

#region Command line arguments.
void PrintErrorAndSetExitCode(string message, Exception ex, bool unknownError)
{
    exitCode = 1;
    Repl.PrintError(message, ex, unknownError);
}

// If command line arguments are specified, the process evaluates code and exits.
if (args.Length > 0)
{
    if (args.Length != 2 || (args[0] != Resources.CODE_OPTION && args[0] != Resources.FILE_OPTION))
        Console.WriteLine(Resources.COMMAND_LINE_HELP);

    else
    {
        try
        {
            string code = args[0] == Resources.CODE_OPTION ? args[1] : File.ReadAllText(args[1]);
            IEvaluationEnvironment? env = null;
            interpreter.EvaluateAndPrint(code, ref env, Repl.Print, PrintErrorAndSetExitCode);
        }
        catch (Exception ex)
        {
            // A file related exception might occur. Interpreter exceptions are already handled by EvaluateAndPrint.
            PrintErrorAndSetExitCode(ex.Message, ex, true);
        }
    }

    return exitCode;
}
#endregion

#region REPL or editor mode.
Console.WriteLine(Resources.LOGO);
Console.Write(Resources.WELCOME_MESSAGE);

// If no command line arguments are specified, the user enters a REPL or editor mode.
Mode mode = Mode.None;
do
{
    Console.WriteLine();
    Console.Write(Resources.MODE_PROMPT);
    mode = Console.ReadKey().Key switch
    {
        ConsoleKey.D0 or ConsoleKey.NumPad0 => Mode.SingleLineRepl,
        ConsoleKey.D1 or ConsoleKey.NumPad1 => Mode.MultiLineRepl,
        ConsoleKey.D2 or ConsoleKey.NumPad1 => Mode.Editor,
        ConsoleKey.D3 or ConsoleKey.NumPad3 => Mode.Help,
        _ => Mode.None
    };
} while (mode == Mode.None);

if (mode == Mode.Help)
{
    Console.WriteLine();
    Console.WriteLine(Resources.COMMAND_LINE_HELP);
}
else if (mode == Mode.Editor)
{
    _ = new CodeEditor(interpreter);
    CodeEditor.Run();
}
else
{
    Console.WriteLine();
    // REPL is an infinite loop.
    interpreter.ReadEvalPrintLoop(mode == Mode.SingleLineRepl ? Repl.ReadLine : Repl.ReadLines, Repl.Print, Repl.PrintError);
}

return exitCode;

enum Mode { None = -1, SingleLineRepl, MultiLineRepl, Editor, Help }
#endregion