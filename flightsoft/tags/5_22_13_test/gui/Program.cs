using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace gui
{
    class Program
    {
       // The main entry point for the application.
       [STAThread]
       static void Main(String[] args)
       {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
             if (args.Length > 0)        
                Application.Run(new Form1(args[0]));          
             else
                Application.Run(new Form1("Config.ini"));
       }
    }
}
