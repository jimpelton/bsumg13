using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using uGCapture;

namespace gui
{
    public class GuiMain : Receiver
    {
        private CaptureClass captureClass;
        private bool boolCapturing = false;
        private static int MAX_DATA_POINTS = 100;
        private string configPath;

        private string dataPath;
        public string guiDataPath
        {
            get { return dataPath; }
            set { dataPath = value; }
        }

        private GuiUpdater guiUpdater;     
        public GuiUpdater GuiUpdater
        {
            get { return guiUpdater; }
            set { guiUpdater = value; }
        }

        private BiteCASPanel CAS;
        public BiteCASPanel guiCAS
        {
            get { return CAS; }
            set { CAS = value; }
        }

        private ImageDisplay ImageDisplay;
        public ImageDisplay guiImageDisplay
        {
            get { return ImageDisplay; }
            set { ImageDisplay = value; }
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

        private ConfigData config;
        public ConfigData CurrentConfig
        {
            get { return config; }
            set { config = value; }
        }

        public GuiMain(Form1 mainForm, string id, String cPath, bool receiving = true)
            : base(id, receiving)
        {
            this.configPath = cPath;
            this.mainForm = mainForm;
        }

        public void Startup_Init()
        {
            config = ConfigLoader.LoadConfig(configPath);
            dp.BroadcastLog(this, "Begin config:\n" + config.Path + "End config\n", 1);

            string path = config.Path.Trim();
            if (!path.EndsWith(@"\"))
            {
                path = path + @"\";
            }
            guiDataPath = path;
            String directoryName = DateTime.Now.ToString("yyyy_MM_dd_HHmm");

            dataFrames = new List<DataPoint>();
            guiUpdater = new GuiUpdater(mainForm, this, CAS, "GuiUpdater");

            IntPtr hwnd;
            unsafe
            {
                hwnd = new IntPtr(mainForm.Handle.ToPointer());
            } 
            captureClass = new CaptureClass(hwnd, "CaptureClass") { StorageDir = path + directoryName };
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

        public DataSet<byte> getLatestData()
        {
            return captureClass.GetLastData();
        }

        public void switchToAlternateDrive()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            DriveInfo bestDrive = null;

            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    if (bestDrive == null)
                        bestDrive = drive;
                    if (drive.TotalFreeSpace > bestDrive.TotalFreeSpace)
                        bestDrive = drive;
                }
            }
            if (bestDrive != null)
            {
                dataPath = bestDrive.RootDirectory + "Data\\";
                config.Path = dataPath;
                captureClass.switchToBackupDrive(bestDrive.RootDirectory + "Data\\");               
            }
        }

        public void executeBiteTest()
        {
            dp.Broadcast(new BiteTestMessage(this));
        }

    }
}
