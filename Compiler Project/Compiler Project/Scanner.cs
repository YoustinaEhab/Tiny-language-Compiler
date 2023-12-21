using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
	main,
	Number,
    String, Integer, Float, Read, Write, Repeat, Until, IF, ElseIf, Else, Then, Return, Endl, End,
    Type_String,
    Comment,
    Identifier,
    PlusOp, MinusOp, MultiplyOp, DivideOp,
    EqualOp, LessThanOp, GreaterThanOp, NotEqualOp,
    AndOp, OrOp,
    LeftParanthesis, RightParanthesis,
    Semicolon, Comma,
    LeftBrace, RightBrace,
    AssignOp
}
namespace Tiny_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }
    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Arithmatic_Operator = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Condition_Operator = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Boolean_Operator = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner(){
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("end", Token_Class.End);

            ReservedWords.Add("string", Token_Class.Type_String);
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("float", Token_Class.Float);

            Arithmatic_Operator.Add("+", Token_Class.PlusOp);
            Arithmatic_Operator.Add("-", Token_Class.MinusOp);
            Arithmatic_Operator.Add("*", Token_Class.MultiplyOp);
            Arithmatic_Operator.Add("/", Token_Class.DivideOp);
            Arithmatic_Operator.Add(":=", Token_Class.AssignOp);

            Condition_Operator.Add("=", Token_Class.EqualOp);
            Condition_Operator.Add("<", Token_Class.LessThanOp);
            Condition_Operator.Add(">", Token_Class.GreaterThanOp);
            Condition_Operator.Add("<>", Token_Class.NotEqualOp);

            Boolean_Operator.Add("&&", Token_Class.AndOp);
            Boolean_Operator.Add("||", Token_Class.OrOp);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LeftParanthesis);
            Operators.Add(")", Token_Class.RightParanthesis);
            Operators.Add("{", Token_Class.LeftBrace);
            Operators.Add("}", Token_Class.RightBrace);

        }
        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = SourceCode[i].ToString();
                char nextchar = ' ';
                if (j != SourceCode.Length - 1)
                {
                    nextchar = SourceCode[j + 1];
                }

                if (CurrentChar == ' ' || CurrentChar == '\r') { 
                    continue;
                }
                else if(CurrentChar == '\n')
                {
                    break;
                }

                if (CurrentChar >= 'A' && CurrentChar <= 'z')
                {

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar >= 'A' && CurrentChar <= 'z')
                            CurrentLexeme += SourceCode[j];
                        else if (CurrentChar >= '0' && CurrentChar <= '9')
                            CurrentLexeme += SourceCode[j];
                        else
                            break;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if ((CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '.')
                            CurrentLexeme += SourceCode[j];
                        else
                            break;
                    }
                    FindTokenClass(CurrentLexeme);

                    i = j - 1;
                }
                else if (CurrentChar == '&' && nextchar == '&' || 
                    CurrentChar == '|' && nextchar == '|' || 
                    CurrentChar == '<' && nextchar == '>' ||
                    CurrentChar == ':' && nextchar == '=')
                {
                    CurrentLexeme = CurrentChar + nextchar.ToString();
                    i++;
                    FindTokenClass(CurrentLexeme);
                }

                
                else if (CurrentChar == '/' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '*')
                {

                    j = i + 2;
                    while (j < SourceCode.Length - 1 && !(SourceCode[j - 1] == '*' && SourceCode[j] == '/'))
                    {
                        j++;
                        
                    }
                    if (SourceCode[j - 1] != '*' && SourceCode[j] != '/')
                    {
                        String incompleteComment = SourceCode.Substring(i);
                        Errors.Error_List.Add(incompleteComment);
                        break;
                    }

                    else
                    {
                        i = j + 1;
                        continue;
                    }
                     
                }
                
                else if (CurrentChar == '"')
                {
                    j = i + 1;
                    while (j < SourceCode.Length-1 && SourceCode[j] != '"')
                    {
                        CurrentLexeme += SourceCode[j];

                     
                        j++;
                    }
                    CurrentLexeme += SourceCode[j];
                    if (SourceCode[j] != '"') {
                        Errors.Error_List.Add(CurrentLexeme);
                        break;
                    }
                    else {
                        Token Tok = new Token();
                        Tok.lex = CurrentLexeme;
                        Tok.token_type = Token_Class.String;
                        Tokens.Add(Tok);

                        i = j + 1;
                        continue;
                    }
                    
                }
                else
                {
                    FindTokenClass(CurrentLexeme);

                }
            }

                Tiny_Compiler.TokenStream = Tokens;
            }
            void FindTokenClass(string Lex)
            {
                Token Tok = new Token();
                Tok.lex = Lex;

            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);

            }
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);

            }
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);

            }
            else if (Arithmatic_Operator.ContainsKey(Lex))
            {
                Tok.token_type = Arithmatic_Operator[Lex];
                Tokens.Add(Tok);

            }
            else if (Condition_Operator.ContainsKey(Lex))
            {
                Tok.token_type = Condition_Operator[Lex];
                Tokens.Add(Tok);

            }
            else if (Boolean_Operator.ContainsKey(Lex))
            {
                Tok.token_type = Boolean_Operator[Lex];
                Tokens.Add(Tok);

            }
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);

            }
            // else if (isComment(Lex))
            // {
            //   return;
            //}
            // else if (isString(Lex))
            // {
            //   Tok.token_type = Token_Class.String;
            // Tokens.Add(Tok);
            //}
            else
            {
                Errors.Error_List.Add(Lex);
            }
        }
        bool isIdentifier(string lex)
        {
             bool isValid = true;
             var s = new Regex("^[a-zA-Z]([a-zA-Z0-9])*$");
             if (!s.IsMatch(lex))
             {
              isValid = false;
             }
             return isValid;
        }
        bool isNumber(string lex)
        {
            bool isValid = true;
            var s = new Regex("^([-+]?[0-9]+(\\.[0-9]+)?([eE][-+]?[0-9]+)?)?$");
            if (!s.IsMatch(lex))
            {
                isValid = false;
            }

            return isValid;
        }      
       // public bool IsComment(string lex)
       // {
          //  Regex reg3 = new Regex(@"^(/\[\s\S]?\*/)$", RegexOptions.Compiled);
           // return reg3.IsMatch(lex);
        //}
        //public bool isString(string lex)
        //{
          //  Regex reg4 = new Regex("\"([^\"]*)\"", RegexOptions.Compiled);
           // return reg4.IsMatch(lex);
        //}

    }
}
