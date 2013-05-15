using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using uGCapture;

namespace gui
{
    public class GuiMain : Receiver
    {
        private CaptureClass captureClass;
        private bool boolCapturing = false;
        private static int MAX_DATA_POINTS = 100;
        private string configPath;
        private GuiUpdater guiUpdater;
       
        public gui.GuiUpdater GuiUpdater
        {
            get { return guiUpdater; }
            set { guiUpdater = value; }
        }

        private gui.BiteCASPanel CAS;
        public gui.BiteCASPanel guiCAS
        {
            get { return CAS; }
            set { CAS = value; }
        }

        private Form1 mainForm;
        public Form1 MainForm
        {
            get { return mainForm; }
        }

        private List<DataPoint> dataFrames;
        public List<DataPoint> DataFrames
        {
            get { return dataFrames; }
            set { dataFrames = value; }
        }

        public GuiMain(Form1 mainForm, string id, String cPath, bool receiving = true)
            : base(id, receiving)
        {
            this.configPath = cPath;
            this.mainForm = mainForm;
        }

        public void Startup_Init()
        {
            ConfigData config = ConfigLoader.LoadConfig(configPath);

            string path = config.Path.Trim();
            if (!path.EndsWith(@"\"))
            {
                path = path + @"\";
            }
            String directoryName = DateTime.Now.ToString("yyyy_MM_dd_HHmm");
            System.IO.Directory.CreateDirectory(config.Path + directoryName);

            dataFrames = new List<DataPoint>();
            guiUpdater = new GuiUpdater(mainForm, this, CAS, "GuiUpdater");
            captureClass = new CaptureClass("CaptureClass")
            {
                StorageDir = path + directoryName
            };

            captureClass.init();
        }

        public bool ToggleCapture()
        {

            if (!boolCapturing)
            {
                boolCapturing = true;
                dp.Broadcast(new SetCaptureStateMessage(this, true));
            }
            else
            {
                DialogResult dr = MessageBox.Show("Stop Capture?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    boolCapturing = false;
                    dp.Broadcast(new SetCaptureStateMessage(this, false));
                }

            }
            return boolCapturing;
        }

        public void DebugOutput(String s, int severity = 0)
        {
            mainForm.DebugOutput(s, severity);
        }

        public void DebugOutput(String s, System.Drawing.Color col)
        {
            mainForm.DebugOutput(s, col);
        }

        public void insertDataPoint(DataPoint p)
        {
            if (dataFrames.Count > 0)
            {
                //we are adding to the end of the list so the previous data point will be last.
                dataFrames.Last().image405 = null;
                dataFrames.Last().image485 = null;
            }
            //and place us at last.
            dataFrames.Add(p);
            if (dataFrames.Count > MAX_DATA_POINTS)
                dataFrames.RemoveAt(0);
        }

        /// <summary>
        /// Broadcasts the ReceiverCleanupMessage() from the Guimain instance.
        /// </summary>
        public void Shutdown()
        {
            Dispatch.Instance().Broadcast(new ReceiverCleanupMessage(this));
        }
        // public int getMaxDataPoints()
        // {
        //     return MAX_DATA_POINTS;
        // }

        public DataSet<byte> getLatestData()
        {
            return captureClass.GetLastData();
        }


    }
}
