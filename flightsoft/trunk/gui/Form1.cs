using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using uGCapture;

namespace gui
{
    public partial class Form1 : Form
    {
        private GuiMain guiMain=null;
        private GuiUpdater guiUpdater=null;
        private delegate void SetTextCallback(string s, Color col);
        private ImageDisplay guiImageDisplay = null;

        public Form1()
        {
            InitializeComponent();
            guiMain = new GuiMain(this, "GuiMain");
            guiMain.Startup_Init();
            guiUpdater = guiMain.GuiUpdater;

            Tab_Control_Main.SelectedIndex=4;
            //TODO: this updating should be handled externally to the form.
            //TODO: make an alternative thread-safe way to do this.
            //it likes it when the timer is in here...
            DebugUpdateTimer.Tick += new EventHandler(guiUpdater.UpdateGUI);

            var childForm = new BiteCASPanel() { TopLevel = false, Visible = true };
            
            Tab_Control_Main.TabPages[2].Controls.Add(childForm);

            childForm.SetBounds(0, 400, this.Width - 6, this.Height - 6);
            
            
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
            bool capting = guiMain.ToggleCapture();           
            if(capting)
                btn_Go.Text = "Stop Capture";
            else
                btn_Go.Text = "Start Capture";
            
        }

        public void DebugOutput(String s, int severity)
        {
            DebugOutput(s, SeverityColor.GetColor(severity));
        }

        public void DebugOutput(String s, Color col)
        {

            if (rTB_Debug_Output.InvokeRequired)
            {
                SetTextCallback a = new SetTextCallback(setDebugText);
                this.Invoke(a, new object[] { s, col });
            }
            else
            {

                setDebugText(s, col);

            }
            
        }

        //TODO: Fix the colors!
        private void setDebugText(string s, Color col)
        {           
            String s2 = GetTimestamp() + s + "\n";
            //rTB_Debug_Output.SelectionStart = rTB_Debug_Output.TextLength;
            //rTB_Debug_Output.SelectionLength = 0;
            //rTB_Debug_Output.SelectionColor = col;
            rTB_Debug_Output.Text += s2;
            //rTB_Debug_Output.SelectionColor = rTB_Debug_Output.ForeColor;
        }


        private String GetTimestamp()
        {
            return DateTime.Now.ToString("dd:HH:mm:ss ");
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            guiMain.Shutdown();
        }

    }
}
