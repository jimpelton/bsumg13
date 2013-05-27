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
        private ConfigData configData;
        private bool startButtonIsCaptureToggle;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string configPath)
        {
            InitializeComponent();
            configData = ConfigLoader.LoadConfig(configPath);
            guiUpdater = new GUIUpdater2("Updater")
                {
                    DebugOutput = this.DebugOutput
                };
            
            setupCASPanelDelegates();
            initCaptureClass(configData);
        }
        private void setupCASPanelDelegates()
        {
            biteCASPanelControl1.BroadcastLog = guiUpdater.BroadcastLogError;
            biteCASPanelControl1.BroadcastCmd = guiUpdater.BroadcastCmd;
        }

        public void UpdateContents(DataSet<byte> dat)
        {
            biteCASPanelControl1.UpdateContents(dat);
            imageDisplayControl.UpdateContents(dat);
        }




        public void DebugOutput(Message s)
        {
            logMessagesControl.AppendText(timestamp() + s.ToString());
        }

        private String timestamp()
        {
            return DateTime.Now.ToString("yyyy MM dd HH:mm:ss.fff ");
        }

        private void initCaptureClass(ConfigData configData)
        {
            guiUpdater.init();
            capture = new CaptureClass(this.Handle, "CaptureClass")
                {
                    StorageDir = configData.Path
                };

            capture.init();


        }
        
        private void stopCapture()
        {
            capture.StopCapture();
        }

        private void startCaptureButton_Click(object sender, EventArgs e)
        {
            if (startButtonIsCaptureToggle == false)
            {
                startCaptureButton.Text = "Starting Up...";
                initCaptureClass(configData);
                startCaptureButton.Text = "Stop Data Capture";

            }
            else
            {
                startCaptureButton.Text = "Stopping capture...";
                stopCapture();
                startCaptureButton.Text = "Start Capture.";
            }
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

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            capture.StopCapture();
            capture.Shutdown();
        }

    }
}