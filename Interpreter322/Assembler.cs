using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter322
{
    public abstract class ASTNode {}

    public abstract class ExpressionNode : ASTNode {}

    public class BinaryNode : ExpressionNode
    {
        public BinaryNode(ExpressionNode left, Token token_operator, ExpressionNode right)
        {
            Left = left;
            Token_Operator = token_operator;
            Right = right;
        }

        public ExpressionNode Left { get; }
        public Token Token_Operator { get; }
        public ExpressionNode Right { get; }
    }

    public class IdentifierNode : ExpressionNode
    {
        public IdentifierNode(Token identifier_token, string name)
        {
            Identifier_Token = identifier_token;
            Name = name;
        }

        public Token Identifier_Token { get; }
        public string Name { get; }
    }

    public class LiteralNode : ExpressionNode
    {
        public LiteralNode(Token literal_token, object literal)
        {
            Literal_Token = literal_token;
            Literal = literal;
        }

        public Token Literal_Token { get; }
        public object Literal { get; }
    }

    public class ParenthesisNode : ExpressionNode
    {
        public ParenthesisNode(Token open, ExpressionNode expression, Token close)
        {
            Open = open;
            Expression = expression;
            Close = close;
        }

        public Token Open { get; }
        public ExpressionNode Expression { get; }
        public Token Close { get; }
    }

    public class UnaryNode : ExpressionNode
    {
        public UnaryNode(Token token_operator, ExpressionNode expression)
        {
            Token_Operator = token_operator;
            Expression = expression;
        }

        public Token Token_Operator { get; }
        public ExpressionNode Expression { get; }
    }

    public abstract class StatementNode : ASTNode {}

    public class DisplayNode : StatementNode
    {
        public DisplayNode(Token display_token, List<ExpressionNode> expressions)
        {
            Display_Token = display_token;
            Expressions = expressions;
        }

        public Token Display_Token { get; }
        public List<ExpressionNode> Expressions { get; }
    }

    public class AssignmentNode : StatementNode
    {
        public AssignmentNode(List<string> identifiers, List<Token> equals_token, ExpressionNode expression)
        {
            Identifiers = identifiers;
            Equals_Token = equals_token;
            Expression = expression;
        }

        public List<string> Identifiers { get; }
        public List<Token> Equals_Token { get; }
        public ExpressionNode Expression { get; }
    }

    public class ConditionalNode : StatementNode
    {
        public ConditionalNode(List<Token> tokens, List<ExpressionNode> expressions, List<ProgramNode> statements)
        {
            Tokens = tokens;
            Expressions = expressions;
            Statements = statements;
        }

        public List<Token> Tokens { get; }
        public List<ExpressionNode> Expressions { get; }
        public List<ProgramNode> Statements { get; }
    }

    public class LoopNode : StatementNode
    {
        public LoopNode(Token while_token, ExpressionNode expression, ProgramNode statement)
        {
            While_Token = while_token;
            Expression = expression;
            Statement = statement;
        }

        public Token While_Token { get; }
        public ExpressionNode Expression { get; }
        public ProgramNode Statement { get; }
    }

    public class ScanNode : StatementNode
    {
        public ScanNode(Token scan_token, List<string> identifiers)
        {
            Scan_Token = scan_token;
            Identifiers = identifiers;
        }

        public Token Scan_Token { get; }
        public List<string> Identifiers { get; }
    }

    public class VariableDeclarationNode : StatementNode
    {
        public VariableDeclarationNode(Token data_type_token, Dictionary<string, ExpressionNode> variables)
        {
            Data_Type_Token = data_type_token;
            Variables = variables;
        }

        public Token Data_Type_Token { get; }
        public Dictionary<string, ExpressionNode> Variables { get; }
    }

    public class ProgramNode : ASTNode
    {
        public ProgramNode(List<StatementNode> statements)
        {
            Statements = statements;
        }

        public List<StatementNode> Statements { get; }
    }
}
