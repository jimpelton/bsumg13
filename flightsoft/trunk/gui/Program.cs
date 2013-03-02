using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace gui
{
    class Program
    {
        public static Program prg = null;
        public static capture.CaptureClass Bottom = null;
        public static bool boolCapturing = false;
       /// <summary>
       /// The main entry point for the application.
       /// </summary>
       [STAThread]
       static void Main()
       {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);           
            Application.Run(new Form1());//Form1 loops back and calls Program's constructor and fills in static prg.          
       }

       public static void DebugOutput(String s, int severity=0)
       {
            Form1.DebugOutput(s, severity);
       }

       public static void DebugOutput(String s, System.Drawing.Color col)
       {
           Form1.DebugOutput(s, col);
       }


       public void Startup_Init()
       {
           if (Bottom==null)
           {
               DebugOutput("Creating Capture Class");
               Bottom = new capture.CaptureClass();
           }
       }

       public void ToggleCapture()
       {       
           if (!boolCapturing)
           {
               
           }
       }

       public Program()
       {
           Startup_Init();
       }

    }
}
