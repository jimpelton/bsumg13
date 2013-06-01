namespace gui
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.DebugUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.b_Execute_BITE_Test = new System.Windows.Forms.Button();
            this.rTB_Debug_Output = new System.Windows.Forms.RichTextBox();
            this.btn_Go = new System.Windows.Forms.Button();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cb_Autoscroll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // DebugUpdateTimer
            // 
            this.DebugUpdateTimer.Enabled = true;
            this.DebugUpdateTimer.Interval = 2000;
            // 
            // b_Execute_BITE_Test
            // 
            this.b_Execute_BITE_Test.Location = new System.Drawing.Point(1616, 98);
            this.b_Execute_BITE_Test.Name = "b_Execute_BITE_Test";
            this.b_Execute_BITE_Test.Size = new System.Drawing.Size(256, 80);
            this.b_Execute_BITE_Test.TabIndex = 2;
            this.b_Execute_BITE_Test.Text = "Execute BITE Test";
            this.b_Execute_BITE_Test.UseVisualStyleBackColor = true;
            this.b_Execute_BITE_Test.Click += new System.EventHandler(this.b_Execute_BITE_Test_Click);
            // 
            // rTB_Debug_Output
            // 
            this.rTB_Debug_Output.Location = new System.Drawing.Point(1110, 12);
            this.rTB_Debug_Output.Name = "rTB_Debug_Output";
            this.rTB_Debug_Output.Size = new System.Drawing.Size(500, 1050);
            this.rTB_Debug_Output.TabIndex = 3;
            this.rTB_Debug_Output.Text = "";
            // 
            // btn_Go
            // 
            this.btn_Go.Location = new System.Drawing.Point(1616, 12);
            this.btn_Go.Name = "btn_Go";
            this.btn_Go.Size = new System.Drawing.Size(256, 80);
            this.btn_Go.TabIndex = 4;
            this.btn_Go.Text = "Start Capture";
            this.btn_Go.UseVisualStyleBackColor = true;
            this.btn_Go.Click += new System.EventHandler(this.btn_Go_Click);
            // 
            // chart2
            // 
            this.chart2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Area3DStyle.Enable3D = true;
            chartArea2.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(1616, 207);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(256, 837);
            this.chart2.TabIndex = 5;
            this.chart2.Text = "chart1";
            // 
            // cb_Autoscroll
            // 
            this.cb_Autoscroll.AutoSize = true;
            this.cb_Autoscroll.Location = new System.Drawing.Point(1616, 184);
            this.cb_Autoscroll.Name = "cb_Autoscroll";
            this.cb_Autoscroll.Size = new System.Drawing.Size(135, 17);
            this.cb_Autoscroll.TabIndex = 6;
            this.cb_Autoscroll.Text = "Autoscroll Log Window";
            this.cb_Autoscroll.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1884, 1054);
            this.Controls.Add(this.cb_Autoscroll);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.btn_Go);
            this.Controls.Add(this.rTB_Debug_Output);
            this.Controls.Add(this.b_Execute_BITE_Test);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Boise State Microgravity Experiment Control";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer DebugUpdateTimer;
        private System.Windows.Forms.Button b_Execute_BITE_Test;
        private System.Windows.Forms.RichTextBox rTB_Debug_Output;
        private System.Windows.Forms.Button btn_Go;
        public System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        public System.Windows.Forms.CheckBox cb_Autoscroll;

    }
}

