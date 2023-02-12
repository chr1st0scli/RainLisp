using System.Text;
using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.Keywords;
using static RainLisp.Grammar.NumberSpecialChars;

namespace RainLisp.Tokenization
{
    public class Tokenizer : ITokenizer
    {
        public IList<Token> Tokenize(string? expression)
        {
            var tokens = new List<Token>();
            uint line = 1, charPosition = 1;

            if (string.IsNullOrEmpty(expression))
            {
                RegisterEOF();
                return tokens;
            }

            bool charInstring = false, charInComment = false, charInNumber = false;
            var lexemeStringBuilder = new StringBuilder();
            StringTokenizer? stringTokenizer = null;
            NumberTokenizer? numberTokenizer = null;

            #region Local Helper Methods.
            void RegisterToken(Token token)
                => tokens.Add(token);

            void RegisterSpecificToken(string value, TokenType tokenType, uint position, double numberValue = 0, bool booleanValue = false, string stringValue = "")
                => RegisterToken(new() { Value = value, Type = tokenType, Line = line, Position = position, NumberValue = numberValue, BooleanValue = booleanValue, StringValue = stringValue });

            void RegisterEOF()
                => RegisterSpecificToken(string.Empty, TokenType.EOF, charPosition);

            void RegisterStringLiteralToken()
            {
                charInstring = false;
                RegisterSpecificToken(stringTokenizer!.GetStringLiteral(), TokenType.String, GetLastStringStartPosition(), stringValue: stringTokenizer.GetStringValue());
                stringTokenizer.Clear();
            }

            void RegisterUnknownToken()
            {
                if (lexemeStringBuilder.Length == 0)
                    return;

                string value = lexemeStringBuilder.ToString();
                lexemeStringBuilder.Clear();

                if (string.IsNullOrWhiteSpace(value))
                    return;

                if (charInNumber)
                {
                    charInNumber = false;
                    RegisterSpecificToken(value, TokenType.Number, charPosition - (uint)value.Length, numberTokenizer!.GetNumber());
                    numberTokenizer.Clear();
                }
                else
                    RegisterToken(InferToken(value, line, charPosition - (uint)value.Length));
            }

            void ChangeLine()
            {
                line++;
                charPosition = 1;
            }

            void PlatformBounceNextNewLine(ref int i)
            {
                // Skip the next \n character, so that \r\n is treated as a single new line if the platform dictates it.
                if (i < expression.Length - 1 && expression[i + 1] == NEW_LINE && Environment.NewLine == $"{CARRIAGE_RETURN}{NEW_LINE}")
                    i++;
            }

            bool NumberStartsAt(int i)
            {
                // If a lexeme is already being built, we are not dealing with the start.
                if (lexemeStringBuilder.Length > 0)
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
            uint GetLastStringStartPosition()
                => charPosition - stringTokenizer!.CharactersProcessed - 1;
            #endregion

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (charInstring)
                    stringTokenizer!.AddToString(c, line, charPosition);

                // Disregard a character in a comment except a new line that ends it.
                else if (charInComment)
                {
                    if (c == CARRIAGE_RETURN)
                    {
                        charInComment = false;
                        ChangeLine();
                        PlatformBounceNextNewLine(ref i);
                    }
                    else if (c == NEW_LINE)
                    {
                        charInComment = false;
                        ChangeLine();
                    }
                    continue; // Skip advancing character position.
                }
                // Start of a comment.
                else if (c == COMMENT)
                {
                    RegisterUnknownToken();
                    charInComment = true;
                    continue; // Skip advancing character position.
                }
                // Start of a string.
                else if (c == DOUBLE_QUOTE)
                {
                    RegisterUnknownToken();
                    charInstring = true;
                    stringTokenizer ??= new StringTokenizer(RegisterStringLiteralToken);
                }
                else if (c == LPAREN)
                {
                    RegisterUnknownToken();
                    RegisterSpecificToken(c.ToString(), TokenType.LParen, charPosition);
                }
                else if (c == RPAREN)
                {
                    RegisterUnknownToken();
                    RegisterSpecificToken(c.ToString(), TokenType.RParen, charPosition);
                }
                else if (c == CARRIAGE_RETURN)
                {
                    RegisterUnknownToken();
                    ChangeLine();
                    PlatformBounceNextNewLine(ref i);
                    continue; // Skip advancing character position.
                }
                else if (c == NEW_LINE)
                {
                    RegisterUnknownToken();
                    ChangeLine();
                    continue; // Skip advancing character position.
                }
                else if (c == SPACE || c == TAB)
                    RegisterUnknownToken();

                else if (charInNumber)
                {
                    numberTokenizer!.AddToNumber(c, line, charPosition);
                    lexemeStringBuilder.Append(c);
                }
                else if (NumberStartsAt(i))
                {
                    charInNumber = true;
                    numberTokenizer ??= new NumberTokenizer();
                    numberTokenizer.AddToNumber(c, line, charPosition);
                    lexemeStringBuilder.Append(c);
                }
                else
                    lexemeStringBuilder.Append(c);

                charPosition++;
            }

            if (charInstring)
                throw new NonTerminatedStringException(line, GetLastStringStartPosition());

            RegisterUnknownToken();
            RegisterEOF();

            return tokens;
        }

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
