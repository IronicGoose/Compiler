using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace WindowsFormsBYQ
{
    
    class Parser
    {
        Token token, T;
        Scanner scanner = null;
        Form1 f1;
        Stack<Token> numStack, chaStack;
        double originX, originY;
        double pX, pY;
        double rotate =0  ,scaleX =1,scaleY = 1 ;
        public Parser()
        {
            numStack = new Stack<Token>();
            chaStack = new Stack<Token>();
            scanner = new Scanner(); 
        }
        public void InitParser(Form1 f11)
        { 
            scanner.InitScanner();
            f1 = f11;
        }
        public void Program()
        {
            scanner.InitScanner();
            FetchToken();
            while(token.type != Token_Type.NONTOKEN)
            { 
                Statement();
                FetchToken();
            }
            ClearParser();
        }
        void Statement()
        {

            switch (token.type)
            {
                case Token_Type.ORIGIN: OriginStatement(); break;
                case Token_Type.SCALE: ScaleStatement(); break;
                case Token_Type.ROT: RotStatement(); break;
                case Token_Type.FOR: ForStatement(); break; 
            }
        }
        void ClearParser()
        {
            scanner.ClearScanner();
            rotate = 0;
            originX = 0; originY = 0;
            scaleX = 1;scaleY = 1;
            numStack.Clear();
            chaStack.Clear(); 
            
        }
        bool MatchToken(Token_Type tp)
        {
            return token.type == tp;
        }
        Token FetchToken()
        {
            return token = scanner.GetToken();
        }
        void OriginStatement()
        {
            FetchToken();
            MatchToken(Token_Type.IS);
            FetchToken();
            MatchToken(Token_Type.L_BRACKET);
            FetchToken();
            Houzhui();
            originX = numStack.Pop().value; 
            MatchToken(Token_Type.COMMA);
            FetchToken();
            Houzhui();
            originY = numStack.Pop().value; 
            MatchToken(Token_Type.R_BRACKET);
            
        }
        void ScaleStatement()
        {
            FetchToken();
            MatchToken(Token_Type.IS);
            FetchToken();
            MatchToken(Token_Type.L_BRACKET);
            FetchToken();
            Houzhui();
            scaleX = numStack.Pop().value;
            MatchToken(Token_Type.COMMA);
            FetchToken();
            Houzhui();
            scaleY = numStack.Pop().value;
            MatchToken(Token_Type.R_BRACKET);
        }
        void RotStatement()
        {
            FetchToken();
            MatchToken(Token_Type.IS); 
            FetchToken();
            Houzhui();
            rotate  = numStack.Pop().value; 

        }
        void ForStatement()
        {
            FetchToken();
            MatchToken(Token_Type.T);
            T = token;
            FetchToken();
            MatchToken(Token_Type.FROM);
            FetchToken();
            Houzhui();
            double f = numStack.Pop().value;
            MatchToken(Token_Type.TO);
            FetchToken();
            Houzhui();
            double t = numStack.Pop().value;
            MatchToken(Token_Type.STEP);
            FetchToken();
            Houzhui();
            double s = numStack.Pop().value;
            int fro = 0;
            fro = scanner.tokenNum;
            for (; f < t; f += s)
            {
                scanner.tokenNum = fro -1;
                T.value = f;
                FetchToken();
                if (MatchToken(Token_Type.DRAW))
                {
                    FetchToken();
                    MatchToken(Token_Type.L_BRACKET);
                    FetchToken();
                    Houzhui();
                    pX = numStack.Pop().value;
                    MatchToken(Token_Type.COMMA);
                    FetchToken();
                    Houzhui();
                    pY = numStack.Pop().value;
                    MatchToken(Token_Type.R_BRACKET);
                    Draw();
                } 
            }



        }
        public void Draw( )
        {
            double ptx, pty;
            pX = pX * scaleX;
            pY = pY * scaleY;
            ptx = pX *   Math.Cos(rotate) +pY* Math.Sin(rotate) ;
            pty = pY *   Math.Cos(rotate) - pX * Math.Sin(rotate);
            pX = ptx; pY = pty;
            f1.DrawPoint((originX+pX) , (originY+pY)  , scaleX,scaleY ); 
        }
        //读到非四则运算字符时，退出，会多读一个字符出来，进行完四则之后不能再读下一个。
        void Houzhui()
        {
            if( MatchToken( Token_Type.L_BRACKET) || MatchToken(Token_Type.PLUS)|| MatchToken(Token_Type.MINUS) )
            {
                chaStack.Push(token);
                FetchToken();
                Houzhui();
                return;
            }
            if( MatchToken(Token_Type.T))
            {
                numStack.Push(T);
                FetchToken();
                Houzhui();
                return;
            }
            if (MatchToken(Token_Type.CONST_ID))
            {
                numStack.Push(token);
                FetchToken();
                Houzhui();
                return; 
            }
            if(MatchToken(Token_Type.FUNC))
            {
                chaStack.Push(token);
                FetchToken();
                Houzhui();
                return;
            }
            if (MatchToken(Token_Type.R_BRACKET))
            {
                if (chaStack.Contains(scanner.TokenLib[0]))
                {
                    Token te = token;
                    while (te.type != Token_Type.L_BRACKET)
                    {

                        te = chaStack.Pop();
                        Cal(te);
                        if (te.type != Token_Type.L_BRACKET)
                            te = chaStack.Pop();
                    }
                    FetchToken();
                    Houzhui();
                    return;
                } 
            }
            if (MatchToken(Token_Type.MUL) || MatchToken(Token_Type.DIV))
            {
                double a, b;
                Token t = new Token();
                bool ff = false;
                t.type = token.type;
                a = numStack.Pop().value;  FetchToken();
                if (token.type != Token_Type.CONST_ID && token.type != Token_Type.T)
                {
                    Houzhui();
                    b = numStack.Pop().value;
                    ff = true;
                }
                else  b = token.value;
                if(t.type == Token_Type.DIV)
                    t.value = a / b;
                else
                {
                    t.value = a * b;
                }
                t.type = Token_Type.CONST_ID;
                numStack.Push(t);
                if(!ff)
                    FetchToken();
                Houzhui();
                return;
            }
            Token tttt;
            if (chaStack.Count!= 0)
            {
                tttt = chaStack.Pop(); 
                while (true)
                {
                    Cal(tttt);
                    if(chaStack.Count != 0)
                    {
                        tttt = chaStack.Pop(); 
                    }
                    else
                    {
                        break;
                    }
                }

            }
        }
        public void Cal(Token c )
        {  
            if(c.type == Token_Type.MINUS || c.type == Token_Type.PLUS)
            {
                Token n = numStack.Pop();
                Token n2 = numStack.Pop();
                
                Token t = new Token();
                t.type = Token_Type.CONST_ID;
                switch (c.type)
                {
                    case Token_Type.PLUS: t.value = n2.value + n.value; break;
                    case Token_Type.MINUS: t.value = n2.value - n.value; break;
                    default: break;
                }
                numStack.Push(t);

            }
            else if(c.type == Token_Type.FUNC)
            {  
                double tt = numStack.Pop().value;
                tt = c.funcPtr(tt);
                c = new Token();
                c.type = Token_Type.CONST_ID;
                c.value = tt;
                numStack.Push(c);  
            }
        }
    }
}
