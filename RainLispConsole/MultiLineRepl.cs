namespace RainLispConsole
{
    internal class MultiLineRepl : Repl
    {
        private const char CTRL_Z = '\u001a';
        
        internal override string? Read()
        {
            Console.WriteLine(Resources.REPL_MULTILINE_PROMPT);

            string input = Console.In.ReadToEnd();

            // Remove the Ctrl + Z in case the user typed it in next to other characters instead on its own in a separate line.
            return input.Trim().Trim(CTRL_Z);
        }
    }
}
