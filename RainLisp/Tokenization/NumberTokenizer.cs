using static RainLisp.Grammar.NumberSpecialChars;

namespace RainLisp.Tokenization
{
    public class NumberTokenizer
    {
        private double _number;
        private bool _inFraction;
        private bool _start;
        private bool _negative;
        private int _fractionDenominator;

        public NumberTokenizer()
        {
            _inFraction = false;
            _start = true;
            _negative = false;
            _fractionDenominator = 1;
        }

        public void AddToNumber(char c, uint line = 0, uint position = 0)
        {
            if (_start)
            {
                _start = false;

                if (c == DOT)
                {
                    _inFraction = true;
                    return;
                }
                else if (c == MINUS)
                {
                    _negative = true;
                    return;
                }
                else if (c == PLUS)
                    return;

                else if (c != PLUS && !char.IsDigit(c))
                    throw new InvalidOperationException($"Invalid character {c}.");
            }
            else
            {
                if (c == DOT)
                {
                    if (_inFraction)
                        throw new InvalidOperationException($"Invalid character {c}.");
                    _inFraction = true;
                    return;
                }
                else if (!char.IsDigit(c))
                    throw new InvalidOperationException($"Invalid character {c}.");
            }

            if (_inFraction)
                _fractionDenominator *= 10;

            // The current digit is determined by its distance from the zero character.
            int currentDigit = c - '0';
            _number = _number * 10 + currentDigit;
        }

        public double GetNumber()
        {
            if (_negative)
                _number = -_number;

            if (_fractionDenominator == 1)
                return _number;

            return _number / _fractionDenominator;
        }
    }
}
