using System.Text;
using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.Keywords;
using static RainLisp.Grammar.NumberSpecialChars;

namespace RainLisp.Tokenization
{
    /// <summary>
    /// Represents a tokenizer capable of performing lexical analysis on the code.
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        private uint _line;
        private uint _charPosition;
        private bool _charInstring;
        private bool _charInNumber;
        private bool _charInComment;

        private readonly StringTokenizer _stringTokenizer;
        private readonly NumberTokenizer _numberTokenizer;
        private readonly StringBuilder _lexemeStringBuilder;
        private readonly IList<Token> _tokens;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        public Tokenizer()
        {
            _stringTokenizer = new StringTokenizer(RegisterStringLiteralToken);
            _numberTokenizer = new NumberTokenizer();
            _lexemeStringBuilder = new StringBuilder();
            _tokens = new List<Token>();
        }

        /// <summary>
        /// Performs lexical analysis on code and produces tokens as a result.
        /// </summary>
        /// <param name="expression">The code to lexically analyze.</param>
        /// <returns>A list of tokens.</returns>
        /// <exception cref="NonTerminatedStringException">A string literal is not properly terminated.</exception>
        /// <exception cref="InvalidEscapeSequenceException">An invalid string literal escape sequence is provided.</exception>
        /// <exception cref="InvalidStringCharacterException">An invalid string literal character is provided.</exception>
        /// <exception cref="InvalidNumberCharacterException">An invalid character for a numeric literal is provided.</exception>
        public IList<Token> Tokenize(string? expression)
        {
            _line = _charPosition = 1;
            _tokens.Clear();

            if (string.IsNullOrEmpty(expression))
            {
                RegisterEOF();
                return _tokens;
            }

            _charInstring = _charInNumber = _charInComment = false;
            _stringTokenizer.Clear();
            _numberTokenizer.Clear();
            _lexemeStringBuilder.Clear();

            for (int i = 0; i < expression.Length; i++)
                ProcessCharacter(expression[i], ref i, expression);

            if (_charInstring)
                throw new NonTerminatedStringException(_line, GetLastStringStartPosition());

            RegisterUnknownToken();
            RegisterEOF();

            return _tokens;
        }

        private void ProcessCharacter(char c, ref int i, string expression)
        {
            if (_charInstring)
                _stringTokenizer.AddToString(c, _line, _charPosition);

            else if (_charInComment)
            {
                ProcessCommentCharacter(c, ref i, expression);
                return; // Skip advancing character position.
            }
            // Start of a comment.
            else if (c == COMMENT)
            {
                RegisterUnknownToken();
                _charInComment = true;
                return; // Skip advancing character position.
            }
            // Start of a string.
            else if (c == DOUBLE_QUOTE)
            {
                RegisterUnknownToken();
                _charInstring = true;
                _stringTokenizer.Clear();
            }
            else if (c == LPAREN)
            {
                RegisterUnknownToken();
                RegisterSpecificToken(c.ToString(), TokenType.LParen, _charPosition);
            }
            else if (c == RPAREN)
            {
                RegisterUnknownToken();
                RegisterSpecificToken(c.ToString(), TokenType.RParen, _charPosition);
            }
            else if (c == SINGLE_QUOTE)
            {
                RegisterUnknownToken();
                RegisterSpecificToken(c.ToString(), TokenType.QuoteAlt, _charPosition);
            }
            else if (c == CARRIAGE_RETURN)
            {
                RegisterUnknownToken();
                ChangeLine();
                PlatformBounceNextNewLine(ref i, expression);
                return; // Skip advancing character position.
            }
            else if (c == NEW_LINE)
            {
                RegisterUnknownToken();
                ChangeLine();
                return; // Skip advancing character position.
            }
            else if (c == SPACE || c == TAB)
                RegisterUnknownToken();

            else if (_charInNumber)
            {
                _numberTokenizer.AddToNumber(c, _line, _charPosition);
                _lexemeStringBuilder.Append(c);
            }
            else if (NumberStartsAt(i, expression))
            {
                _charInNumber = true;
                _numberTokenizer.Clear();
                _numberTokenizer.AddToNumber(c, _line, _charPosition);
                _lexemeStringBuilder.Append(c);
            }
            else
                _lexemeStringBuilder.Append(c);

            _charPosition++;
        }

        private void ProcessCommentCharacter(char c, ref int i, string expression)
        {
            // Disregard a character in a comment except a new line that ends it.
            if (c == CARRIAGE_RETURN)
            {
                _charInComment = false;
                ChangeLine();
                PlatformBounceNextNewLine(ref i, expression);
            }
            else if (c == NEW_LINE)
            {
                _charInComment = false;
                ChangeLine();
            }
        }

        private void RegisterToken(Token token)
            => _tokens.Add(token);

        private void RegisterSpecificToken(string value, TokenType tokenType, uint position, double numberValue = 0, bool booleanValue = false, string stringValue = "")
            => RegisterToken(new() { Value = value, Type = tokenType, Line = _line, Position = position, NumberValue = numberValue, BooleanValue = booleanValue, StringValue = stringValue });

        private void RegisterEOF()
            => RegisterSpecificToken(string.Empty, TokenType.EOF, _charPosition);

        private void RegisterStringLiteralToken()
        {
            _charInstring = false;
            RegisterSpecificToken(_stringTokenizer.GetStringLiteral(), TokenType.String, GetLastStringStartPosition(), stringValue: _stringTokenizer.GetStringValue());
            _stringTokenizer.Clear();
        }

        private void RegisterUnknownToken()
        {
            if (_lexemeStringBuilder.Length == 0)
                return;

            string value = _lexemeStringBuilder.ToString();
            _lexemeStringBuilder.Clear();

            if (_charInNumber)
            {
                _charInNumber = false;
                RegisterSpecificToken(value, TokenType.Number, _charPosition - (uint)value.Length, _numberTokenizer.GetNumber());
                _numberTokenizer.Clear();
            }
            else
                RegisterToken(InferToken(value, _line, _charPosition - (uint)value.Length));
        }

        private void ChangeLine()
        {
            _line++;
            _charPosition = 1;
        }

        private static void PlatformBounceNextNewLine(ref int i, string expression)
        {
            // Skip the next \n character, so that \r\n is treated as a single new line if the platform dictates it.
            if (i < expression.Length - 1 && expression[i + 1] == NEW_LINE && Environment.NewLine == $"{CARRIAGE_RETURN}{NEW_LINE}")
                i++;
        }

        private bool NumberStartsAt(int i, string expression)
        {
            // If a lexeme is already being built, we are not dealing with the start of a number.
            if (_lexemeStringBuilder.Length > 0)
                return false;

            char c = expression[i];

            // If it starts with a digit, then a number is being started.
            if (char.IsDigit(c))
                return true;

            // If it starts with a number sign followed by a digit, then a number is considered to being started.
            if ((c == PLUS || c == MINUS) && i < expression.Length - 1 && char.IsDigit(expression[i + 1]))
                return true;

            return false;
        }

        // The string token's position relates to the actual number of characters typed in the string and not the resulting string value's length.
        private uint GetLastStringStartPosition()
            => _charPosition - _stringTokenizer.CharactersProcessed - 1;

        private static Token InferToken(string value, uint line, uint position)
        {
            TokenType tokenType;
            bool booleanValue = false;

            if (value == TRUE)
            {
                tokenType = TokenType.Boolean;
                booleanValue = true;
            }
            else if (value == FALSE)
            {
                tokenType = TokenType.Boolean;
                booleanValue = false;
            }
            else
                tokenType = value.ToTokenType();

            return new() { Value = value, Type = tokenType, Line = line, Position = position, BooleanValue = booleanValue };
        }
    }
}
