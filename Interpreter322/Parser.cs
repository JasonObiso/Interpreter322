using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter322
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _current_token;
        private List<string> _variable_names;
        private bool _can_declare;

        public Parser(Lexer lexer)
        {
            this._lexer = lexer;
            this._current_token = lexer.GetToken();
            this._variable_names = new List<string>();
            this._can_declare = true;
        }

        // Parse the whole code
        public ProgramNode ParseProgram(TType token_type = TType.CODE)
        {
            while (MatchToken(TType.NEWLINE))
                ConsumeToken(TType.NEWLINE);

            ConsumeToken(TType.BEGIN);
            ConsumeToken(token_type);

            while (MatchToken(TType.NEWLINE))
                ConsumeToken(TType.NEWLINE);

            List<StatementNode> statements = ParseStatements();

            while (MatchToken(TType.NEWLINE))
                ConsumeToken(TType.NEWLINE);

            ConsumeToken(TType.END);
            ConsumeToken(token_type);

            while (MatchToken(TType.NEWLINE))
                ConsumeToken(TType.NEWLINE);

            if (token_type == TType.CODE)
                ConsumeToken(TType.ENDOFFILE);

            return new ProgramNode(statements);
        }

        // Start Parse Statements
        private List<StatementNode> ParseStatements()
        {
            List<StatementNode> statement_list = new List<StatementNode>();

            while (!MatchToken(TType.END))
            {
                if (MatchToken(TType.INT) || MatchToken(TType.FLOAT) ||
                    MatchToken(TType.CHAR) || MatchToken(TType.BOOL))
                {
                    if (_can_declare)
                        statement_list.Add(ParseVariableDeclarationStatement());
                    else
                        throw new Exception($"({_current_token.Line},{_current_token.Column}): Invalid syntax.");
                }
                else if (MatchToken(TType.IDENTIFIER))
                {
                    _can_declare = false;
                    statement_list.Add(ParseAssignmentStatement());
                }
                else if (MatchToken(TType.DISPLAY))
                {
                    _can_declare = false;
                    statement_list.Add(ParseDisplayStatement());
                }
                else if (MatchToken(TType.SCAN))
                {
                    _can_declare = false;
                    statement_list.Add(ParseScanStatement());
                }
                else if (MatchToken(TType.IF))
                {
                    _can_declare = false;
                    statement_list.Add(ParseIfStatement());
                }
                else if (MatchToken(TType.WHILE))
                {
                    _can_declare = false;
                    statement_list.Add(ParseWhileStatement());
                }

                else if (MatchToken(TType.ENDOFFILE))
                    throw new Exception($"({_current_token.Line},{_current_token.Column}): Missing End Statement.");

                else
                    throw new Exception($"({_current_token.Line},{_current_token.Column}): Invalid syntax \"{_current_token.Code}\".");

                while (MatchToken(TType.NEWLINE))
                    ConsumeToken(TType.NEWLINE);
            }

            return statement_list;
        }

        private StatementNode ParseVariableDeclarationStatement()
        {
            // ex. INT a,b=5,c
            // Read and remove the current token
            Token data_type_token = _current_token;
            ConsumeToken(data_type_token.Token_Type);

            // Dictionary of variables with the value
            Dictionary<string, ExpressionNode> variables = new Dictionary<string, ExpressionNode>();

            // Getting the variable name and value, value can be null
            // GetVariable function is declared last in this class
            (string, ExpressionNode) variable = GetVariable();

            variables.Add(variable.Item1, variable.Item2);
            _variable_names.Add(variable.Item1);

            // While token is comma then call GetVariable again until
            // there's no comma token.
            while (MatchToken(TType.COMMA))
            {
                ConsumeToken(TType.COMMA);
                variable = GetVariable();
                variables.Add(variable.Item1, variable.Item2);
                _variable_names.Add(variable.Item1);
            }

            // Create the Variable Declaration Statement
            return new VariableDeclarationNode(data_type_token, variables);
        }

        private StatementNode ParseAssignmentStatement()
        {
            // ex. x=y=4
            // List of identifiers and its equals
            List<string> identifiers = new List<string>();
            List<Token> equals = new List<Token>();

            // Read and remove the current token
            Token identifier_token = _current_token;
            ConsumeToken(TType.IDENTIFIER);
            Token equal_token = _current_token;
            ConsumeToken(TType.EQUAL);

            identifiers.Add(identifier_token.Code);
            equals.Add(equal_token);

            ExpressionNode expression_value = ParseExpression();

            while (MatchToken(TType.EQUAL))
            {
                // Read and remove the current token
                IdentifierNode iden_expr = (IdentifierNode)expression_value;
                equal_token = _current_token;
                ConsumeToken(TType.EQUAL);

                identifiers.Add(iden_expr.Name);
                equals.Add(equal_token);

                // Read the literal or expression
                expression_value = ParseExpression();
            }

            // Create the Assignment Statement
            return new AssignmentNode(identifiers, equals, expression_value);
        }

        private StatementNode ParseDisplayStatement()
        {
            // Read and remove the current token
            Token display_token = _current_token;
            ConsumeToken(TType.DISPLAY);
            ConsumeToken(TType.COLON);

            // List of expressions
            List<ExpressionNode> expressions = new List<ExpressionNode>();

            // If display starts with '$'
            // ex. DISPLAY: $ ....
            if (MatchToken(TType.DOLLAR))
            {
                expressions.Add(new LiteralNode(_current_token, "\n"));
                ConsumeToken(TType.DOLLAR);

                // While token is & iterate until no & token.
                while (MatchToken(TType.AMPERSAND))
                {
                    ConsumeToken(TType.AMPERSAND);

                    // If newline is next to & throw an error
                    if (MatchToken(TType.NEWLINE))
                        throw new Exception($"({_current_token.Line},{_current_token.Column}): Unexpected {_current_token.Token_Type} token expected expression token");

                    // If $ is next to &, create a new Literal Expression with
                    // the value \n
                    if (MatchToken(TType.DOLLAR))
                    {
                        expressions.Add(new LiteralNode(_current_token, "\n"));
                        ConsumeToken(TType.DOLLAR);
                    }
                    // Else get the expression
                    else
                        expressions.Add(ParseExpression());
                }

                // If the token is not newline then throw an error
                if (!MatchToken(TType.NEWLINE))
                    throw new Exception($"({_current_token.Line},{_current_token.Column}): Unexpected {_current_token.Token_Type} token expected {TType.NEWLINE} token");

                // Create the Display Statement
                return new DisplayNode(display_token, expressions);
            }
            // If display starts with expression
            // ex. DISPLAY: 6 ....
            else if (MatchToken(TType.ESCAPE) || MatchToken(TType.IDENTIFIER) || MatchToken(TType.INTLITERAL) || MatchToken(TType.FLOATLITERAL)
                || MatchToken(TType.CHARLITERAL) || MatchToken(TType.BOOLLITERAL) || MatchToken(TType.STRINGLITERAL)
                || MatchToken(TType.MINUS) || MatchToken(TType.PLUS) || MatchToken(TType.NOT))
            {
                expressions.Add(ParseExpression());

                // While token is & iterate until no & token.
                while (MatchToken(TType.AMPERSAND))
                {
                    ConsumeToken(TType.AMPERSAND);

                    // If newline is next to & throw an error
                    if (MatchToken(TType.NEWLINE))
                        throw new Exception($"({_current_token.Line},{_current_token.Column}): Unexpected {_current_token.Token_Type} token expected expression token");

                    // If $ is next to &, create a new Literal Expression with
                    // the value \n
                    if (MatchToken(TType.DOLLAR))
                    {
                        expressions.Add(new LiteralNode(_current_token, "\n"));
                        ConsumeToken(TType.DOLLAR);
                    }
                    // Else get the expression
                    else
                        expressions.Add(ParseExpression());
                }

                // If the token is not newline then throw an error
                if (!MatchToken(TType.NEWLINE))
                    throw new Exception($"({_current_token.Line},{_current_token.Column}): Unexpected {_current_token.Token_Type} token expected {TType.NEWLINE} token");

                // Create the Display Statement
                return new DisplayNode(display_token, expressions);
            }
            // If display starts with '&'
            // ex. DISPLAY: & ....
            else
                throw new Exception($"({_current_token.Line},{_current_token.Column}): Unexpected {_current_token.Token_Type} token expected expression token");
        }

        private StatementNode ParseScanStatement()
        {
            // ex. SCAN: a,b
            // Read and remove the current token
            Token scan_token = _current_token;
            ConsumeToken(TType.SCAN);
            ConsumeToken(TType.COLON);

            // List of identifiers
            List<string> identifiers = new List<string>();

            // Store the identifier and remove the current token
            identifiers.Add(_current_token.Code);
            ConsumeToken(TType.IDENTIFIER);

            // While the token is comma then iterate until it doesn't see a comma
            while (MatchToken(TType.COMMA))
            {
                ConsumeToken(TType.COMMA);
                identifiers.Add(_current_token.Code);
                ConsumeToken(TType.IDENTIFIER);
            }

            // Create the Scan Statement
            return new ScanNode(scan_token, identifiers);
        }

        private StatementNode ParseIfStatement()
        {
            bool is_else = false;
            // List of condition in every if, else if
            List<ExpressionNode> conditions = new List<ExpressionNode>();
            // List of statement in every if, else if, else
            List<ProgramNode> statement_blocks = new List<ProgramNode>();
            // List of tokens (IF, ELSE, ELSE)
            List<Token> tokens = new List<Token>();

            // ex. IF (a == b)
            // Store token and condition
            tokens.Add(_current_token);
            ConsumeToken(TType.IF);
            conditions.Add(ParseConditionExpression());

            // Store the statement block in if
            statement_blocks.Add(ParseProgram(TType.IF));

            // While the token is else, iterate
            while (MatchToken(TType.ELSE))
            {
                // Check if the else only statement has already passed
                if (is_else)
                    throw new Exception($"({_current_token.Line}, {_current_token.Column}): Invalid syntax {_current_token.Token_Type}");

                // Store token
                tokens.Add(_current_token);
                ConsumeToken(TType.ELSE);

                // ELSE IF (a <> b)
                if (MatchToken(TType.IF))
                {
                    // Store condition
                    ConsumeToken(TType.IF);
                    conditions.Add(ParseConditionExpression());
                }
                // ELSE
                else
                {
                    conditions.Add(null);
                    is_else = true;
                }

                // Store the statement block in else
                statement_blocks.Add(ParseProgram(TType.IF));
            }

            // Create the Condition Statement
            return new ConditionalNode(tokens, conditions, statement_blocks);
        }

        private StatementNode ParseWhileStatement()
        {
            // ex. WHILE (a < 5)
            // Read and remove the current token
            Token while_token = _current_token;
            ConsumeToken(TType.WHILE);

            // Parse the condition
            ExpressionNode condition = ParseConditionExpression();

            // Parse the statement block
            ProgramNode statement_block = ParseProgram(TType.WHILE);

            // Create the Loop Statement
            return new LoopNode(while_token, condition, statement_block);
        }
        // End Parse Statements

        // Start Parse Expressions
        private ExpressionNode ParseExpression()
        {
            ExpressionNode expression;
            Debug.WriteLine(_current_token);
            if (MatchToken(TType.ESCAPE))
            {
                Token escape_token = _current_token;
                ConsumeToken(TType.ESCAPE);
                return new LiteralNode(escape_token, escape_token.Value);
            }
            else if (MatchToken(TType.OPENPARENTHESIS))
            {
                expression = ParseParenthesisExpression();
                return expression;
            }
            else if (MatchToken(TType.PLUS) || MatchToken(TType.MINUS) || MatchToken(TType.NOT))
            {
                expression = ParseUnaryExpression();
                return expression;
            }
            else if (MatchToken(TType.IDENTIFIER) || MatchToken(TType.INTLITERAL) || MatchToken(TType.FLOATLITERAL)
                || MatchToken(TType.CHARLITERAL) || MatchToken(TType.BOOLLITERAL) || MatchToken(TType.STRINGLITERAL))
            {
                expression = ParseBinaryExpression();
                return expression;
            }
            else
                throw new Exception($"({_current_token.Line}, {_current_token.Column}): Unexpected {_current_token.Token_Type} token expected expression token.");
        }

        private ExpressionNode ParseParenthesisExpression()
        {
            // ex. ((1 + 1) - 10) * 15
            // Read and remove the current token
            Token open_parenthesis = _current_token;
            ConsumeToken(TType.OPENPARENTHESIS);

            ExpressionNode expression = ParseExpression();

            // Read and remove the current token
            Token close_parenthesis = _current_token;
            ConsumeToken(TType.CLOSEPARENTHESIS);

            // Get precedence of the next token
            int precedence = Grammar.GetBinaryPrecedence(_current_token.Token_Type);

            // If precedence is greater than zero
            if (precedence > 0)
            {
                // Create the parenthesis expression and pass it to the ParseBinaryExpression
                ParenthesisNode paren_expr = new ParenthesisNode(open_parenthesis, expression, close_parenthesis);
                return ParseBinaryExpression(paren_expr);

            }
            // Create the parenthesis expression
            return new ParenthesisNode(open_parenthesis, expression, close_parenthesis);
        }

        private ExpressionNode ParseConditionExpression()
        {
            // ex. (a==b)
            // Read and remove the current token
            Token open_parenthesis = _current_token;
            ConsumeToken(TType.OPENPARENTHESIS);

            ExpressionNode expression = ParseExpression();

            // Read and remove the current token
            Token close_parenthesis = _current_token;
            ConsumeToken(TType.CLOSEPARENTHESIS);

            // Create the parenthesis expression
            return new ParenthesisNode(open_parenthesis, expression, close_parenthesis);
        }

        private ExpressionNode ParseUnaryExpression()
        {
            // ex. -1 || +2 || NOT "TRUE"
            // Read and remove the current token
            Token unary_token = _current_token;
            ConsumeToken(unary_token.Token_Type);
            ExpressionNode expression = null;
            if (MatchToken(TType.OPENPARENTHESIS))
                expression = ParseExpression();
            else
                expression = ParseTerm();

            UnaryNode unary_expr = new UnaryNode(unary_token, expression);

            if (Grammar.GetBinaryPrecedence(_current_token.Token_Type) > 0)
                return ParseBinaryExpression(unary_expr);

            // Create the unary expression
            return unary_expr;
        }

        private ExpressionNode ParseBinaryExpression(ExpressionNode prev_left = null)
        {
            // ex. 10 + 6 * 6
            // If prev_left parameter is not null
            // set left as prev_left
            ExpressionNode left;
            if (prev_left != null)
                left = prev_left;
            // Else set left using ParseTerm
            else
                left = ParseTerm();

            // Get precedence of the next token
            int precedence = Grammar.GetBinaryPrecedence(_current_token.Token_Type);

            // While precedence is greater than zero means that the current token is an operator
            while (precedence > 0)
            {
                // Read and remove the current operator token
                Token binary_token = _current_token;
                ConsumeToken(binary_token.Token_Type);

                // Set right using ParseTerm
                ExpressionNode right = ParseTerm();

                // Get next precedence of the next token
                int next_precedence = Grammar.GetBinaryPrecedence(_current_token.Token_Type);

                // If the next precedence is greater than to the previous precedence
                // then set right using ParseBinaryExpression and pass the previous value of right as parameter
                if (next_precedence > precedence)
                    right = ParseBinaryExpression(right);
                // Set left as new Binary node using the previous value of left
                left = new BinaryNode(left, binary_token, right);

                // Get precedence of the next token
                precedence = Grammar.GetBinaryPrecedence(_current_token.Token_Type);
            }

            return left;
        }

        private ExpressionNode ParseTerm()
        {
            if (MatchToken(TType.IDENTIFIER))
            {
                Token identifier_token = _current_token;
                ConsumeToken(TType.IDENTIFIER);
                return new IdentifierNode(identifier_token, identifier_token.Code);
            }
            else if (MatchToken(TType.INTLITERAL) || MatchToken(TType.FLOATLITERAL) || MatchToken(TType.CHARLITERAL)
                || MatchToken(TType.BOOLLITERAL) || MatchToken(TType.STRINGLITERAL))
            {
                Token literal_token = _current_token;
                ConsumeToken(literal_token.Token_Type);
                return new LiteralNode(literal_token, literal_token.Value);
            }
            else
                return ParseExpression();
            //throw new Exception($"({_current_token.Line}, {_current_token.Column}): Unexpected {_current_token} token expected expression token");
        }

        // End Parse Expressions

        // Get next token
        private void ConsumeToken(TType token_type)
        {
            // If param token_type is the same as the current token type
            if (MatchToken(token_type))
            {
                Token prev_token = _current_token;
                _current_token = _lexer.GetToken();
                // If the new current token is error
                if (MatchToken(TType.ERROR))
                {
                    // Check if the previous token is data type
                    if (prev_token.Token_Type == TType.INT || prev_token.Token_Type == TType.FLOAT || prev_token.Token_Type == TType.CHAR || prev_token.Token_Type == TType.BOOL)
                    {
                        // If the value/message of the error token is Invalid keyword or Invalid data type
                        if (_current_token.Value.ToString().Contains("Invalid keyword") || _current_token.Value.ToString().Contains("Invalid data type"))
                        {
                            _current_token.Token_Type = TType.IDENTIFIER;
                            _current_token.Value = null;
                        }
                    }
                    // If the previous token is not data type but in the variable names list it contains the code
                    else if (_variable_names.Contains(_current_token.Code))
                    {
                        _current_token.Token_Type = TType.IDENTIFIER;
                        _current_token.Value = null;
                    }
                    // If none of the condition met just throw exception
                    else
                        throw new Exception($"({_current_token.Line}, {_current_token.Column}): {_current_token.Value}");
                }
            }
            else
                throw new Exception($"({_current_token.Line},{_current_token.Column}): Unexpected token {_current_token.Token_Type} token expected {token_type} token");
        }

        // Match current to param {token_type}
        private bool MatchToken(TType token_type)
        {
            return _current_token.Token_Type == token_type;
        }

        // Helper method for variable declaration
        private (string, ExpressionNode) GetVariable()
        {
            Token identifier = _current_token;

            ConsumeToken(TType.IDENTIFIER);

            if (MatchToken(TType.EQUAL))
            {
                ConsumeToken(TType.EQUAL);
                return (identifier.Code, ParseExpression());
            }
            return (identifier.Code, null);
        }

    }
}
