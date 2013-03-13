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
        private GuiMain guiMain;

        public Form1()
        {
            InitializeComponent();
            guiMain = new GuiMain(this);
            guiMain.Startup_Init();

            Tab_Control_Main.SelectedIndex=4;
            //TODO: this updating should be handled externally to the form.
            //DebugUpdateTimer.Tick += new EventHandler(grabCaptureDebugMessages);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DebugOutput("Starting up...", 1);
        }

        private void rTB_Debug_Output_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_Go_Click(object sender, EventArgs e)
        {
            
        }

        private void grabCaptureDebugMessages(object sender, EventArgs e)
        {
            //if (uGCapture.CaptureClass.DebugMessages.Count > 0)
            //{
            //    uGCapture.LogMessage l = uGCapture.CaptureClass.DebugMessages.Dequeue();
            //    DebugOutput(l.message, l.severity);
            //}
        }
        public void DebugOutput(String s, int severity)
        {
            DebugOutput(s, SeverityColor.GetColor(severity));
        }

        public void DebugOutput(String s, Color col)
        {
            String s2 = GetTimestamp() + s + "\n";
            RichTextBox box = rTB_Debug_Output;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = col;
            box.AppendText(s2);
            box.SelectionColor = box.ForeColor;           
        }

        private String GetTimestamp()
        {
            return DateTime.Now.ToString("dd:HH:mm:ss ");
        }    
    }
}
