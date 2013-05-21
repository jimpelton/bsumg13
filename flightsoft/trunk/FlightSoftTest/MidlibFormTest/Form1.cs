using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uGCapture;

namespace MidlibFormTest
{
    public partial class Form1 : Form
    {
        Initter initter = new Initter();
        public LinkedList<string> s = new LinkedList<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            initter.Hwnd = Handle;
            initter.set_callback(log_message);
            initter.init();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            initter.startThread();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private delegate void SetTextCallback(string s);
        public void log_message(string s)
        {
            if (! s.EndsWith("\n"))
            {
                s += "\n";
            }
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallback a = new SetTextCallback(log_message_helper);
                Invoke(a, new object[] { s });
            }
            else
            {
                log_message_helper(s);
            }
        }

        public void log_message_helper(string s)
        {
            this.richTextBox1.Text += s;
        }
    }
}
