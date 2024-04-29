using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter322
{
    public enum DType // Data Types
    {
        Int,
        Float,
        Char,
        Bool,
        String
    }

    public enum TType //Token Types
    {
        BEGIN, END, CODE,
        IF, ELSE, WHILE,
        DISPLAY, SCAN, AND,
        OR, NOT,

        INT, FLOAT,
        CHAR, BOOL,

        IDENTIFIER,

        INTLITERAL, FLOATLITERAL,
        CHARLITERAL, BOOLLITERAL,
        STRINGLITERAL,

        COMMA, EQUAL, COLON,
        QUOTE, APOSTROPHE, POUND,
        DOLLAR, AMPERSAND,

        OPENPARENTHESIS, CLOSEPARENTHESIS,
        STAR, SLASH, PERCENT, PLUS, MINUS,

        GREATERTHAN, LESSTHAN, GREATEREQUAL,
        LESSEQUAL, EQUALTO, NOTEQUAL,

        NEWLINE, ESCAPE, ERROR, ENDOFFILE
    }
}
