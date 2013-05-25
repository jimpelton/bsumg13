using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gui
{
    public partial class LogMessagesControl : UserControl
    {
        public LogMessagesControl()
        {
            InitializeComponent();
        }

        public void AppendText(string messageText)
        {
            richTextBox1.AppendText(messageText);
        }
    }
}
