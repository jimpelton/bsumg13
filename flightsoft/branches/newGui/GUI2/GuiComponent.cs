using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUI2
{
    class GuiComponent
    {
        public Queue<String> myQ = new Queue<String>();
        private System.Windows.Forms.TextBox textBox1;

        public GuiComponent(System.Windows.Forms.TextBox textBox1)
        {
            // TODO: Complete member initialization
            this.textBox1 = textBox1;
        }

        internal void Start()
        {
            throw new NotImplementedException();
        }
    }
}
