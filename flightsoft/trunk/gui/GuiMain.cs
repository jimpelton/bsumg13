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
        public GuiUpdater guiUpdater;
        public List<DataPoint> dataFrames = null;



        private Form1 mainForm;
        public Form1 MainForm
        {
            get { return mainForm; }
        }

        public GuiMain(Form1 mainForm)
        {
            this.mainForm = mainForm;
        }

       public void Startup_Init()
       {
           dataFrames = new List<DataPoint>();
           guiUpdater = new GuiUpdater(mainForm,this);
           captureClass = new CaptureClass();
           captureClass.init();
       }

       public bool ToggleCapture()
       {

           if (!boolCapturing)
           {
               boolCapturing = true;
               dp.Broadcast(new SetCaptureStateMessage(this, boolCapturing));
               return boolCapturing;
           }
           //if we are capturing bring up a dialog and ask if we really want to stop.
           if (boolCapturing)
           {
               DialogResult dr = MessageBox.Show("Stop Capture?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
               if (dr == DialogResult.Yes)
                   boolCapturing = false;
               //now we stop capture.
               dp.Broadcast(new SetCaptureStateMessage(this, boolCapturing));
                
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
            if(dataFrames.Count>0)
            {
                //we are adding to the end of the list so the previous data point will be last.
                dataFrames.Last().image405 = null;
                dataFrames.Last().image485 = null;
            }
            //and place us at last.
            dataFrames.Add(p);
        }
    }
}
