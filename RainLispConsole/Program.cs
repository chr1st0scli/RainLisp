// See https://aka.ms/new-console-template for more information
using RainLisp;
using RainLisp.Evaluation;
using RainLispConsole;

var interpreter = new Interpreter();

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
            interpreter.EvaluateAndPrint(code, ref env, Repl.Print, Repl.PrintError);
        }
        catch (Exception ex)
        {
            // A file related exception might occur. Interpreter exceptions are already handled by EvaluateAndPrint.
            Repl.PrintError(ex.Message, ex, true);
        }
    }

    return;
}

Console.WriteLine(Resources.LOGO);
Console.Write(Resources.WELCOME_MESSAGE);

int mode = -1;
do
{
    Console.WriteLine();
    Console.Write(Resources.MODE_PROMPT);
    mode = Console.ReadKey().Key switch
    {
        ConsoleKey.D0 or ConsoleKey.NumPad0 => 0,
        ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
        ConsoleKey.D2 or ConsoleKey.NumPad1 => 2,
        ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
        _ => -1
    };
} while (mode == -1);


if (mode == 3)
{
    Console.WriteLine();
    Console.WriteLine(Resources.COMMAND_LINE_HELP);
}
else if (mode == 2)
{
    _ = new CodeEditor(interpreter);
    CodeEditor.Run();
}
else
{
    Console.WriteLine();
    interpreter.ReadEvalPrintLoop(mode == 0 ? Repl.ReadLine : Repl.ReadLines, Repl.Print, Repl.PrintError);
}
