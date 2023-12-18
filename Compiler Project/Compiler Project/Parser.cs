using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Tiny_Compiler;
using static System.Windows.Forms.AxHost;

namespace Compiler_Project
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
	public class Parser
	{
		int InputPointer = 0;
		List<Token> TokenStream;
		public Node root;
		public Node StartParsing(List<Token> TokenStream)
		{
			this.InputPointer = 0;
			this.TokenStream = TokenStream;
			// root = new Node("Program");
			// root.Children.Add(Program());
			return root;
		}
		public Node match(Token_Class ExpectedToken)
		{

			if (InputPointer < TokenStream.Count)
			{
				if (ExpectedToken == TokenStream[InputPointer].token_type)
				{
					InputPointer++;
					Node newNode = new Node(ExpectedToken.ToString());

					return newNode;
				}
				else
				{
					Errors.Error_List.Add("Parsing Error: Expected "
						+ ExpectedToken.ToString() + " and " +
						TokenStream[InputPointer].token_type.ToString() +
						"  found\r\n");
					InputPointer++;
					return null;
				}
			}
			else
			{
				Errors.Error_List.Add("Parsing Error: Expected "
						+ ExpectedToken.ToString() + "\r\n");
				InputPointer++;
				return null;
			}
		}

		public static TreeNode PrintParseTree(Node root)
		{
			TreeNode tree = new TreeNode("Parse Tree");
			TreeNode treeRoot = PrintTree(root);
			if (treeRoot != null)
				tree.Nodes.Add(treeRoot);
			return tree;
		}
		static TreeNode PrintTree(Node root)
		{
			if (root == null || root.Name == null)
				return null;
			TreeNode tree = new TreeNode(root.Name);
			if (root.Children.Count == 0)
				return tree;
			foreach (Node child in root.Children)
			{
				if (child == null)
					continue;
				tree.Nodes.Add(PrintTree(child));
			}
			return tree;
		}

		//    project code
		Node Program()
		{
			//================== msh Tmam ================

			Node program = new Node("Program");

			//Function_Statement Program | Main_Function 
			if (TokenStream[InputPointer + 1].token_type != Token_Class.main &&
				(TokenStream[InputPointer].token_type == Token_Class.Integer ||
				TokenStream[InputPointer].token_type == Token_Class.Float ||
				TokenStream[InputPointer].token_type == Token_Class.String))
			{
				program.Children.Add(Function_Statement());
				program.Children.Add(Program());
			}
			else if (TokenStream[InputPointer + 1].token_type == Token_Class.main)
			{
				program.Children.Add(Main_Function());
			}

			MessageBox.Show("Success");
			return program;
		}

		private Node Main_Function()
		{
			//================== msh Tmam ================

			Node main_func = new Node("Main_Function");

			//Datatype main () Function_Body
			main_func.Children.Add(Datatype());
			main_func.Children.Add(match(Token_Class.main));
			main_func.Children.Add(match(Token_Class.LeftParanthesis));
			main_func.Children.Add(match(Token_Class.RightParanthesis));
			main_func.Children.Add(Function_Body());

			return main_func;
		}

		private Node Function_Body()
		{
			//================== Tmam ================

			Node function_body = new Node("Function_Body");

			//{set_of_Statements Return_Statement } 
			function_body.Children.Add(match(Token_Class.LeftBrace));
			function_body.Children.Add(set_of_Statements());
			function_body.Children.Add(Return_Statement());
			function_body.Children.Add(match(Token_Class.RightBrace));

			return function_body;
		}

		private Node Return_Statement()
		{
			//================== Tmam ================

			Node return_statement = new Node("Return_Statement");

			//return Expression; 
			return_statement.Children.Add(match(Token_Class.Return));
			return_statement.Children.Add(Expression());
			return_statement.Children.Add(match(Token_Class.Semicolon));

			return return_statement;
		}

		private Node Expression()
		{
			//================== Tmam ================

			Node expression = new Node("Expression");

			//String | Term | Equation 
			if (TokenStream[InputPointer].token_type == Token_Class.String)
			{
				expression.Children.Add(match(Token_Class.String));
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.Number ||
				TokenStream[InputPointer].token_type == Token_Class.Identifier ||
				(TokenStream[InputPointer].token_type == Token_Class.Identifier &&
				TokenStream[InputPointer + 1].token_type == Token_Class.LeftParanthesis))
			{
				expression.Children.Add(Term());

			}
			else if (TokenStream[InputPointer].token_type == Token_Class.LeftParanthesis ||
					(TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
					 TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
					 TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
					 TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp))
			{
				expression.Children.Add(Equation());

			}

			return expression;
		}

		private Node Equation()
		{
			//================== Tmam ================

			Node equation = new Node("Equation");

			//Term Operations | (Term Operations ) Operations 
			if (TokenStream[InputPointer].token_type == Token_Class.Number ||
				TokenStream[InputPointer].token_type == Token_Class.Identifier ||
				(TokenStream[InputPointer].token_type == Token_Class.Identifier &&
				TokenStream[InputPointer + 1].token_type == Token_Class.LeftParanthesis))
			{
				equation.Children.Add(Term());
				equation.Children.Add(Operations());

			}
			else if (TokenStream[InputPointer].token_type == Token_Class.LeftParanthesis)
			{
				equation.Children.Add(match(Token_Class.LeftParanthesis));
				equation.Children.Add(Term());
				equation.Children.Add(Operations());
				equation.Children.Add(match(Token_Class.RightParanthesis));
				equation.Children.Add(Operations());
			}

			return equation;
		}

		private Node Operations()
		{
			//================== Tmam ================

			Node operations = new Node("Operations");

			//Arithmatic_Operator Equation | ε 
			//+ | - | * | / 

			if (TokenStream[InputPointer].token_type == Token_Class.PlusOp
				|| TokenStream[InputPointer].token_type == Token_Class.MinusOp
				|| TokenStream[InputPointer].token_type == Token_Class.MultiplyOp
				|| TokenStream[InputPointer].token_type == Token_Class.DivideOp)
			{
				operations.Children.Add(Arithmatic_Operator());
				operations.Children.Add(Equation());
			}
			else
				return null;

			return operations;
		}

		private Node Arithmatic_Operator()
		{
			//================== Tmam ================

			Node arithmatic_operator = new Node("Arithmatic_Operator");

			//+ | - | * | /
			if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
			{
				arithmatic_operator.Children.Add(match(Token_Class.PlusOp));
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
			{
				arithmatic_operator.Children.Add(match(Token_Class.MinusOp));
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
			{
				arithmatic_operator.Children.Add(match(Token_Class.MultiplyOp));
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
			{
				arithmatic_operator.Children.Add(match(Token_Class.DivideOp));
			}

			return arithmatic_operator;
		}

		private Node Term()
		{
			//================== Tmam ================

			Node term = new Node("Term");

			//Number | Identifier | Function_Call 
			if (TokenStream[InputPointer].token_type == Token_Class.Number)
			{
				term.Children.Add(match(Token_Class.Number));
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
			{
				term.Children.Add(match(Token_Class.Identifier));
			}
			else if (TokenStream[InputPointer + 1].token_type == Token_Class.LeftParanthesis)
			{
				term.Children.Add(Function_Call());
			}

			return term;
		}

		private Node Function_Call()
		{
			//================== Tmam ================

			Node function_call = new Node("Function_Call");

			//identifier (Arguments) 
			function_call.Children.Add(match(Token_Class.Identifier));
			function_call.Children.Add(match(Token_Class.LeftParanthesis));
			function_call.Children.Add(Arguments());
			function_call.Children.Add(match(Token_Class.RightParanthesis));

			return function_call;
		}

		private Node Arguments()
		{
			//================== i think Tmam ================

			Node arguments = new Node("Arguments");

			// identifier, Arguments | identifier | ε 

			if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
			{
				arguments.Children.Add(match(Token_Class.Identifier));
				if (TokenStream[InputPointer].token_type == Token_Class.Comma)
				{
					arguments.Children.Add(match(Token_Class.Comma));
					arguments.Children.Add(Arguments());
				}
			}
			else
				return null;

			return arguments;
		}

		private Node set_of_Statements()
		{
			//=========== Tmam ===============

			Node set_of_statements = new Node("set_of_Statements");

			//SetStat Set_of_Statements | ε 

			if (TokenStream[InputPointer].token_type == Token_Class.Float ||
				TokenStream[InputPointer].token_type == Token_Class.Integer ||
				TokenStream[InputPointer].token_type == Token_Class.String ||
				TokenStream[InputPointer].token_type == Token_Class.Identifier ||
				TokenStream[InputPointer].token_type == Token_Class.Write ||
				TokenStream[InputPointer].token_type == Token_Class.Read ||
				TokenStream[InputPointer].token_type == Token_Class.Repeat ||
				TokenStream[InputPointer].token_type == Token_Class.IF ||
				TokenStream[InputPointer].token_type == Token_Class.Comment)
			{
				set_of_statements.Children.Add(SetStat());
				set_of_statements.Children.Add(set_of_Statements());
			}
			else
				return null;

			return set_of_statements;
		}

		private Node SetStat()
		{
			//================== i think Tmam =========
			Node setStat = new Node("SetStat");

			// Assignment_Statement | Declaration_Statement | Write_Statement |Read_Statement | If_Statement | Repeat_Statement
			if (TokenStream[InputPointer].token_type == Token_Class.Identifier &&
				TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp)
			{
				setStat.Children.Add(Assignment_Statement());
			}
			// herrrrre 
			else if (TokenStream[InputPointer].token_type == Token_Class.Integer ||
				TokenStream[InputPointer].token_type == Token_Class.Float ||
				TokenStream[InputPointer].token_type == Token_Class.String)
			{
				setStat.Children.Add(Declaration_Statement());
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.Write)
			{
				setStat.Children.Add(Write_Statement());
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.Read)
			{
				setStat.Children.Add(Read_Statement());
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.IF)
			{
				setStat.Children.Add(If_Statement());
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
			{
				setStat.Children.Add(Repeat_Statement());
			}

			return setStat;
		}

		private Node Repeat_Statement()
		{
			//================== Tmam =========

			Node repeat = new Node("Repeat_Statement");

			//repeat set_of_Statements until Condition_Statement
			repeat.Children.Add(match(Token_Class.Repeat));
			repeat.Children.Add(set_of_Statements());
			repeat.Children.Add(match(Token_Class.Until));
			repeat.Children.Add(Condition_Statement());

			return repeat;
		}

		private Node Condition_Statement()
		{
			//=============here
			Node condition = new Node("Condition_Statement");

			//Condition_Statement -> Condition Boolean_Operator  
			condition.Children.Add(Condition());
			condition.Children.Add(Boolean_Operator());
			
			return condition;
		}

		private Node Boolean_Operator()
		{
			//============= Tmam ============
			Node boolean_op = new Node("Boolean_Operator");

			// && Condition_Statement  | || Condition_Statement | ε 

			if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
			{
				boolean_op.Children.Add(match(Token_Class.AndOp));
				boolean_op.Children.Add(Condition_Statement());
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.OrOp)
			{
				boolean_op.Children.Add(match(Token_Class.OrOp));
				boolean_op.Children.Add(Condition_Statement());
			}
			else
				return null;

			return boolean_op;
		}

		private Node Condition()
		{
			//=========== tmam ===========

			Node cond = new Node("Condition");

			//	Condition -> identifier Condition_Operator Term 
			cond.Children.Add(match(Token_Class.Identifier));
			cond.Children.Add(Condition_Operator());
			cond.Children.Add(Term());
			return cond;
		}

		private Node Condition_Operator()
		{
			//========== tmam ============

			Node cond_op = new Node("Condition_Operator");

			//< | > | = | <>
			if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
			{
				cond_op.Children.Add(match(Token_Class.LessThanOp));
			}
			else if(TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
			{
				cond_op.Children.Add(match(Token_Class.GreaterThanOp));
			}
			else if(TokenStream[InputPointer].token_type == Token_Class.EqualOp)
			{
				cond_op.Children.Add(match(Token_Class.EqualOp));
			}
            else if(TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
				cond_op.Children.Add(match(Token_Class.NotEqualOp));
			}

            return cond_op;
		}

		private Node If_Statement()
		{
			//=========== tmam =========

			Node if_statement = new Node("If_Statement");

			//if Con_Statement Options 
			if_statement.Children.Add(match(Token_Class.IF));
			if_statement.Children.Add(Con_Statement());
			if_statement.Children.Add(Options());

			return if_statement;
		}

		private Node Options()
		{
			//======= tmam =======

			Node options = new Node("Options");

			// Options -> Else_If_Statement Options | Else_Statement | end

			if (TokenStream[InputPointer].token_type == Token_Class.Else )
			{
				options.Children.Add(Else_Statement());
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.ElseIf)
			{
				options.Children.Add(Else_If_Statement());
				options.Children.Add(Options());
			}
			else if(TokenStream[InputPointer].token_type == Token_Class.End)
			{
				options.Children.Add(match(Token_Class.End));
			}

			return options;
		}

		private Node Else_If_Statement()
		{
			//======= tmam =======

			Node elsf = new Node("Else_If_Statement");

			//   Else_If_Statement -> elseif Con_Statement
			elsf.Children.Add(match(Token_Class.ElseIf));
			elsf.Children.Add(Con_Statement());

			return elsf;
		}

		private Node Else_Statement()
		{
			//======= tmam =======

			Node els = new Node("Else_Statement");

			// Else_Statement -> else set_of_Statements end
			els.Children.Add(match(Token_Class.Else));
			els.Children.Add(set_of_Statements());
			els.Children.Add(match(Token_Class.End));

			return els;
		}

		private Node Con_Statement()
		{
			//======= tmam =======

			Node con = new Node("Con_Statement");

			//Con_Statement -> Condition_Statement then set_of_Statements
			con.Children.Add(Con_Statement());
			con.Children.Add(match(Token_Class.Then));
			con.Children.Add(set_of_Statements());

			return con;
		}

		private Node Read_Statement()
		{
			//======= tmam =======

			Node read = new Node("Read_Statement");

			//read identifier ; 
			read.Children.Add(match(Token_Class.Read));
			read.Children.Add(match(Token_Class.Identifier));

			return read;
		}

		private Node Write_Statement()
		{
			//======= tmam =======

			Node write = new Node("Write_Statement");

			//write Write_stat ;  
			write.Children.Add(match(Token_Class.Write));
			write.Children.Add(Write_stat());

			return write;
		}

		private Node Write_stat()
		{
			//======= tmam =======

			Node write = new Node("Write_stat");

			// Expression | endl 
			if (TokenStream[InputPointer].token_type == Token_Class.Endl)
			{
				write.Children.Add(match(Token_Class.Endl));
			}
			else 
			{
				write.Children.Add(Expression());
			}
			return write;
		}


		private Node Declaration_Statement()
		{
			//============= Tmam =============

			Node declaration_statement = new Node("Declaration_Statement");

			//  Declaration_Statement -> Datatype Declaration_stat;
			declaration_statement.Children.Add(Datatype());
			declaration_statement.Children.Add(Declaration_stat());

			return declaration_statement;
		}

		private Node Declaration_stat()
		{
			//============= i think Tmam =============

			Node dec_stat = new Node("Declaration_stat");

			// Declaration_stat ->identifiers , Declaration_stat | Assignment_Statement , Declaration_stat | identifiers | Assignment_Statement 

			if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
			{
				dec_stat.Children.Add(match(Token_Class.Identifier));
				if (TokenStream[InputPointer].token_type == Token_Class.Comma)
				{
					dec_stat.Children.Add(match(Token_Class.Comma));
					dec_stat.Children.Add(Declaration_stat());
				}
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp)
			{
				dec_stat.Children.Add(Assignment_Statement());
				if (TokenStream[InputPointer].token_type == Token_Class.Comma)
				{
					dec_stat.Children.Add(match(Token_Class.Comma));
					dec_stat.Children.Add(Declaration_stat());
				}
			}

			return dec_stat;
		}

		private Node Assignment_Statement()
		{
			//============= Tmam =============

			Node assignment_statement = new Node("Assignment_Statement");

			//	Assignment_Statement -> identifier assignmentoperator Expression 
			assignment_statement.Children.Add(match(Token_Class.Identifier));
			assignment_statement.Children.Add(match(Token_Class.AssignOp));
			assignment_statement.Children.Add(Expression());

			return assignment_statement;
		}

		private Node Datatype()
		{
			//============= Tmam =============

			Node datatype = new Node("Datatype");

			//int | float | string 
			if (TokenStream[InputPointer].token_type == Token_Class.Integer)
			{
				datatype.Children.Add(match(Token_Class.Integer));
			}
			else if (TokenStream[InputPointer].token_type == Token_Class.Float)
			{
				datatype.Children.Add(match(Token_Class.Float));
			}
			else if (TokenStream[InputPointer + 1].token_type == Token_Class.String)
			{
				datatype.Children.Add(match(Token_Class.String));
			}
			return datatype;
			;
		}

		private Node Function_Statement()
		{
			//============= Tmam =============

			Node function_Statement = new Node("Function_Statement");

			//Function_Declaration Function_Body 
			function_Statement.Children.Add(Function_Declaration());
			function_Statement.Children.Add(Function_Body());

			return function_Statement;
		}

		private Node Function_Declaration()
		{
			//============= Tmam =============

			Node function_declaration = new Node("Function_Declaration");

			//Datatype FunctionName (Function_Parameters) 

			function_declaration.Children.Add(Datatype());
			function_declaration.Children.Add(FunctionName());
			function_declaration.Children.Add(match(Token_Class.LeftParanthesis));
			function_declaration.Children.Add(Function_Parameters());
			function_declaration.Children.Add(match(Token_Class.RightParanthesis));

			return function_declaration;
		}

		private Node Function_Parameters()
		{
			//============= i think Tmam =============

			Node function_parameters = new Node("Function_Parameters");

			//   Function_Parameters -> Parameter ,  Function_Parameters | Parameter | ε
			//   Parameter -> Datatype identifier


			if (TokenStream[InputPointer].token_type == Token_Class.Integer ||
				TokenStream[InputPointer].token_type == Token_Class.Float ||
				TokenStream[InputPointer].token_type == Token_Class.String)
			{
				function_parameters.Children.Add(Parameter());
				if (TokenStream[InputPointer].token_type == Token_Class.Comma)
				{
					function_parameters.Children.Add(match(Token_Class.Comma));
					function_parameters.Children.Add(Function_Parameters());
				}
			}
			else
				return null;

			return function_parameters;
		}

		private Node Parameter()
		{
			//=========== Tmam ==========

			Node parameter = new Node("Parameter");

			//   Parameter -> Datatype identifier
			parameter.Children.Add(Datatype());
			parameter.Children.Add(match(Token_Class.Identifier));

			return parameter;
		}

		private Node FunctionName()
		{
			//============= Tmam =============

			Node functionName = new Node("FunctionName");

			// identifier 
			functionName.Children.Add(match(Token_Class.Identifier));

			return functionName;
		}
	}
}
