using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using uGCapture;

namespace GUI2 
{
    public partial class Form1 : Form
    {
        public delegate void UpdateComponent(Queue<String> myQ);
        public UpdateComponent [] updaters = new UpdateComponent[6];
        public int LOGGER = 0;
        //1 = status
        //2 = notify
        //3 = graph
        //4 = map
        //5 = values
        GuiDriver gd;
        bool active = false;
        Thread oThread;
        GuiReceiver loggerComp;
        public Form1(String config)
        {
            gd = new GuiDriver(this, "gd", config, true);
            InitializeComponent();
            gd.Startup_Init();
            button1.Text = "Start Capture";
            //oThread = new Thread(new ThreadStart(updateLogBox));
            this.FormClosing += Form1_FormClosing;
            updaters[0] = new UpdateComponent(UpdateLogger);
            loggerComp = new GuiReceiver(this, 0, "gd", true);
            loggerComp.Start();
        }

        private void UpdateLogger(Queue<string> myQ)
        {
            textBox1.AppendText(myQ.Dequeue());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (active)
            {
                    //Application.Exit();
            }
            else
            {
                gd.ToggleCapture();
                active = true;
                button1.Text = "Stop Capture";
            }
        }
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                // Display a MsgBox asking the user to save changes or abort. 
            if (MessageBox.Show("Do you really want to halt the data capture?", "Data Capture",
               MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                oThread.Abort();
                gd.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }
                // Updates the textbox text.
        private void UpdateText(string text)
        {
          // Set the textbox text.
          textBox1.Text += "\n\n\n" + text;
        }
        private void updateLogBox()
        {
            while (true)
            {
                
                if (gd.myQ.Count != 0)
                {
                    textBox1.Invoke((Action)delegate
                    {
                        lock (gd.myQ)
                        {
                            textBox1.Text = gd.myQ.Dequeue();
                        }

                    });
                }
            }

        }
    }



}
