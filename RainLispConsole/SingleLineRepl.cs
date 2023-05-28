namespace RainLispConsole
{
    internal class SingleLineRepl : Repl
    {
        internal override string? Read()
        {
            Console.Write(Resources.REPL_PROMPT);
            return Console.ReadLine();
        }
    }
}
