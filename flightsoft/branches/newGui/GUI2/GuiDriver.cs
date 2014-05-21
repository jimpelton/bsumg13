using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uGCapture;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
namespace GUI2
{
    class GuiDriver : Receiver
    {
        Form mainForm;
        string configPath;
        ConfigData config;
        public Queue<String> myQ = new Queue<String>();
        private CaptureClass captureClass;
        private bool boolCapturing;
        private string dataPath;
        public string guiDataPath
        {
            get { return dataPath; }
            set { dataPath = value; }
        }

        // Creates a synchronized wrapper around the Queue.

        //private CaptureClass cc = new CaptureClass();
        public GuiDriver(Form mainForm, string id, string config, bool receiving = true)
            : base(id, receiving)
        {
            this.mainForm = mainForm;

            this.configPath = config;
            dp.Register(this);
        }

        public void Startup_Init()
        {
            config = ConfigLoader.LoadConfig(configPath);

            string path = config.Path.Trim();
            if (!path.EndsWith(@"\"))
            {
                path = path + @"\";
            }
            //guiDataPath = path;
            String directoryName = DateTime.Now.ToString("yyyy_MM_dd_HHmm");

            captureClass = new CaptureClass(mainForm.Handle, "CaptureClass") { StorageDir = path + directoryName };
            //dp.StartAllExecuting();
            dp.BroadcastLog(this, "Begin config: " + config.Path + "End config", 1);
        }

        public void StartCapture()
        {
            dp.Broadcast(new SetCaptureStateMessage(this, true));
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
                     //dp.Broadcast(new SetCaptureStateMessage(this, false));
                }

            }
            return boolCapturing;
        }


        /// <summary>
        /// Broadcasts the ReceiverCleanupMessage() from the Guimain instance.
        /// </summary>
        public void Shutdown()
        {
            //Dispatch.Instance().Broadcast(new ReceiverCleanupMessage(this));
            captureClass.Shutdown();

        }

        public DataSet<byte> getLatestData()
        {
            return captureClass.GetLastData();
        }

        public void switchToAlternateDrive()
        {
            ManagementObject bestDrive = null;

            ObjectQuery query = new ObjectQuery("Select * FROM  Win32_LogicalDisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            foreach (ManagementObject mo in collection)
            {
                if (bestDrive == null)
                    bestDrive = mo;

                if (mo["FreeSpace"] != null)
                {
                    ulong mySpace = (ulong)mo["FreeSpace"];
                    ulong theirSpace = (ulong)bestDrive["FreeSpace"];

                    if (mySpace > theirSpace)
                    {
                        if (((string)mo["VolumeName"]).ToLower().Contains("nasa"))
                        {
                            bestDrive = mo;
                        }
                    }
                }


            }
            if (bestDrive != null)
            {
                String directoryName = DateTime.Now.ToString("yyyy_MM_dd_HHmm");
                dataPath = bestDrive["Name"] + "\\Data\\" + directoryName;
                config.Path = dataPath;
                captureClass.switchToBackupDrive(bestDrive["Name"] + "\\Data\\" + directoryName);
            }
        }

        public override void exStatusMessage(Receiver r, uGCapture.Message m)
        {
            StatusMessage mes = m as StatusMessage;
            if (mes == null) return;
            lock (myQ)
            {
                myQ = new Queue<string>();
                myQ.Enqueue(mes.ToString());
            }
        }
        public override void exLogMessage(Receiver r, uGCapture.Message m)
        {
            LogMessage mes = m as LogMessage;
            if (mes == null) return;
            lock (myQ)
            {
                myQ.Enqueue(mes.ToString());
            }
        }
    }
}
