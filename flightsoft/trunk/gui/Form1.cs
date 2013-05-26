﻿// ******************************************************************************
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
        private delegate void SetTextCallback(string s);
        private ImageDisplay guiImageDisplay = null;
        private BiteCASPanel CAS = null;

        public Form1(String config)
        {
            InitializeComponent();
            var childForm = new BiteCASPanel() { TopLevel = false, Visible = true };
            var infoControl = new InformationPanel();
            var guiimagedisp = new ImageDisplay() { TopLevel = false, Visible = true };
            this.Controls.Add(childForm);
            this.Controls.Add(infoControl);
            this.Controls.Add(guiimagedisp);
            
            
            guiimagedisp.SetBounds(0, 0, 1100, 400);
            infoControl.SetBounds(0, 410, 1100, 350);
            childForm.SetBounds(0, 770, 1100, 290);
            CAS = childForm;
            guiImageDisplay = guiimagedisp;

            guiMain = new GuiMain(this, "GuiMain", config);
            guiMain.guiCAS = CAS;
            guiMain.guiImageDisplay = guiimagedisp;
            guiMain.Startup_Init();
            guiUpdater = guiMain.GuiUpdater;

            //TODO: this updating should be handled externally to the form.
            //TODO: make an alternative thread-safe way to do this.
            //it likes it when the timer is in here...
            DebugUpdateTimer.Tick += new EventHandler(guiUpdater.UpdateGUI);         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DebugOutput("Gui Initialized...");
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

        public void DebugOutput(String s)
        {
            if (rTB_Debug_Output.InvokeRequired)
            {
                SetTextCallback a = setDebugText;
                this.Invoke(a, new object[] { s });
            }
            else
            {
                setDebugText(s);
            }
        }

        //public void DebugOutput(String s, Color col)
        //{

        //    if (rTB_Debug_Output.InvokeRequired)
        //    {
        //        SetTextCallback a = new SetTextCallback(setDebugText);
        //        this.Invoke(a, new object[] { s, col });
        //    }
        //    else
        //    {
        //        setDebugText(s, col);
        //    }
            
        //}

        private void setDebugText(string s)
        {           
            rTB_Debug_Output.AppendText(s);
            //rTB_Debug_Output.SelectionLength = rTB_Debug_Output.Text.Length;
            //rTB_Debug_Output.ScrollToCaret();
        }

        private String GetTimestamp()
        {
            return DateTime.Now.ToString("dd:HH:mm:ss ");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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
