using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uGCapture;

namespace gui
{
    public class GuiMain : Receiver
    {
        private CaptureClass captureClass;
        private bool boolCapturing = false;

        //TODO:        private GuiUpdater guiUpdater;

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
           captureClass = new CaptureClass();
       }

       public void ToggleCapture()
       {
           if (!boolCapturing)
           {

           }
       }

       public void DebugOutput(String s, int severity = 0)
       {
           mainForm.DebugOutput(s, severity);
       }

       public void DebugOutput(String s, System.Drawing.Color col)
       {
           mainForm.DebugOutput(s, col);
       }

    }
}
