﻿using System;
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
        private static int MAX_DATA_POINTS = 100;



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
            if(dataFrames.Count>0)
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

        public List<DataPoint> getDataPoints()
        {
            return dataFrames;
        }

       // public int getMaxDataPoints()
       // {
       //     return MAX_DATA_POINTS;
       // }
    }
}
