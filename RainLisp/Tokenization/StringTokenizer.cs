using System.Text;
using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.StringEscapableChars;

namespace RainLisp.Tokenization
{
    public class StringTokenizer
    {
        private bool _escaping;
        private readonly StringBuilder _valueStringBuilder;
        private readonly StringBuilder _literalStringBuilder;
        private readonly Action _stringCompletedAction;

        public StringTokenizer(Action stringCompletedAction)
        {
            _stringCompletedAction = stringCompletedAction ?? throw new ArgumentNullException(nameof(stringCompletedAction));
            _valueStringBuilder = new StringBuilder();
            _literalStringBuilder = new StringBuilder();
            _escaping = false;
            CharactersProcessed = 0;
        }

        public void Clear()
        {
            _valueStringBuilder.Clear();
            _literalStringBuilder.Clear();
            _escaping = false;
            CharactersProcessed = 0;
        }

        public void AddToString(char c, uint line, uint position)
        {
            _literalStringBuilder.Append(c);

            if (_escaping)
            {
                if (c == ESCAPABLE_DOUBLE_QUOTE || c == ESCAPE)
                    _valueStringBuilder.Append(c);

                else if (c == ESCAPABLE_NEW_LINE)
                    _valueStringBuilder.Append(NEW_LINE);

                else if (c == ESCAPABLE_CARRIAGE_RETURN)
                    _valueStringBuilder.Append(CARRIAGE_RETURN);

                else if (c == ESCAPABLE_TAB)
                    _valueStringBuilder.Append(TAB);

                else
                    throw new InvalidEscapeSequenceException(line, position, c);

                // Stop escaping, escaping applies to one character only.
                _escaping = false;
            }
            else
            {
                if (c == ESCAPE)
                    _escaping = true;

                // A not escaped double quote ends the string.
                else if (c == DOUBLE_QUOTE)
                {
                    _stringCompletedAction();
                    return; // Do not count this character as a processed one.
                }

                // Multiline string literals are not supported.
                else if (c == CARRIAGE_RETURN || c == NEW_LINE)
                    throw new InvalidStringCharacterException(line, position, c);

                else
                    _valueStringBuilder.Append(c);
            }

            CharactersProcessed++;
        }

        public string GetStringValue() => _valueStringBuilder.ToString();

        public string GetStringLiteral() => _literalStringBuilder.Insert(0, DOUBLE_QUOTE).ToString();

        public uint CharactersProcessed { get; private set; }
    }
}
