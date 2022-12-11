// See https://aka.ms/new-console-template for more information
using RainLisp;
using RainLispConsole;

Console.WriteLine(Resources.LOGO);
Console.Write(Resources.WELCOME_PROMPT);

int mode = -1;
do
{
    Console.WriteLine();
    Console.Write(Resources.ZERO_OR_ONE);
    mode = Console.ReadKey().Key switch
    {
        ConsoleKey.D0 => 0,
        ConsoleKey.D1 => 1,
        _ => -1
    };
} while (mode == -1);

if (mode == 0)
{
    Console.WriteLine();

    var interpreter = new Interpreter();
    interpreter.ReadEvalPrintLoop(Read, Print, PrintError);

    string? Read()
    {
        Console.Write(Resources.REPL_PROMPT);
        return Console.ReadLine();
    }

    void Print(string result)
    {
        Console.WriteLine(result);
    }

    void PrintError(string message, Exception ex)
    {
        Console.WriteLine(message);

        // Print the entire exception if it is unknown.
        if (message == ErrorMessages.UNKNOWN_ERROR)
            Console.WriteLine(ex.ToString());

        // Or print the exception's message if one provided by a programmer extending the library.
        else if (!string.IsNullOrWhiteSpace(ex.Message) && !ex.Message.StartsWith("Exception of type"))
            Console.WriteLine(ex.Message);
    }
}
else
{
    _ = new CodeEditor();
    CodeEditor.Run();
}
