using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsBYQ
{
    public partial class Form1 : Form
    {
        Scanner scanner;
        Parser parser;
        public Form1()
        {
            parser = new Parser();
            parser.InitParser(this);
            InitializeComponent();
         //   DrawIt();
        } 
        public  void DrawIt()
        {
            System.Drawing.Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Red,3);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.DrawLine(myPen, 0, 0, 200, 200); 
        }
        public void DrawPoint(double x,double y,double scaleX,double scaleY)
        {
            System.Drawing.Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            Brush aBrush = (Brush)Brushes.Black;
            formGraphics.FillRectangle(aBrush,(float) x,(float) y, (float)scaleX,(float) scaleY);
        }
        private void Form1_Load(object sender, EventArgs e)
        { 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.Clear(Color.YellowGreen);
            parser.Program();
            //      DrawIt(); 
        }
    }
}
