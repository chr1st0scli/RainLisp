using static RainLisp.Grammar.NumberSpecialChars;

namespace RainLisp.Tokenization
{
    /// <summary>
    /// Represents a number tokenizer capable of performing lexical analysis on numeric literals.
    /// </summary>
    public class NumberTokenizer
    {
        private bool _cleared;
        private double _number;
        private bool _inFraction;
        private bool _negative;
        private double _fractionDenominator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberTokenizer"/> class.
        /// </summary>
        public NumberTokenizer()
            => Clear();

        /// <summary>
        /// Clears the state of the number tokenizer so that it can start processing a new numeric literal.
        /// </summary>
        public void Clear()
        {
            _cleared = true;
            _number = 0;
            _inFraction = false;
            _negative = false;
            _fractionDenominator = 1;
        }

        /// <summary>
        /// Adds a character to the number tokenizer processing engine.
        /// It should be called with consecutive characters, once a number literal is detected to start with a sign or digit character, until a delimeter is reached.
        /// </summary>
        /// <param name="c">A character within a numeric literal.</param>
        /// <param name="line">The line in the code that <paramref name="c"/> is in.</param>
        /// <param name="position">The character position in the <paramref name="line"/> where <paramref name="c"/> is at.</param>
        /// <exception cref="InvalidNumberCharacterException">An invalid character for a numeric literal is provided.</exception>
        public void AddToNumber(char c, uint line = 0, uint position = 0)
        {
            if (_cleared)
            {
                _cleared = false;

                if (c == MINUS)
                {
                    _negative = true;
                    return;
                }
                else if (c == PLUS)
                    return;

                else if (!IsDigit(c))
                    throw new InvalidNumberCharacterException(line, position, c);
            }
            else
            {
                if (c == DOT)
                {
                    // A second decimal point is not a valid character.
                    if (_inFraction)
                        throw new InvalidNumberCharacterException(line, position, c);

                    _inFraction = true;
                    return;
                }
                else if (!IsDigit(c))
                    throw new InvalidNumberCharacterException(line, position, c);
            }

            if (_inFraction)
                _fractionDenominator *= 10;

            // The current digit is determined by its distance from the zero character.
            int currentDigit = c - '0';
            // It is faster to progressively accumulate the number like this than using double.Parse.
            _number = _number * 10 + currentDigit;
        }

        /// <summary>
        /// Returns the number value that results from processing the numeric literal.
        /// It should be called when a numeric literal is considered complete, e.g. when a delimeter is encountered.
        /// </summary>
        /// <returns>The number value that results from processing the numeric literal.</returns>
        public double GetNumber()
        {
            if (_negative)
                _number = -_number;

            if (_fractionDenominator == 1)
                return _number;

            return _number / _fractionDenominator;
        }

        private static bool IsDigit(char c)
            => c >= '0' && c <= '9';    // Do not use char.IsDigit.
    }
}
