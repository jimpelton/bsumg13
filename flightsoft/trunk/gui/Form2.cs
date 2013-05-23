// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-23                                                                      
// ******************************************************************************


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uGCapture;
using Message = uGCapture.Message;

namespace gui
{
    public partial class Form2 : Form
    {
        private GuiMain guiMain;
        private GUIUpdater2 guiUpdater;
        private CaptureClass capture;
        private ConfigData loader;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string configPath)
        {
            InitializeComponent();
            ConfigLoader.LoadConfig(configPath);
            setupCASPanelDelegates();
            guiUpdater = new GUIUpdater2("Updater")
                {
                    DebugOutput = this.DebugOutput
                };
            
        }
        private void setupCASPanelDelegates()
        {
            biteCASPanelControl1.BroadcastLog = guiUpdater.BroadcastLogError;
            biteCASPanelControl1.BroadcastCmd = guiUpdater.BroadcastCmd;

        }
        public void UpdateContents(DataSet<byte> dat)
        {
            biteCASPanelControl1.UpdateContents(dat);
            imageDisplayControl1.UpdateContents(dat);
        }

        private void guiUpdateTimer_Tick(object sender, EventArgs e)
        {
            DataSet<byte> dat = capture.GetLastData();
            if (dat == null)
            {
                Console.Error.WriteLine("Data from capture.GetLastData() was null!");
                return;
            }
            UpdateContents(dat);
        }

        public void DebugOutput(Message s)
        {
            //if (rTB_Debug_Output.InvokeRequired)
            //{
            //    SetTextCallback a = new SetTextCallback(setDebugText);
            //    this.Invoke(a, new object[] { s, col });
            //}
            //else
            //{
            //    setDebugText(s, col);
            //}
        }

        private void setDebugText(string s, Color col)
        {
            //String s2 = GetTimestamp() + s + "\n";
            //rTB_Debug_Output.AppendText(s2);
            //rTB_Debug_Output.SelectionLength = rTB_Debug_Output.Text.Length;
            //rTB_Debug_Output.ScrollToCaret();
        }

    }
}