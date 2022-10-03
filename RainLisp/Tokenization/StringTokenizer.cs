using System.Text;
using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.StringEscapableChars;

namespace RainLisp.Tokenization
{
    public class StringTokenizer
    {
        private bool _escaping;
        private readonly StringBuilder _stringBuilder;
        private readonly Action _stringCompletedAction;

        public StringTokenizer(Action stringCompletedAction)
        {
            _stringCompletedAction = stringCompletedAction ?? throw new ArgumentNullException(nameof(stringCompletedAction));
            _stringBuilder = new StringBuilder();
            _escaping = false;
            CharactersProcessed = 0;
        }

        public void AddToString(char c)
        {
            CharactersProcessed++;

            if (_escaping)
            {
                if (c == ESCAPABLE_DOUBLE_QUOTE || c == ESCAPE)
                    _stringBuilder.Append(c);

                else if (c == ESCAPABLE_NEW_LINE)
                    _stringBuilder.Append(NEW_LINE);

                else if (c == ESCAPABLE_CARRIAGE_RETURN)
                    _stringBuilder.Append(CARRIAGE_RETURN);

                else if (c == ESCAPABLE_TAB)
                    _stringBuilder.Append(TAB);

                else
                    // TODO Unknown escape sequence exception.
                    _stringBuilder.Append(ESCAPE).Append(c);

                // Stop escaping, escaping applies to one character only.
                _escaping = false;
            }
            else
            {
                if (c == ESCAPE)
                    _escaping = true;

                // A not escaped double quote ends the string.
                else if (c == DOUBLE_QUOTE)
                    _stringCompletedAction();

                else
                    _stringBuilder.Append(c);
            }
        }

        public string GetString() => _stringBuilder.ToString();

        public uint CharactersProcessed { get; private set; }
    }
}
