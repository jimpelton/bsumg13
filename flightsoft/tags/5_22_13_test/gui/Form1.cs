// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-19                                                                      
// ******************************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

namespace gui
{
    public partial class Form1 : Form
    {
        private GuiMain guiMain=null;
        private GuiUpdater guiUpdater=null;
        private delegate void SetTextCallback(string s, Color col);
        private ImageDisplay guiImageDisplay = null;
        private BiteCASPanel CAS = null;

        public Form1(String config)
        {
            InitializeComponent();
            var childForm = new BiteCASPanel() { TopLevel = false, Visible = true };
            var guiimagedisp = new ImageDisplay() { TopLevel = false, Visible = true };
            Tab_Control_Main.TabPages[0].Controls.Add(childForm);
            Tab_Control_Main.TabPages[0].Controls.Add(guiimagedisp);
            childForm.SetBounds(0, 700, childForm.Width, childForm.Height);
            guiimagedisp.SetBounds(0, 250, guiimagedisp.Width, guiimagedisp.Height);
            CAS = childForm;
            guiImageDisplay = guiimagedisp;

            guiMain = new GuiMain(this, "GuiMain", config);
            guiMain.guiCAS = CAS;
            guiMain.guiImageDisplay = guiimagedisp;
            guiMain.Startup_Init();
            guiUpdater = guiMain.GuiUpdater;

            Tab_Control_Main.SelectedIndex=4;
            //TODO: this updating should be handled externally to the form.
            //TODO: make an alternative thread-safe way to do this.
            //it likes it when the timer is in here...
            DebugUpdateTimer.Tick += new EventHandler(guiUpdater.UpdateGUI);
            Tab_Control_Main.SelectedIndexChanged+=Tab_Control_Main_SelectedIndexChanged;

        }

        void Tab_Control_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Tab_Control_Main.SelectedIndex == 0)
            {
                Tab_Control_Main.TabPages[2].Controls.Remove(CAS);
                Tab_Control_Main.TabPages[0].Controls.Add(CAS);
                CAS.SetBounds(0, 700, CAS.Width, CAS.Height);
            }
            else
            {
                Tab_Control_Main.TabPages[0].Controls.Remove(CAS);
                Tab_Control_Main.TabPages[2].Controls.Add(CAS);
                CAS.SetBounds(0, 700, CAS.Width, CAS.Height);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DebugOutput("Gui Initialized...", 1);
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
            rTB_Debug_Output.AppendText(s2);
            //rTB_Debug_Output.SelectionLength = rTB_Debug_Output.Text.Length;
            //rTB_Debug_Output.ScrollToCaret();
        }


        private String GetTimestamp()
        {
            return DateTime.Now.ToString("dd:HH:mm:ss ");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            guiMain.Shutdown();
        }

        private void b_Execute_BITE_Test_Click(object sender, EventArgs e)
        {
            if (guiMain != null)
                guiMain.executeBiteTest();
        }

        private void TabPage_Capture_Click(object sender, EventArgs e)
        {

        }

    }
}
