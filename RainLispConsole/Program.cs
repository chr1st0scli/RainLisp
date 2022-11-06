// See https://aka.ms/new-console-template for more information
using RainLisp;

Console.WriteLine("RainLisp Console");

var interpreter = new Interpreter();

interpreter.ReadEvalPrintLoop(Read, Print);


string? Read()
{
    Console.Write("> ");
    return Console.ReadLine();
}

void Print(string result)
{
    Console.WriteLine(result);
}