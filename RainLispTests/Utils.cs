namespace RainLispTests
{
    internal static class Utils
    {
        internal static uint PickLine(uint winOS, uint otherOS)
            => Environment.NewLine == "\r\n" ? winOS : otherOS;

        internal static uint PickLine(uint winOS)
            => PickLine(winOS, winOS * 2 - 1);

        internal static string ReadAllTextOnAnyPlatform(string path)
            => File.ReadAllText(path).Replace("\r\n", Environment.NewLine);
    }
}
