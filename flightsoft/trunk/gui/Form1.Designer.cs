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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.TabPage_Debug = new System.Windows.Forms.TabPage();
            this.rTB_Debug_Output = new System.Windows.Forms.RichTextBox();
            this.TabPage_Graphs = new System.Windows.Forms.TabPage();
            this.TabPage_BITE = new System.Windows.Forms.TabPage();
            this.TabPage_Wells = new System.Windows.Forms.TabPage();
            this.TabPage_Capture = new System.Windows.Forms.TabPage();
            this.btn_Go = new System.Windows.Forms.Button();
            this.Tab_Control_Main = new System.Windows.Forms.TabControl();
            this.DebugUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.TabPage_Debug.SuspendLayout();
            this.TabPage_Capture.SuspendLayout();
            this.Tab_Control_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // TabPage_Debug
            // 
            this.TabPage_Debug.Controls.Add(this.rTB_Debug_Output);
            this.TabPage_Debug.Location = new System.Drawing.Point(4, 29);
            this.TabPage_Debug.Name = "TabPage_Debug";
            this.TabPage_Debug.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Debug.Size = new System.Drawing.Size(1154, 688);
            this.TabPage_Debug.TabIndex = 4;
            this.TabPage_Debug.Text = "Debug";
            this.TabPage_Debug.UseVisualStyleBackColor = true;
            // 
            // rTB_Debug_Output
            // 
            this.rTB_Debug_Output.Location = new System.Drawing.Point(6, 6);
            this.rTB_Debug_Output.Name = "rTB_Debug_Output";
            this.rTB_Debug_Output.Size = new System.Drawing.Size(1142, 676);
            this.rTB_Debug_Output.TabIndex = 0;
            this.rTB_Debug_Output.Text = "";
            this.rTB_Debug_Output.TextChanged += new System.EventHandler(this.rTB_Debug_Output_TextChanged);
            // 
            // TabPage_Graphs
            // 
            this.TabPage_Graphs.Location = new System.Drawing.Point(4, 29);
            this.TabPage_Graphs.Name = "TabPage_Graphs";
            this.TabPage_Graphs.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Graphs.Size = new System.Drawing.Size(1154, 688);
            this.TabPage_Graphs.TabIndex = 3;
            this.TabPage_Graphs.Text = "Graphs";
            this.TabPage_Graphs.UseVisualStyleBackColor = true;
            // 
            // TabPage_BITE
            // 
            this.TabPage_BITE.Location = new System.Drawing.Point(4, 29);
            this.TabPage_BITE.Name = "TabPage_BITE";
            this.TabPage_BITE.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_BITE.Size = new System.Drawing.Size(1154, 688);
            this.TabPage_BITE.TabIndex = 2;
            this.TabPage_BITE.Text = "BITE";
            this.TabPage_BITE.UseVisualStyleBackColor = true;
            // 
            // TabPage_Wells
            // 
            this.TabPage_Wells.Location = new System.Drawing.Point(4, 29);
            this.TabPage_Wells.Name = "TabPage_Wells";
            this.TabPage_Wells.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Wells.Size = new System.Drawing.Size(1154, 688);
            this.TabPage_Wells.TabIndex = 1;
            this.TabPage_Wells.Text = "Wells";
            this.TabPage_Wells.UseVisualStyleBackColor = true;
            // 
            // TabPage_Capture
            // 
            this.TabPage_Capture.Controls.Add(this.chart1);
            this.TabPage_Capture.Controls.Add(this.btn_Go);
            this.TabPage_Capture.Location = new System.Drawing.Point(4, 29);
            this.TabPage_Capture.Name = "TabPage_Capture";
            this.TabPage_Capture.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Capture.Size = new System.Drawing.Size(1154, 688);
            this.TabPage_Capture.TabIndex = 0;
            this.TabPage_Capture.Text = "Capture Control";
            this.TabPage_Capture.UseVisualStyleBackColor = true;
            // 
            // btn_Go
            // 
            this.btn_Go.Location = new System.Drawing.Point(444, 59);
            this.btn_Go.Name = "btn_Go";
            this.btn_Go.Size = new System.Drawing.Size(215, 106);
            this.btn_Go.TabIndex = 0;
            this.btn_Go.Text = "Start Capture";
            this.btn_Go.UseVisualStyleBackColor = true;
            this.btn_Go.Click += new System.EventHandler(this.btn_Go_Click);
            // 
            // Tab_Control_Main
            // 
            this.Tab_Control_Main.Controls.Add(this.TabPage_Capture);
            this.Tab_Control_Main.Controls.Add(this.TabPage_Wells);
            this.Tab_Control_Main.Controls.Add(this.TabPage_BITE);
            this.Tab_Control_Main.Controls.Add(this.TabPage_Graphs);
            this.Tab_Control_Main.Controls.Add(this.TabPage_Debug);
            this.Tab_Control_Main.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Tab_Control_Main.Location = new System.Drawing.Point(12, 12);
            this.Tab_Control_Main.Name = "Tab_Control_Main";
            this.Tab_Control_Main.SelectedIndex = 0;
            this.Tab_Control_Main.Size = new System.Drawing.Size(1162, 721);
            this.Tab_Control_Main.TabIndex = 0;
            // 
            // DebugUpdateTimer
            // 
            this.DebugUpdateTimer.Enabled = true;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(6, 192);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(851, 300);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 745);
            this.Controls.Add(this.Tab_Control_Main);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Boise State Microgravity Experiment Control";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.TabPage_Debug.ResumeLayout(false);
            this.TabPage_Capture.ResumeLayout(false);
            this.Tab_Control_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage TabPage_Debug;
        private System.Windows.Forms.TabPage TabPage_Graphs;
        private System.Windows.Forms.TabPage TabPage_BITE;
        private System.Windows.Forms.TabPage TabPage_Wells;
        private System.Windows.Forms.TabPage TabPage_Capture;
        private System.Windows.Forms.TabControl Tab_Control_Main;
        private System.Windows.Forms.RichTextBox rTB_Debug_Output;
        private System.Windows.Forms.Button btn_Go;
        private System.Windows.Forms.Timer DebugUpdateTimer;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;

    }
}

