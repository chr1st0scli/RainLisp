using System.Text;
using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.StringEscapableChars;

namespace RainLisp.Tokenization
{
    /// <summary>
    /// Represents a string tokenizer capable of performing lexical analysis on string literals.
    /// </summary>
    public class StringTokenizer
    {
        private bool _escaping;
        private readonly StringBuilder _valueStringBuilder;
        private readonly StringBuilder _literalStringBuilder;
        private readonly Action _stringCompletedAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTokenizer"/> class.
        /// </summary>
        /// <param name="stringCompletedAction">An action to be called when the current string being built is complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="stringCompletedAction"/> is null.</exception>
        public StringTokenizer(Action stringCompletedAction)
        {
            _stringCompletedAction = stringCompletedAction ?? throw new ArgumentNullException(nameof(stringCompletedAction));
            _valueStringBuilder = new StringBuilder();
            _literalStringBuilder = new StringBuilder();
            _escaping = false;
            CharactersProcessed = 0;
        }

        /// <summary>
        /// Clears the state of the string tokenizer so that it can start processing a new string literal.
        /// </summary>
        public void Clear()
        {
            _valueStringBuilder.Clear();
            _literalStringBuilder.Clear();
            _escaping = false;
            CharactersProcessed = 0;
        }

        /// <summary>
        /// Adds a character to the string tokenizer processing engine.
        /// It should be called with consecutive characters, once a string literal is detected to start with an opening double quote.
        /// The opening double quote should not be fed to this method.
        /// </summary>
        /// <param name="c">A character within a string literal.</param>
        /// <param name="line">The line in the code that <paramref name="c"/> is in.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where <paramref name="c"/> is at.</param>
        /// <exception cref="InvalidEscapeSequenceException">An invalid string literal escape sequence is provided.</exception>
        /// <exception cref="InvalidStringCharacterException">An invalid string literal character is provided. Multiline string literals are not supported.</exception>
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

        /// <summary>
        /// Returns the string value that results from processing the string literal.
        /// Escape sequences are properly replaced by the respective characters.
        /// It should be called when notified by the callback provided in the constructor.
        /// </summary>
        /// <returns>The string value that results from processing the string literal.</returns>
        public string GetStringValue()
            => _valueStringBuilder.ToString();

        /// <summary>
        /// Returns the original string literal fed to the string tokenizer, including the surrounding double quotes.
        /// Escape sequences are not processed and are returned as is.
        /// </summary>
        /// <returns>The original string literal fed to the string tokenizer, including the surrounding double quotes.</returns>
        public string GetStringLiteral()
            => _literalStringBuilder.Insert(0, DOUBLE_QUOTE).ToString();

        /// <summary>
        /// Gets or sets the total number of processed characters excluding the double quotes surrounding the string literal.
        /// </summary>
        public uint CharactersProcessed { get; private set; }
    }
}
