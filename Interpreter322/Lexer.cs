using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter322
{
    public class Lexer
    {
        private readonly string _code;
        private int _position;
        private int _line, _column;

        public Lexer(string code)
        {
            this._code = code;
            this._position = 0;
            this._line = 1;
            this._column = 1;
        }

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _code.Length)
                return '\0';
            return _code[index];
        }

        private void Next(int offset = 1)
        {
            _position += offset;
            _column += offset;
        }

        private void NewLine()
        {
            _line++;
            _column = 1;
            Next();
        }

        public Token GetToken()
        {
            while (_position < _code.Length)
            {
                if (char.IsLetter(Current))
                    return GetKeywordOrDTypeOrIdentifierToken();

                if (char.IsDigit(Current))
                    return GetNumberLiteralToken();

                switch (Current)
                {
                    // WhiteSpace
                    case ' ':
                    case '\t':
                        Next();
                        continue;
                    case '\n':
                        Token new_line = new Token(TType.NEWLINE, "\n", null, _line, _column);
                        NewLine();
                        return new_line;
                    // Identifier
                    case '_':
                        return GetKeywordOrDTypeOrIdentifierToken();
                    // Literals
                    case '\'':
                        return GetCharacterLiteralToken();
                    case '\"':
                        return GetBooleanOrStringLiteralToken();
                    case '.':
                        return GetNumberLiteralToken();
                    // Comment
                    case '#':
                        while (Current != '\n' && Current != '\0')
                            Next();
                        continue;
                    // Arithmetic Operators
                    case '*':
                        Next();
                        return new Token(TType.STAR, "*", null, _line, _column - 1);
                    case '/':
                        Next();
                        return new Token(TType.SLASH, "/", null, _line, _column - 1);
                    case '%':
                        Next();
                        return new Token(TType.PERCENT, "%", null, _line, _column - 1);
                    case '+':
                        Next();
                        return new Token(TType.PLUS, "+", null, _line, _column - 1);
                    case '-':
                        Next();
                        return new Token(TType.MINUS, "-", null, _line, _column - 1);
                    // Logical Operators
                    case '>':
                        if (LookAhead == '=')
                        {
                            Next(2);
                            return new Token(TType.GREATEREQUAL, ">=", null, _line, _column - 2);
                        }
                        Next();
                        return new Token(TType.GREATERTHAN, ">", null, _line, _column - 1);
                    case '<':
                        if (LookAhead == '=')
                        {
                            Next(2);
                            return new Token(TType.LESSEQUAL, "<=", null, _line, _column - 2);
                        }
                        else if (LookAhead == '>')
                        {
                            Next(2);
                            return new Token(TType.NOTEQUAL, "<>", null, _line, _column - 2);
                        }
                        Next();
                        return new Token(TType.LESSTHAN, "<", null, _line, _column - 1);
                    case '=':
                        if (LookAhead == '=')
                        {
                            Next(2);
                            return new Token(TType.EQUALTO, "==", null, _line, _column - 2);
                        }
                        Next();
                        return new Token(TType.EQUAL, "=", null, _line, _column - 1);
                    // Symbols
                    case '$':
                        Next();
                        return new Token(TType.DOLLAR, "$", null, _line, _column - 1);
                    case '&':
                        Next();
                        return new Token(TType.AMPERSAND, "&", null, _line, _column - 1);
                    case '[':
                        return GetEscapeCodeToken();
                    case '(':
                        Next();
                        return new Token(TType.OPENPARENTHESIS, "(", null, _line, _column - 1);
                    case ')':
                        Next();
                        return new Token(TType.CLOSEPARENTHESIS, ")", null, _line, _column - 1);
                    case ',':
                        Next();
                        return new Token(TType.COMMA, ",", null, _line, _column - 1);
                    case ':':
                        Next();
                        return new Token(TType.COLON, ":", null, _line, _column - 1);
                    default:
                        Next();
                        return new Token(TType.ERROR, Current.ToString(), "Unknown symbol", _line, _column - 1);
                }
            }
            return new Token(TType.ENDOFFILE, "\0", null, _line, _column);
        }

        private Token GetKeywordOrDTypeOrIdentifierToken()
        {
            int start = _position;
            int line_col = _column;

            while (char.IsLetter(Current) || Current == '_' || char.IsDigit(Current))
                Next();

            int length = _position - start;
            string text = _code.Substring(start, length);

            return Grammar.GetWordToken(text, _line, line_col);
        }

        private Token GetCharacterLiteralToken()
        {
            int start = _position;
            int line_col = _column;

            Next();
            // ' '
            while (Current != '\'' && !char.IsWhiteSpace(LookAhead))
                Next();
            Next();

            string char_pattern = @"^'(?:\[[\[\]\&\$\#']\])'|'[^\[\]\&\$\#']'$";
            Regex char_regex = new Regex(char_pattern);

            int length = _position - start;
            string text = _code.Substring(start, length);
            object value = null;

            if (char_regex.IsMatch(text))
            {
                value = text.ToCharArray()[text.Length / 2];
                return new Token(TType.CHARLITERAL, text, value, _line, line_col);
            }
            return new Token(TType.ERROR, text, "Invalid CHAR literal.", _line, line_col);
        }

        private Token GetBooleanOrStringLiteralToken()
        {
            int start = _position;
            int line_col = _column;

            Next();
            // " "
            while (Current != '\"' && !char.IsWhiteSpace(LookAhead))
                Next();
            Next();

            string bool_pattern = @"^\""TRUE\""$|^\""FALSE\""$";
            string string_pattern = @"^""[^""]*""$";
            Regex bool_regex = new Regex(bool_pattern);
            Regex string_regex = new Regex(string_pattern);

            int length = _position - start;
            string text = _code.Substring(start, length);

            Debug.WriteLine(text);

            if (bool_regex.IsMatch(text))
                return new Token(TType.BOOLLITERAL, text, text == "\"TRUE\"" ? true : false, _line, line_col);
            else if (string_regex.IsMatch(text))
                return new Token(TType.STRINGLITERAL, text, text.Substring(1, text.Length - 2), _line, line_col);
            else
            {
                string error_message = text.Contains("TRUE") || text.Contains("FALSE") ? "Invalid BOOL literal" : "Invalid STRING literal";
                return new Token(TType.ERROR, text, error_message, _line, line_col);
            }
        }

        private Token GetNumberLiteralToken()
        {
            bool is_float = Current == '.' ? true : false;

            int start = _position;
            int line_col = _column;

            while (char.IsDigit(Current) || Current == '.')
                Next();

            int length = _position - start;
            string text = _code.Substring(start, length);

            //^\d+(\.\d+)?$|^(\d+)?\.\d+?$
            string float_pattern = @"^\d*\.\d+$";
            string int_pattern = @"^\d+$";
            Regex float_regex = new Regex(float_pattern);
            Regex int_regex = new Regex(int_pattern);

            object val = null;

            if (int_regex.IsMatch(text))
            {
                val = int.Parse(text);
                return new Token(TType.INTLITERAL, text, val, _line, line_col);
            }
            else if (float_regex.IsMatch(text))
            {
                val = float.Parse(text);
                return new Token(TType.FLOATLITERAL, text, val, _line, line_col);
            }
            return new Token(TType.ERROR, text, "Invalid Number.", _line, line_col);
        }

        private Token GetEscapeCodeToken()
        {
            int start = _position;
            int line_col = _column;

            while (!char.IsWhiteSpace(Current))
                Next();

            int length = _position - start;
            string text = _code.Substring(start, length);
            object val = null;

            string escape_sequence_pattern = @"^\[[\]\[\&\$\#]\]$";
            Regex escape_regex = new Regex(escape_sequence_pattern);

            if (escape_regex.IsMatch(text))
            {
                val = text.ToCharArray()[1];
                return new Token(TType.ESCAPE, text, val, _line, line_col);
            }
            return new Token(TType.ERROR, text, $"Invalid '{text}' as escape sequence.", _line, line_col);
        }
    }
}
