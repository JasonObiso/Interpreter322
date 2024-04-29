using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter322
{
    public static class Grammar
    {
        public static Token GetWordToken(string input, int line, int column)
        {
            Dictionary<string, TType> keywords = new Dictionary<string, TType>()
                {
                    {"BEGIN", TType.BEGIN}, {"END", TType.END}, {"CODE", TType.CODE}, {"IF", TType.IF},
                    {"ELSE", TType.ELSE}, {"WHILE", TType.WHILE}, {"DISPLAY", TType.DISPLAY}, {"SCAN", TType.SCAN},
                    {"AND", TType.AND}, {"OR", TType.OR}, {"NOT", TType.NOT}
                };

            Dictionary<string, TType> data_types = new Dictionary<string, TType>()
                {
                    {"INT", TType.INT}, {"FLOAT", TType.FLOAT}, {"CHAR", TType.CHAR}, {"BOOL", TType.BOOL}
                };

            if (keywords.ContainsKey(input.ToUpper()))
            {
                if (keywords.ContainsKey(input))
                    return new Token(keywords[input], input, null, line, column);
                else
                    return new Token(TType.ERROR, input, $"Invalid keyword '{input}' should be {input.ToUpper()}", line, column);
            }
            else if (data_types.ContainsKey(input.ToUpper()))
            {
                if (data_types.ContainsKey(input))
                    return new Token(data_types[input], input, null, line, column);
                else
                    return new Token(TType.ERROR, input, $"Invalid data type '{input}' should be {input.ToUpper()}", line, column);
            }
            else
                return new Token(TType.IDENTIFIER, input, null, line, column);
        }

        public static int GetBinaryPrecedence(TType token_type)
        {
            switch (token_type)
            {
                case TType.OR:
                    return 1;
                case TType.AND:
                    return 2;
                case TType.LESSTHAN:
                case TType.LESSEQUAL:
                case TType.GREATERTHAN:
                case TType.GREATEREQUAL:
                case TType.EQUALTO:
                case TType.NOTEQUAL:
                    return 4;
                case TType.PLUS:
                case TType.MINUS:
                    return 5;
                case TType.PERCENT:
                    return 6;
                case TType.STAR:
                case TType.SLASH:
                    return 7;
                default:
                    return 0;
            }
        }

        public static DType GetDType(Token token)
        {
            TType token_type = token.Token_Type;

            switch (token_type)
            {
                case TType.INT:
                case TType.INTLITERAL:
                    return DType.Int;
                case TType.FLOAT:
                case TType.FLOATLITERAL:
                    return DType.Float;
                case TType.CHAR:
                case TType.CHARLITERAL:
                    return DType.Char;
                case TType.BOOL:
                case TType.BOOLLITERAL:
                    return DType.Bool;
                default:
                    throw new Exception($"Unknown data type");
            }
        }

        public static DType GetDType(TType token_type)
        {
            switch (token_type)
            {
                case TType.INT:
                case TType.INTLITERAL:
                    return DType.Int;
                case TType.FLOAT:
                case TType.FLOATLITERAL:
                    return DType.Float;
                case TType.CHAR:
                case TType.CHARLITERAL:
                    return DType.Char;
                case TType.BOOL:
                case TType.BOOLLITERAL:
                    return DType.Bool;
                default:
                    throw new Exception($"Unknown data type");
            }
        }

        public static DType GetDType(object val)
        {
            if (val.GetType() == typeof(int))
                return DType.Int;
            else if (val.GetType() == typeof(float) || val.GetType() == typeof(double))
                return DType.Float;
            else if (val.GetType() == typeof(char))
                return DType.Char;
            else if (val.GetType() == typeof(bool))
                return DType.Bool;
            else
                return DType.String;
        }

        public static bool IsArithmeticOperator(TType token_type)
        {
            List<TType> arithmetic_operator = new List<TType>
                {
                    TType.PLUS, TType.MINUS, TType.STAR, TType.SLASH, TType.PERCENT
                };

            return arithmetic_operator.Contains(token_type);
        }

        public static bool IsComparisonOperator(TType token_type)
        {
            List<TType> logical_operator = new List<TType>
                {
                    TType.LESSTHAN, TType.GREATERTHAN,
                    TType.LESSEQUAL, TType.GREATEREQUAL,
                    TType.EQUALTO, TType.NOTEQUAL
                };

            return logical_operator.Contains(token_type);
        }

        public static object ConvertValue(string val)
        {
            //string float_pattern = @"^-?\d+(\.\d+)?$";
            string float_pattern = @"^(?:\+|\-)?\d*\.\d+$";
            string int_pattern = @"^(?:\+|\-)?\d+$";
            string char_pattern = @"^'(?:\[[\[\]\&\$\#']\])'|'[^\[\]\&\$\#']'$";
            string bool_pattern = @"^\""TRUE\""$|^\""FALSE\""$";

            Regex float_regex = new Regex(float_pattern);
            Regex int_regex = new Regex(int_pattern);
            Regex char_regex = new Regex(char_pattern);
            Regex bool_regex = new Regex(bool_pattern);

            if (int_regex.IsMatch(val))
                return Convert.ToInt32(val);
            else if (float_regex.IsMatch(val))
                return Convert.ToDouble(val);
            else if (char_regex.IsMatch(val))
                return val;
            else if (bool_regex.IsMatch(val))
                return val == "\"TRUE\"" ? true : false;
            else
                throw new Exception($"Runtime Error: Invalid input {val}.");
        }

        public static bool MatchDType(DType ldt, DType rdt)
        {
            if ((ldt == DType.Float && rdt == DType.Int))
                return true;
            return ldt == rdt;
        }
    }
}

