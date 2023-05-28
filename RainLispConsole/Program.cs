// See https://aka.ms/new-console-template for more information
using RainLisp;
using RainLispConsole;

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
        _ => -1
    };
} while (mode == -1);

if (mode == 2)
{
    _ = new CodeEditor();
    CodeEditor.Run();
}
else
{
    Console.WriteLine();

    Repl repl = mode switch
    {
        0 => new SingleLineRepl(),
        _ => new MultiLineRepl(),
    };

    var interpreter = new Interpreter();
    interpreter.ReadEvalPrintLoop(repl.Read, Repl.Print, Repl.PrintError);
}
