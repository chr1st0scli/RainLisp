using System.Text;

namespace RainLispConsole
{
    internal class MultiLineRepl : Repl
    {
        private bool _isFirstLine;

        internal override string? Read()
        {
            Console.Write(Resources.REPL_PROMPT);

            var sb = new StringBuilder();

            bool terminate = false;
            do
            {
                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();

                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        terminate = true;
                    else
                        sb.Append(keyInfo.KeyChar);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft > 0)
                    {
                        Console.CursorLeft--;
                        Console.Write(' ');
                        Console.CursorLeft--;
                    }

                    if (sb.Length > 0)
                        sb.Remove(sb.Length - 1, 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    sb.Append(keyInfo.KeyChar);
                    Console.Write(keyInfo.KeyChar);
                }
            } while (!terminate);

            return sb.ToString();
        }
    }
}
