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
			root = Program();
			return root;
		}

		//    project code
		Node Program()
		{
			//================== Tmam ================

			if (InputPointer < TokenStream.Count)
			{
				Node program = new Node("Program");

				//Function_Statement Program | Main_Function 
				if (TokenStream[InputPointer + 1].token_type != Token_Class.main)
				{ 
					program.Children.Add(Function_Statement());
				    program.Children.Add(Program());
			    }
				else if (TokenStream[InputPointer + 1].token_type == Token_Class.main)
				{
					program.Children.Add(Main_Function());
					if (InputPointer < TokenStream.Count)
					{
						Errors.Error_List.Add("Parsing Error: Expected nothing after main function \r\n");

					}
				}

				MessageBox.Show("Success");
				return program;
			}
			return null;
		}

		private Node Main_Function()
		{
			//================== msh Tmam ================

			if (InputPointer < TokenStream.Count)
			{
			    Node main_func = new Node("Main_Function");

			    //Datatype main () Function_Body
				main_func.Children.Add(Datatype());
				main_func.Children.Add(match(Token_Class.main));
				main_func.Children.Add(match(Token_Class.LeftParanthesis));
				main_func.Children.Add(match(Token_Class.RightParanthesis));
				main_func.Children.Add(Function_Body());

				return main_func;
			}
			return null;
		}

		private Node Function_Body()
		{
			//================== Tmam ================

			if (InputPointer < TokenStream.Count)
			{
			    Node function_body = new Node("Function_Body");

			    //{set_of_Statements Return_Statement } 
				function_body.Children.Add(match(Token_Class.LeftBrace));
				function_body.Children.Add(set_of_Statements());
				function_body.Children.Add(Return_Statement());
				function_body.Children.Add(match(Token_Class.RightBrace));

				return function_body;
			}
			return null;
		}

		private Node Return_Statement()
		{
			//================== Tmam ================

			if (InputPointer < TokenStream.Count)
			{
			    Node return_statement = new Node("Return_Statement");

			    //return Expression; 
				return_statement.Children.Add(match(Token_Class.Return));
				return_statement.Children.Add(Expression());
				return_statement.Children.Add(match(Token_Class.Semicolon));

				return return_statement;
			}
			return null;
		}

		private Node Expression()
		{
			//================== Tmam ================
			if (InputPointer < TokenStream.Count)
			{
			     Node expression = new Node("Expression");

			     //String | Term | Equation
				if (TokenStream[InputPointer].token_type == Token_Class.String)
				{
					expression.Children.Add(match(Token_Class.String));
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.LeftParanthesis ||
					     TokenStream[InputPointer].token_type == Token_Class.Number ||
					     TokenStream[InputPointer].token_type == Token_Class.Identifier &&
						 (TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
						 TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
						 TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
						 TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp))
				{
					expression.Children.Add(Equation());

				}
				else if (TokenStream[InputPointer].token_type == Token_Class.Number ||
					TokenStream[InputPointer].token_type == Token_Class.Identifier)
				{
					expression.Children.Add(Term());

				}
				else
				{
                        
                        Errors.Error_List.Add("Parsing Error Expected Either a String or a Term or an Equation ");
                        
                        return null;
                  
                }



                return expression;
			}
			return null;
		}

		private Node Equation()
		{
			//================== Tmam ================

			if (InputPointer < TokenStream.Count)
			{
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
			return null;
		}

		private Node Operations()
		{
			//================== Tmam ================

			if (InputPointer < TokenStream.Count)
			{
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
			return null;
		}

		private Node Arithmatic_Operator()
		{
			//================== Tmam ================

			if (InputPointer < TokenStream.Count)
			{
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
			return null;
		}

		private Node Term()
		{
			//================== Tmam ================

			if (InputPointer < TokenStream.Count)
			{
			     Node term = new Node("Term");

			//Number | Identifier | Function_Call
			//
				if (TokenStream[InputPointer].token_type == Token_Class.Number)
				{
					term.Children.Add(match(Token_Class.Number));
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.LeftParanthesis)
				{
					term.Children.Add(Function_Call());
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
				{
					term.Children.Add(match(Token_Class.Identifier));
				}

                return term;
			}
			return null;
		}

		private Node Function_Call()
		{
			//================== Tmam ================
			if (InputPointer < TokenStream.Count)
			{
			    Node function_call = new Node("Function_Call");

			//identifier (Arguments) 
				function_call.Children.Add(match(Token_Class.Identifier));
				function_call.Children.Add(match(Token_Class.LeftParanthesis));
				function_call.Children.Add(Arguments());
				function_call.Children.Add(match(Token_Class.RightParanthesis));

				return function_call;
			}
			return null;
		}

		private Node Arguments()
		{
			//================== i think Tmam ================

			if (InputPointer < TokenStream.Count)
			{
			    Node arguments = new Node("Arguments");


				// identifier, Arguments | identifier | ε 

				//if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
				//{
				//	arguments.Children.Add(match(Token_Class.Identifier));
				//	if (TokenStream[InputPointer].token_type == Token_Class.Comma)
				//	{
				//		arguments.Children.Add(match(Token_Class.Comma));
				//		arguments.Children.Add(Arguments());
				//	}
				//}
				//else if (TokenStream[InputPointer].token_type == Token_Class.Number)
				//{
				//	arguments.Children.Add(match(Token_Class.Number));
				//	if (TokenStream[InputPointer].token_type == Token_Class.Comma)
				//	{
				//		arguments.Children.Add(match(Token_Class.Comma));
				//		arguments.Children.Add(Arguments());
				//	}
				//}
				//else
				//	return null;



				// Expression, Arguments | Expression | ε 
				Node temp = Expression();
				if(temp!=null)
				{
					arguments.Children.Add(temp);
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
			return null;
		}

		private Node set_of_Statements()
		{
			//=========== Tmam ===============

			if (InputPointer < TokenStream.Count)
			{

			//SetStat Set_of_Statements | ε 
				if (TokenStream[InputPointer].token_type == Token_Class.Float ||
				TokenStream[InputPointer].token_type == Token_Class.Integer ||
				TokenStream[InputPointer].token_type == Token_Class.Type_String ||
				TokenStream[InputPointer].token_type == Token_Class.Identifier ||
				TokenStream[InputPointer].token_type == Token_Class.Write ||
				TokenStream[InputPointer].token_type == Token_Class.Read ||
				TokenStream[InputPointer].token_type == Token_Class.Repeat ||
				TokenStream[InputPointer].token_type == Token_Class.IF)
				{
			        Node set_of_statements = new Node("set_of_Statements");
					set_of_statements.Children.Add(SetStat());
					Node temp = set_of_Statements();
					if(temp != null)
					{
						set_of_statements.Children.Add(temp);
					}
					
				    return set_of_statements;
				}
				else
					return null;

			}
			return null;
		}

		private Node SetStat()
		{
			//================== i think Tmam =========
			if (InputPointer < TokenStream.Count)
			{
			     Node setStat = new Node("SetStat");

			// Assignment_Statement | Declaration_Statement | Write_Statement |Read_Statement | If_Statement | Repeat_Statement
				if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
				{
					setStat.Children.Add(Assignment_Statement());
				}
				// herrrrre 
				else if (TokenStream[InputPointer].token_type == Token_Class.Integer ||
					TokenStream[InputPointer].token_type == Token_Class.Float ||
					TokenStream[InputPointer].token_type == Token_Class.Type_String)
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
				else
				{
                    Errors.Error_List.Add("Parsing Error Not A Valid Staement");
					return null;

                }


                return setStat;
			}
			return null;
		}

		private Node Repeat_Statement()
		{
			//================== Tmam =========
			if (InputPointer < TokenStream.Count)
			{
				Node repeat = new Node("Repeat_Statement");

				//repeat set_of_Statements until Condition_Statement
				repeat.Children.Add(match(Token_Class.Repeat));
				repeat.Children.Add(set_of_Statements());
				repeat.Children.Add(match(Token_Class.Until));
				repeat.Children.Add(Condition_Statement());

				return repeat;
			}
			return null;
		}

		private Node Condition_Statement()
		{
			if (InputPointer < TokenStream.Count)
			{
				Node condition = new Node("Condition_Statement");

				//Condition_Statement -> Condition Boolean_Operator  
				condition.Children.Add(Condition());
				condition.Children.Add(Boolean_Operator());

				return condition;
			}
			return null;
		}

		private Node Boolean_Operator()
		{
			//============= Tmam ============
			if (InputPointer < TokenStream.Count)
			{
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
			return null;
		}

		private Node Condition()
		{
			//=========== tmam ===========
			if (InputPointer < TokenStream.Count)
			{
				Node cond = new Node("Condition");

				//	Condition -> identifier Condition_Operator Term 
				cond.Children.Add(match(Token_Class.Identifier));
				cond.Children.Add(Condition_Operator());
				cond.Children.Add(Term());
				return cond;
			}
			return null;
		}

		private Node Condition_Operator()
		{
			//========== tmam ============
			if (InputPointer < TokenStream.Count)
			{
				Node cond_op = new Node("Condition_Operator");

				//< | > | = | <>
				if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
				{
					cond_op.Children.Add(match(Token_Class.LessThanOp));
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
				{
					cond_op.Children.Add(match(Token_Class.GreaterThanOp));
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
				{
					cond_op.Children.Add(match(Token_Class.EqualOp));
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
				{
					cond_op.Children.Add(match(Token_Class.NotEqualOp));
				}


                return cond_op;
			}
			return null;
		}

		private Node If_Statement()
		{
			//=========== tmam =========
			if (InputPointer < TokenStream.Count)
			{
				Node if_statement = new Node("If_Statement");

				//if Con_Statement Options end 

				if_statement.Children.Add(match(Token_Class.IF));
				Node con = Con_Statement();
				if(con != null)
				{
					if_statement.Children.Add(con);
					Node op = Options();
					if (op != null)
						if_statement.Children.Add(op);

				}
				if_statement.Children.Add(match(Token_Class.End));
				
				return if_statement;
			}
			return null;
		}

		private Node Options()
		{
			//======= tmam =======
			if (InputPointer < TokenStream.Count)
			{
				// Options -> Else_If_Statement Options | Else_Statement | end

				if (TokenStream[InputPointer].token_type == Token_Class.Else)
				{
				    Node options = new Node("Options");
					options.Children.Add(Else_Statement());
					return options;
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.ElseIf)
				{
					Node options = new Node("Options");
					options.Children.Add(Else_If_Statement());
					options.Children.Add(Options());
					return options;
				}
            }
            return null;
		}

		private Node Else_If_Statement()
		{
			//======= tmam =======
			if (InputPointer < TokenStream.Count)
			{
				Node elsf = new Node("Else_If_Statement");

				//   Else_If_Statement -> elseif Con_Statement
				elsf.Children.Add(match(Token_Class.ElseIf));
				elsf.Children.Add(Con_Statement());

				return elsf;
			}
			return null;
		}

		private Node Else_Statement()
		{
			//======= tmam =======
			if (InputPointer < TokenStream.Count)
			{
				Node els = new Node("Else_Statement");

				// Else_Statement -> else set_of_Statements end
				els.Children.Add(match(Token_Class.Else));
				els.Children.Add(set_of_Statements());

				return els;
			}
			return null;
		}

		private Node Con_Statement()
		{
			//======= tmam =======
			if (InputPointer < TokenStream.Count)
			{
				Node con = new Node("Con_Statement");

				//Con_Statement -> Condition_Statement then set_of_Statements
				con.Children.Add(Condition_Statement());
				con.Children.Add(match(Token_Class.Then));
				con.Children.Add(set_of_Statements());

				return con;
			}
			return null;
		}

		private Node Read_Statement()
		{
			//======= tmam =======
			if (InputPointer < TokenStream.Count)
			{
				Node read = new Node("Read_Statement");

				//read identifier ; 
				read.Children.Add(match(Token_Class.Read));
				read.Children.Add(match(Token_Class.Identifier));
				read.Children.Add(match(Token_Class.Semicolon));
				return read;
			}
			return null;	
		}

		private Node Write_Statement()
		{
			//======= tmam =======
			if (InputPointer < TokenStream.Count)
			{
				Node write = new Node("Write_Statement");

				//write Write_stat ;  
				write.Children.Add(match(Token_Class.Write));
				write.Children.Add(Write_stat());

				return write;
			}
			return null;	
		}

		private Node Write_stat()
		{
			//======= tmam =======
			if (InputPointer < TokenStream.Count)
			{
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
				write.Children.Add(match(Token_Class.Semicolon));
				return write;
			}
			return null;	
		}

		private Node Declaration_Statement()
		{
			//============= Tmam =============
			if (InputPointer < TokenStream.Count)
			{
				Node declaration_statement = new Node("Declaration_Statement");

				//  Declaration_Statement -> Datatype Declaration_stat;
				declaration_statement.Children.Add(Datatype());
				declaration_statement.Children.Add(Declaration_stat());
				declaration_statement.Children.Add(match(Token_Class.Semicolon));

				return declaration_statement;
			}
			return null;
		}

		private Node Declaration_stat()
		{
			//============= i think Tmam =============
			if (InputPointer < TokenStream.Count)
			{
				Node dec_stat = new Node("Declaration_stat");

				// Declaration_stat ->identifiers , Declaration_stat | Assignment , Declaration_stat | identifiers | Assignment

				 if (TokenStream[InputPointer].token_type == Token_Class.Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp)
				{
					dec_stat.Children.Add(Assignment());
					if (TokenStream[InputPointer].token_type == Token_Class.Comma)
					{
						dec_stat.Children.Add(match(Token_Class.Comma));
						dec_stat.Children.Add(Declaration_stat());
					}
				}
				else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
				{
					dec_stat.Children.Add(match(Token_Class.Identifier));
					if (TokenStream[InputPointer].token_type == Token_Class.Comma)
					{
						dec_stat.Children.Add(match(Token_Class.Comma));
						dec_stat.Children.Add(Declaration_stat());
					}
				}

                return dec_stat;
			}
			return null;
		}
		private Node Assignment_Statement()
		{
			if (InputPointer < TokenStream.Count)
			{
				Node assignment_statement = new Node("Assignment_Statement");

				assignment_statement.Children.Add(Assignment());
				assignment_statement.Children.Add(match(Token_Class.Semicolon));
				return assignment_statement;
			}
			return null;

		}
		private Node Assignment()
		{
			//============= Tmam =============
			if (InputPointer < TokenStream.Count)
			{
				Node assignment = new Node("Assignment");

				//	Assignment -> identifier assignmentoperator Expression 
				assignment.Children.Add(match(Token_Class.Identifier));
				assignment.Children.Add(match(Token_Class.AssignOp));
				assignment.Children.Add(Expression());
				
				return assignment;
			}
			return null;
		}

		private Node Datatype()
		{
			//============= Tmam =============
			if (InputPointer < TokenStream.Count)
			{
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
				else if (TokenStream[InputPointer].token_type == Token_Class.Type_String)
				{
					datatype.Children.Add(match(Token_Class.Type_String));
				}

				return datatype;
			}
			return null;
		}

		private Node Function_Statement()
		{

			//============= Tmam =============
			if (InputPointer < TokenStream.Count)
			{
				Node function_Statement = new Node("Function_Statement");

				//Function_Declaration Function_Body 
				if (TokenStream[InputPointer].token_type == Token_Class.Integer ||
					TokenStream[InputPointer].token_type == Token_Class.Float ||
					TokenStream[InputPointer].token_type == Token_Class.Type_String)
				{
					function_Statement.Children.Add(Function_Declaration());
					function_Statement.Children.Add(Function_Body());
				}
				else
					return null;
				return function_Statement;
			}
			return null;

		}

		private Node Function_Declaration()
		{
			//============= Tmam =============
			if (InputPointer < TokenStream.Count)
			{
				Node function_declaration = new Node("Function_Declaration");

				//Datatype FunctionName (Function_Parameters) 
				function_declaration.Children.Add(Datatype());
				function_declaration.Children.Add(FunctionName());
				function_declaration.Children.Add(match(Token_Class.LeftParanthesis));
				function_declaration.Children.Add(Function_Parameters());
				function_declaration.Children.Add(match(Token_Class.RightParanthesis));

				return function_declaration;
			}
			return null;
		}
		bool first=true;
		private Node Function_Parameters()
		{
			//============= i think Tmam =============
			if (InputPointer < TokenStream.Count)
			{

				//   Function_Parameters -> Parameter ,  Function_Parameters | Parameter | ε
				//   Parameter -> Datatype identifier
				if (TokenStream[InputPointer].token_type == Token_Class.Integer ||
					TokenStream[InputPointer].token_type == Token_Class.Float ||
					TokenStream[InputPointer].token_type == Token_Class.Type_String)
				{
				    Node function_parameters = new Node("Function_Parameters");
					function_parameters.Children.Add(Parameter());
					if (TokenStream[InputPointer].token_type == Token_Class.Comma)
					{
						function_parameters.Children.Add(match(Token_Class.Comma));
						function_parameters.Children.Add(Function_Parameters());
					}
				    return function_parameters;
				}
				else
					return null;

			}
			return null;
		}

		private Node Parameter()
		{
			//=========== Tmam ==========
			if (InputPointer < TokenStream.Count)
			{
				Node parameter = new Node("Parameter");

				//   Parameter -> Datatype identifier
				parameter.Children.Add(Datatype());
				parameter.Children.Add(match(Token_Class.Identifier));

				return parameter;
			}
			return null;
		}

		private Node FunctionName()
		{
			//============= Tmam =============
			if (InputPointer < TokenStream.Count)
			{
				Node functionName = new Node("FunctionName");

				// identifier 
				functionName.Children.Add(match(Token_Class.Identifier));

				return functionName;
			}
			return null;
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
	}
}
