using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


 
namespace WindowsFormsBYQ
{
    enum Token_Type
    {
        ORIGIN, SCALE, ROT, IS,
        TO, STEP, DRAW, FOR, FROM,
        T,
        SEMICO, L_BRACKET, R_BRACKET, COMMA,
        PLUS, MINUS, MUL, DIV, POWER,
        FUNC,
        CONST_ID,
        NONTOKEN,
        ERRTOKEN
    };
    delegate double FuncPtr(double t);
   
    class Token
    {
        public Token_Type type;
        public string lexico;
        public double value;
        public FuncPtr funcPtr;
        public  Token(Token_Type tp, string le,double d , FuncPtr fp)
        { 
            type = tp; lexico = le; value = d; funcPtr = fp;
        }
        public Token()
        {
            funcPtr = null;
            value = 0;

        } 
    }
    class Scanner
    {
        public List<Token> TokenLib,tokenList;
        char[] keyChars = {'(',')',';' ,'\n','+','-','*','/',','};
        public Scanner()
        {
            tokenString = new List<string>();
            tokenList = new List<Token>();

            TokenLib = new List<Token>();
            TokenLib.Add(new Token(Token_Type.L_BRACKET,
               null, 0, null));
            TokenLib.Add(new Token(Token_Type.R_BRACKET,
                null, 0, null));
            TokenLib.Add(new Token(Token_Type.CONST_ID, 
                "PI" , Math.PI, null));
            TokenLib.Add(new Token(Token_Type.CONST_ID,"E", 2.71828, null));
            TokenLib.Add(new Token(Token_Type.T, "T" , 0.0, null));
            TokenLib.Add(new Token(Token_Type.FUNC, "SIN", 0.0,Math.Sin));
            TokenLib.Add(new Token(Token_Type.FUNC, "COS", 0.0, Math.Cos));
            TokenLib.Add(new Token(Token_Type.FUNC, "TAN", 0.0, Math.Tan));
            TokenLib.Add(new Token(Token_Type.FUNC, "LN", 0.0, Math.Log));
            TokenLib.Add(new Token(Token_Type.FUNC, "EXP", 0.0, Math.Exp));
            TokenLib.Add(new Token(Token_Type.FUNC, "SQET", 0.0,Math.Sqrt));
            TokenLib.Add(new Token(Token_Type.
                ORIGIN, "ORIGIN", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                SCALE, "SCALE", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                ROT, "ROT", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                IS, "IS", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                FOR, "FOR", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                FROM, "FROM", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                TO, "TO", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                STEP, "STEP", 0.0, null));
            TokenLib.Add(new Token(Token_Type.
                DRAW, "DRAW", 0.0, null));
        }
        List<string> tokenString;
        public int tokenNum = 0; 
        public bool InitScanner( ) {
            ClearScanner(); 
            FileStream fs = new FileStream("C:/Users/Chenzhida/documents/visual studio 2015/Projects/WindowsFormsBYQ/WindowsFormsBYQ/Test.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs); 
            if(sr!= null)
            {
                char[] paras = { ' ' , '\t', '\r','\0'};
                string sm;sm = sr.ReadToEnd();
                sr.Dispose();
                string[] TokenBuffer = sm.Split(paras);
                DealInput(TokenBuffer);
                sr.Close();
                fs.Close();
                return true;
            }
            return false;
        } 
        public void DealIfKeyChar(char i,string te,int head = 0)
        { 
            char[] paras = { ' ', '\t', '\r', '\0' ,'/'};
            bool isCommand = false;
            int inum = 0;
            if (i == '/')
            {
                int it = te.IndexOf('/');
                if (it+1 != te.Length && te[it + 1] == '/')
                {
                    inum = tokenString.Count;
                    isCommand = true;
                }
            }
            if (te.IndexOf(i) == 0 )
            {
                head = 0;
            }
            else if (te.IndexOf(i) == te.Length -1)
            {
                head = 2;
            }
            else
            {
                head = 1;
            }
            StringBuilder sb = new StringBuilder(te);
            sb[te.IndexOf(i)] = ' ';
            if (isCommand)
                sb[te.IndexOf(i) + 1] = ' ';
            te = sb.ToString() ;
            //string[] temp = te.Replace(i, ' ').Trim().Split(paras);
            string[] temp = te.Trim(' ').Split(paras);
            string mm;
            if (head == 0)
            {
                tokenString.Add(i.ToString());
                if(i == '/' && isCommand)
                    tokenString.Add(i.ToString());
            }
            for (int ii = 0; ii < temp.Length;ii++)
            {
                mm = temp[ii];
                if (head == 1 && ii != 0)
                    tokenString.Add(i.ToString());
                if (mm != "" )
                {
                    int tem = mm.IndexOfAny(keyChars);
                    if (tem != -1  && mm.Trim() != i.ToString()) 
                    {
                        DealString(mm); 
                    }
                    else if (tem == -1)
                    {
                        tokenString.Add(mm); 
                        
                    }
                }
            }
            if (head == 2)
            {
                tokenString.Add(i.ToString());
                inum = tokenString.Count - 1;

            } 
    //        if (head == 2 && temp.Length != 1)
   //             tokenString.RemoveAt(inum);
        }
      
        public void DealInput(string[] TokenBuffer)
        {
            int curLen = 0,emptyNum = 0;
            bool isEmpty = false;
            for (int i = 0; i < TokenBuffer.Length;i++)
            {
                if (isEmpty)
                {
                    emptyNum++;
                    TokenBuffer[i - emptyNum] = TokenBuffer[i];
                    isEmpty = false; 
                }
                 if(TokenBuffer[i] == "" )
                {
                    isEmpty = true;
                }
                if (!isEmpty)
                {
                    curLen++;
                    TokenBuffer[i - emptyNum] = TokenBuffer[i];
                } 
            }
            string[] nstrings = new string[curLen];
            for(int i = 0; i < curLen; i++)
            {
                nstrings[i] = TokenBuffer[i];
                DealString(nstrings[i]);
            }
            DealCommand();
            TokenBuffer = nstrings;
            MakeTokens();
        }
        public void DealCommand()
        {
            for(int i = 0;i < tokenString.Count;i++  )
            {
                if(tokenString[i] == "/" && tokenString[i+1] == "/")
                {
                    while (tokenString[i] != "\n")
                    {
                        tokenString.RemoveAt(i);
                        if (i >= tokenString.Count)
                            break;
                    }
                    if (i >= tokenString.Count)
                        break;
                }
                if (tokenString[i] == "\n")
                { 
                    tokenString.RemoveAt(i);
                    i--;
                }
            }
        }
        public void MakeTokens()
        {
            Token tokenMaked = new Token();
            tokenMaked.type = Token_Type.NONTOKEN;  
            foreach(var i in tokenString) {
                switch (i.ToUpper())
                {
                    case "(":
                        {
                            tokenMaked = TokenLib[0];
                            tokenList.Add(tokenMaked);
                        }   break;
                    case ")":
                        {
                            tokenMaked = TokenLib[1];
                            tokenList.Add(tokenMaked);
                        } break;
                    case "+":
                        {
                            tokenMaked = new Token(Token_Type.PLUS, null, 0, null);
                            tokenList.Add(tokenMaked);
                        } break;
                    case "-":
                        {
                            tokenMaked = new Token(Token_Type.MINUS, null, 0, null);
                            tokenList.Add(tokenMaked);
                        } break;
                    case "*":
                        {
                            tokenMaked = new Token(Token_Type.MUL, null, 0, null);
                            tokenList.Add(tokenMaked);
                        } break;
                    case "/":
                        {
                            tokenMaked = new Token(Token_Type.DIV, null, 0, null);
                            tokenList.Add(tokenMaked);
                        } break;
                    case ",":
                        {
                            tokenMaked = new Token(Token_Type.COMMA, null, 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case ";":
                        {
                            tokenMaked = new Token(Token_Type.SEMICO, null, 0, null);
                            tokenList.Add(tokenMaked);
                        } break;
                    case "ROT":
                        {
                            tokenMaked = new Token(Token_Type.ROT, "ROTATE", 0, null);
                            tokenList.Add(tokenMaked);
                        }break;
                    case "FOR":
                        {
                            tokenMaked = new Token(Token_Type.FOR, "FOR", 0, null);
                            tokenList.Add(tokenMaked);
                        }break;
                    case "T":
                        {
                            tokenMaked = new Token(Token_Type.T, "T", 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "FROM":
                        {
                            tokenMaked = new Token(Token_Type.FROM, "FROM", 0, null);
                            tokenList.Add(tokenMaked);
                        }break;
                    case "TO":
                        {
                            tokenMaked = new Token(Token_Type.TO, "TO", 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "STEP":
                        {
                            tokenMaked = new Token(Token_Type.STEP, "STEP", 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "IS":
                        {
                            tokenMaked = new Token(Token_Type.IS, "IS", 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "ORIGIN":
                        {
                            tokenMaked = new Token(Token_Type.ORIGIN, "ORIGIN", 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "SCALE":
                        {
                            tokenMaked = new Token(Token_Type.SCALE, "SCALE", 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "COS":
                        {
                            tokenMaked = new Token(Token_Type.FUNC, "COS", 0, Math.Cos);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "SIN":
                        {
                            tokenMaked = new Token(Token_Type.FUNC, "SIN", 0, Math.Sin);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "DRAW":
                        {
                            tokenMaked = new Token(Token_Type.DRAW, "DRAW", 0, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    case "PI":
                        {
                            tokenMaked = new Token(Token_Type.CONST_ID, "PI", Math.PI, null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                    default:
                        {
                            double ttt = 0;
                            double.TryParse(i, out ttt);
                            tokenMaked = new Token(Token_Type.CONST_ID, null,ttt , null);
                            tokenList.Add(tokenMaked);
                        }
                        break;
                }

            } 
        }
        public void DealString(string nstrings)
        {
            if (nstrings != "//" && nstrings.IndexOf('/') != -1)
            {
                DealIfKeyChar('/', nstrings );
                return;
            }
            if (nstrings != "\n" && nstrings.IndexOf('\n') != -1)
            {
                DealIfKeyChar('\n', nstrings, 1);
                return;
            }
            if (nstrings != ";" && nstrings .IndexOf(';') != -1)
            {
                DealIfKeyChar(';', nstrings  , 1);
                return;
            }
            if (nstrings  != "(" && nstrings .IndexOf('(') != -1)
            {
                DealIfKeyChar('(', nstrings );
                return;
            }
            if (nstrings != "+" && nstrings.IndexOf('+') != -1)
            {
                DealIfKeyChar('+', nstrings, 2);
                return;
            }
            if (nstrings != "-" && nstrings.IndexOf('-') != -1)
            {
                DealIfKeyChar('-', nstrings, 2);
                return;
            }
            if (nstrings != "*" && nstrings.IndexOf('*') != -1)
            {
                DealIfKeyChar('*', nstrings, 2);
                return;
            }
            if (nstrings != "/" && nstrings.IndexOf('/') != -1)
            {
                DealIfKeyChar('/', nstrings, 2);
                return;
            }
            if (nstrings != "," && nstrings.IndexOf(',') != -1)
            {
                DealIfKeyChar(',', nstrings, 2);
                return;
            }
            if (nstrings != ")" && nstrings.IndexOf(')') != -1)
            {
                DealIfKeyChar(')', nstrings, 1);
                return;
            }
            tokenString.Add(nstrings);

        }
        public Token GetToken()
        {
            if(tokenNum  <  tokenList.Count)
                return tokenList[tokenNum++];
            Token t = new Token();
            t.type = Token_Type.NONTOKEN;
            return t;
        }
        public void ClearScanner()
        {
            tokenString.Clear();
            tokenNum = 0;
            tokenList.Clear();
        }
    }
}
