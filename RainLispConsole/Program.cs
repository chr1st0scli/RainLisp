// See https://aka.ms/new-console-template for more information
using RainLisp;

Console.WriteLine("RainLisp Console");

var interpreter = new Interpreter();

interpreter.ReadEvalPrintLoop(Read, Print, PrintError);


string? Read()
{
    Console.Write("> ");
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