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
    public delegate void UpdateTextCallback(string text);
    public partial class Form1 : Form
    {
        GuiDriver gd;
        bool active = true;
        public Form1()
        {
            gd = new GuiDriver("gd", true);
            InitializeComponent();
            gd.init();
            button1.Name = "Stop Capture";
            Thread oThread = new Thread(new ThreadStart(updateLogBox));
            this.FormClosing += Form1_FormClosing;
            // Start the thread
            oThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (active)
            {
                gd.stopCapture();
                active = false;
                button1.Text = "Start Capture";
            }
            else
            {
                gd.startCapture();
                active = true;
                button1.Text = "Stop Capture";
            }
        }
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                // Display a MsgBox asking the user to save changes or abort. 
            if (MessageBox.Show("Do you really want to kill this application?", "Data Capture",
               MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                gd.kill();
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
                            textBox1.AppendText(gd.myQ.Dequeue());
                        }

                    });
                }
            }

        }
    }
    class GuiDriver : Receiver
    {
        private CaptureClass cls;
        TextBox t;
        public Queue<String> myQ = new Queue<String>();
        

      // Creates a synchronized wrapper around the Queue.
      
        //private CaptureClass cc = new CaptureClass();
        public GuiDriver(string id, bool receiving = true)
            : base(id, receiving)
        {
        }
        public void setOutputBox(TextBox t){
            this.t = t;
        }
        public void init()
        {
            dp.Register(this);
            cls = new CaptureClass("CaptureClass")
            {
                StorageDir = @"C:\Data\"
            };
            cls.init();
        }
        public void startCapture()
        {
            cls.StartCapture();
        }
        public void kill()
        {
            cls.StopCapture();
            cls.Shutdown();
        }
        public void stopCapture()
        {
            cls.StopCapture();
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
