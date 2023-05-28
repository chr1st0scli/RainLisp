namespace RainLispConsole
{
    internal abstract class Repl
    {
        internal abstract string? Read();

        internal static void Print(string result)
        {
            if (!string.IsNullOrEmpty(result))
                Console.WriteLine(result);
        }

        internal static void PrintError(string message, Exception ex, bool unknownError)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(message);

            // Print the entire exception if it is unknown.
            if (unknownError)
                Console.WriteLine(ex.ToString());

            // Or print the exception's message if one provided by a programmer extending the library.
            else if (!string.IsNullOrWhiteSpace(ex.Message) && !ex.Message.StartsWith("Exception of type"))
                Console.WriteLine(ex.Message);

            Console.ForegroundColor = originalColor;
        }
    }
}
