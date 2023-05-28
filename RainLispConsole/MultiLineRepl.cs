using System.Text;

namespace RainLispConsole
{
    internal class MultiLineRepl : Repl
    {
        private readonly List<string> _lines;

        public MultiLineRepl()
        {
            _lines = new List<string>();
        }

        internal override string? Read()
        {
            Console.WriteLine(Resources.REPL_MULTILINE_PROMPT);
            var lineBuilder = new StringBuilder();
            _lines.Add("");

            int currentLine = 0;
            bool terminate = false;
            do
            {
                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    _lines[currentLine] = lineBuilder.ToString();
                    lineBuilder.Clear();

                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        terminate = true;
                    else
                    {
                        currentLine++;
                        if (currentLine == _lines.Count)
                            _lines.Add("");
                        else
                            _lines.Insert(currentLine, "");
                        Console.WriteLine();
                    }
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (Console.CursorLeft > 0)
                        Console.CursorLeft--;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (Console.CursorLeft < lineBuilder.Length)
                        Console.CursorLeft++;
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (currentLine > 0)
                    {
                        _lines[currentLine] = lineBuilder.ToString();
                        lineBuilder.Clear();

                        Console.CursorTop--;
                        currentLine--;

                        lineBuilder.Append(_lines[currentLine]);

                        if (Console.CursorLeft > lineBuilder.Length)
                            Console.CursorLeft = lineBuilder.Length;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (currentLine < _lines.Count - 1)
                    {
                        _lines[currentLine] = lineBuilder.ToString();
                        lineBuilder.Clear();

                        Console.CursorTop++;
                        currentLine++;

                        lineBuilder.Append(_lines[currentLine]);

                        if (Console.CursorLeft > lineBuilder.Length)
                            Console.CursorLeft = lineBuilder.Length;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft > 0 && lineBuilder.Length > 0)
                    {
                        int cursorPos = Console.CursorLeft;
                        Console.CursorLeft--;
                        if (cursorPos < lineBuilder.Length)
                            Console.Write(lineBuilder.ToString(cursorPos, lineBuilder.Length - cursorPos));
                        Console.Write(' ');
                        Console.CursorLeft = cursorPos - 1;
                        lineBuilder.Remove(cursorPos - 1, 1);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Delete)
                {
                    if (Console.CursorLeft >= 0 && Console.CursorLeft < lineBuilder.Length)
                    {
                        int cursorPos = Console.CursorLeft;
                        if (cursorPos + 1 < lineBuilder.Length)
                            Console.Write(lineBuilder.ToString(cursorPos + 1, lineBuilder.Length - cursorPos - 1));
                        Console.Write(' ');
                        Console.CursorLeft = cursorPos;
                        lineBuilder.Remove(cursorPos, 1);
                    }
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    if (Console.CursorLeft == lineBuilder.Length)
                    {
                        lineBuilder.Append(keyInfo.KeyChar);
                        Console.Write(keyInfo.KeyChar);
                    }
                    else
                    {
                        int cursorPos = Console.CursorLeft;
                        lineBuilder.Insert(cursorPos, keyInfo.KeyChar);
                        Console.Write(lineBuilder.ToString(cursorPos, lineBuilder.Length - cursorPos));
                        Console.CursorLeft = cursorPos + 1;
                    }
                }
            } while (!terminate);

            if (currentLine < _lines.Count - 1)
                Console.CursorTop += _lines.Count - currentLine - 1;

            if (Console.CursorLeft < _lines[^1].Length)
                Console.CursorLeft = _lines[^1].Length;

            Console.WriteLine();

            var sb = new StringBuilder().AppendJoin(Environment.NewLine, _lines);
            _lines.Clear();

            return sb.ToString();
        }
    }
}
