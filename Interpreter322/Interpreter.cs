using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter322
{
    public class Interpreter
    {
        private Variables variables;
        private ProgramNode program;

        public Interpreter(string code)
        {
            Lexer lex = new Lexer(code);
            Parser parser = new Parser(lex);
            Semantic semantic = new Semantic();

            program = parser.ParseProgram();
            semantic.Analyze(program);
            variables = new Variables();
        }

        public void Execute(ProgramNode statement_block = null)
        {
            ProgramNode prog = statement_block == null ? program : statement_block;

            foreach (StatementNode statement in prog.Statements)
            {
                switch (statement)
                {
                    case VariableDeclarationNode var_stmt:
                        ExecuteVariableDeclaration(var_stmt);
                        break;
                    case AssignmentNode assign_stmt:
                        ExecuteAssignment(assign_stmt);
                        break;
                    case DisplayNode display_stmt:
                        ExecuteDisplay(display_stmt);
                        break;
                    case ScanNode scan_stmt:
                        ExecuteScan(scan_stmt);
                        break;
                    case ConditionalNode cond_stmt:
                        ExecuteCondition(cond_stmt);
                        break;
                    case LoopNode loop_stmt:
                        ExecuteLoop(loop_stmt);
                        break;
                }
            }
        }

        // Start Execute Statements
        private void ExecuteVariableDeclaration(VariableDeclarationNode statement)
        {
            // Loop through the dictionary of variables
            foreach (var variable in statement.Variables)
            {
                // Get the variable name
                string identifier = variable.Key;

                // Set default value to null
                object value = null;

                // If the variable value is not null ex. INT a = 5
                if (variable.Value != null)
                    value = EvaluateExpression(variable.Value);

                // Add variable to table of variables
                variables.AddVariable(identifier, Grammar.GetDType(statement.Data_Type_Token.Token_Type), value);
            }
        }

        private void ExecuteAssignment(AssignmentNode statement)
        {
            // Set default value to null
            object value = null;

            // Loop through a list of identifiers
            foreach (string identifier in statement.Identifiers)
            {
                // Get the value of the expression
                value = EvaluateExpression(statement.Expression);

                // Add variable to table of variables
                variables.AddValue(identifier, value);
            }
        }

        private void ExecuteDisplay(DisplayNode statement)
        {
            // Set display result to an empty string
            string result = "";

            // Loop through the list of expression
            foreach (var expression in statement.Expressions)
                result += EvaluateExpression(expression);

            // Display result
            Console.Write(result);
        }

        private void ExecuteScan(ScanNode statement)
        {
            // List of values
            List<string> values = new List<string>();
            // List of identifiers
            List<string> identifiers = statement.Identifiers;
            // Set input to an empty string
            string inputted = "";

            // Read inputs
            Console.Write("");
            inputted = Console.ReadLine();

            // Split the input using ',' as delimiter
            values = inputted.Replace(" ", "").Split(',').ToList();

            // Runtime error: if the length of values and identifiers
            if (values.Count != identifiers.Count)
                throw new Exception($"Runtime Error: Missing input/s.");

            // Set default value to null
            object value = null;

            int index = 0;
            foreach (var val in values)
            {
                // Convert the string to specific data type
                value = Grammar.ConvertValue(val);

                // If the data type of the identifier and expression is not same
                if (!Grammar.MatchDType(variables.GetType(identifiers[index]), Grammar.GetDType(value)))
                    throw new Exception($"Runtime Error: Unable to assign {Grammar.GetDType(value)} on \"{identifiers[index]}\".");

                // Add variable to table of variables
                variables.AddValue(identifiers[index], value);

                index++;
            }
        }

        private void ExecuteCondition(ConditionalNode statement)
        {
            // Set displayed to false as default
            bool displayed = false;
            // Set index to 0 as default
            int index = 0;

            // Loop through the list of expressions
            foreach (var expression in statement.Expressions)
            {
                // If expression is not null -> if, else if
                if (expression != null)
                {
                    // Evaluate the expression and cast to bool
                    // If true then set display to true and
                    // Execute the statement block corresponding to
                    // the statement/expression
                    if ((bool)EvaluateExpression(expression))
                    {
                        displayed = true;
                        Execute(statement.Statements[index]);
                        break;
                    }
                }
                // If expression is null break out of the loop
                else
                    break;

                index++;
            }

            // Check if expression is null -> null is else
            if (statement.Expressions[index] == null)
                // If displayed is false then execute the statement block for
                // else statement
                if (!displayed)
                    Execute(statement.Statements[index]);
        }

        private void ExecuteLoop(LoopNode statement)
        {
            // Evaluate the expression and cast to bool
            // If true then do a while loop
            while ((bool)EvaluateExpression(statement.Expression))
                Execute(statement.Statement);
        }
        // End Execute Statements

        // Get the value of the expression
        private object EvaluateExpression(ExpressionNode expression)
        {
            switch (expression)
            {
                case BinaryNode binary_expr:
                    return EvaluateBinaryExpression(binary_expr);

                case UnaryNode unary_expr:
                    return EvaluateUnaryExpression(unary_expr);

                case ParenthesisNode parenthesis_expr:
                    return EvaluateExpression(parenthesis_expr.Expression);

                case IdentifierNode identifier_expr:
                    return EvaluateIdentifierExpression(identifier_expr);

                case LiteralNode literal_expr:
                    return literal_expr.Literal;

                default:
                    throw new Exception("Unknown expression.");
            }
        }

        private object EvaluateBinaryExpression(BinaryNode expression)
        {
            dynamic left = EvaluateExpression(expression.Left);
            dynamic right = EvaluateExpression(expression.Right);
            dynamic bin_result;

            switch (expression.Token_Operator.Token_Type)
            {
                case TType.PLUS:
                    bin_result = left + right;
                    return bin_result;
                case TType.MINUS:
                    bin_result = left - right;
                    return bin_result;
                case TType.STAR:
                    bin_result = left * right;
                    return bin_result;
                case TType.SLASH:
                    bin_result = left / right;
                    return bin_result;
                case TType.PERCENT:
                    bin_result = left % right;
                    return bin_result;
                case TType.LESSTHAN:
                    bin_result = left < right;
                    return bin_result;
                case TType.GREATERTHAN:
                    bin_result = left > right;
                    return bin_result;
                case TType.LESSEQUAL:
                    bin_result = left <= right;
                    return bin_result;
                case TType.GREATEREQUAL:
                    bin_result = left >= right;
                    return bin_result;
                case TType.EQUALTO:
                    bin_result = left == right;
                    return bin_result;
                case TType.NOTEQUAL:
                    bin_result = left != right;
                    return bin_result;
                case TType.AND:
                    bin_result = left && right;
                    return bin_result;
                case TType.OR:
                    bin_result = left || right;
                    return bin_result;
                default:
                    throw new Exception($"Unknown operator.");
            }
        }

        private object EvaluateUnaryExpression(UnaryNode expression)
        {
            dynamic unary_value = EvaluateExpression(expression.Expression);
            if (expression.Token_Operator.Token_Type == TType.MINUS)
                return -unary_value;
            else if (expression.Token_Operator.Token_Type == TType.NOT)
                return !(unary_value.Contains("TRUE") ? true : false);
            else
                return unary_value;
        }

        private object EvaluateIdentifierExpression(IdentifierNode expression)
        {
            if (variables.GetValue(expression.Name) == null)
                throw new Exception($"({expression.Identifier_Token.Line},{expression.Identifier_Token.Column}): Variable '{expression.Name}' is null.");

            object result = variables.GetValue(expression.Name);

            if (result.GetType() == typeof(bool))
                return (bool)result ? "TRUE" : "FALSE";
            return result;
        }
    }
}
