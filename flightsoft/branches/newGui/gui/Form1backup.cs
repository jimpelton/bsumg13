using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gui
{
    public partial class Form1 : Form
    {
        private static RichTextBox DebugBox = null;

        public static void DebugOutput(String s, int severity)
        {
            s += "\n";
            RichTextBox box = DebugBox;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;       
            if (severity > 0)
            {
                if(severity==1)
                    box.SelectionColor = Color.Blue;
                else if (severity == 2) 
                    box.SelectionColor = Color.BlueViolet;
                else if (severity == 3) 
                    box.SelectionColor = Color.Yellow;
                else if (severity == 4) 
                    box.SelectionColor = Color.Red;
                else if (severity > 4)
                    box.SelectionColor = Color.OrangeRed;

                box.AppendText(s);
                box.SelectionColor = box.ForeColor;
            }
        }

        public static void DebugOutput(String s, Color col)
        {
            s += "\n";
            RichTextBox box = DebugBox;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = col;
            box.AppendText(s);
            box.SelectionColor = box.ForeColor;           
        }

        public Form1()
        {
            InitializeComponent();
            DebugBox = rTB_Debug_Output;
            Tab_Control_Main.SelectedIndex=4;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.DebugOutput("Starting up...", 1);
        }

        private void rTB_Debug_Output_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_Go_Click(object sender, EventArgs e)
        {

        }

    }
}
