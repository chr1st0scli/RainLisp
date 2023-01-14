﻿using static RainLisp.Grammar.NumberSpecialChars;

namespace RainLisp.Tokenization
{
    public class NumberTokenizer
    {
        private bool _cleared;
        private double _number;
        private bool _inFraction;
        private bool _negative;
        private double _fractionDenominator;

        public NumberTokenizer() => Clear();

        public void Clear()
        {
            _cleared = true;
            _number = 0;
            _inFraction = false;
            _negative = false;
            _fractionDenominator = 1;
        }

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

        private static bool IsDigit(char c)
            => c >= '0' && c <= '9';    // Do not use char.IsDigit.

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