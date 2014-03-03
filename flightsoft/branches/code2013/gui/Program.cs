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
           {
               if (args.Length >= 2)
               {
                   if (args[1].Equals("2"))
                   {
                       Application.Run(new Form2(args[0]));
                   }
                   else
                   {
                       Application.Run(new Form2("Config.ini"));
                   }
               }
               else
               {
                   Application.Run(new Form1(args[0]));
               }
           }
           else
           {
               Application.Run(new Form1("Config.ini"));
           } 
       }
    }
}
