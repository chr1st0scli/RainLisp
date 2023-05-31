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

var interpreter = new Interpreter();

if (mode == 2)
{
    _ = new CodeEditor(interpreter);
    CodeEditor.Run();
}
else
{
    Console.WriteLine();
    interpreter.ReadEvalPrintLoop(mode == 0 ? Repl.ReadLine : Repl.ReadLines, Repl.Print, Repl.PrintError);
}
