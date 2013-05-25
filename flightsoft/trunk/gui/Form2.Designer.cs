namespace gui
{
    partial class Form2
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
            this.form2TopLevelLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.biteCASPanelControl1 = new gui.BiteCASPanelControl();
            this.imageDisplayGroupBox = new System.Windows.Forms.GroupBox();
            this.imageDisplayControl = new gui.ImageDisplayControl();
            this.loggerTextGroupBox = new System.Windows.Forms.GroupBox();
            this.logMessagesControl = new gui.LogMessagesControl();
            this.componentStatusGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.biteTestButton = new System.Windows.Forms.Button();
            this.startCaptureButton = new System.Windows.Forms.Button();
            this.guiUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.form2TopLevelLayoutPanel.SuspendLayout();
            this.imageDisplayGroupBox.SuspendLayout();
            this.loggerTextGroupBox.SuspendLayout();
            this.componentStatusGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // form2TopLevelLayoutPanel
            // 
            this.form2TopLevelLayoutPanel.ColumnCount = 3;
            this.form2TopLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.form2TopLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.form2TopLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.form2TopLevelLayoutPanel.Controls.Add(this.biteCASPanelControl1, 0, 2);
            this.form2TopLevelLayoutPanel.Controls.Add(this.imageDisplayGroupBox, 0, 1);
            this.form2TopLevelLayoutPanel.Controls.Add(this.loggerTextGroupBox, 2, 1);
            this.form2TopLevelLayoutPanel.Controls.Add(this.componentStatusGroupBox, 2, 2);
            this.form2TopLevelLayoutPanel.Controls.Add(this.startCaptureButton, 0, 0);
            this.form2TopLevelLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.form2TopLevelLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.form2TopLevelLayoutPanel.Name = "form2TopLevelLayoutPanel";
            this.form2TopLevelLayoutPanel.RowCount = 3;
            this.form2TopLevelLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.04348F));
            this.form2TopLevelLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.47826F));
            this.form2TopLevelLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.47826F));
            this.form2TopLevelLayoutPanel.Size = new System.Drawing.Size(1187, 683);
            this.form2TopLevelLayoutPanel.TabIndex = 0;
            // 
            // biteCASPanelControl1
            // 
            this.biteCASPanelControl1.BroadcastCmd = null;
            this.biteCASPanelControl1.BroadcastLog = null;
            this.form2TopLevelLayoutPanel.SetColumnSpan(this.biteCASPanelControl1, 2);
            this.biteCASPanelControl1.CurrentConfig = null;
            this.biteCASPanelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.biteCASPanelControl1.Location = new System.Drawing.Point(3, 388);
            this.biteCASPanelControl1.Name = "biteCASPanelControl1";
            this.biteCASPanelControl1.Size = new System.Drawing.Size(784, 292);
            this.biteCASPanelControl1.TabIndex = 0;
            // 
            // imageDisplayGroupBox
            // 
            this.imageDisplayGroupBox.AutoSize = true;
            this.imageDisplayGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.form2TopLevelLayoutPanel.SetColumnSpan(this.imageDisplayGroupBox, 2);
            this.imageDisplayGroupBox.Controls.Add(this.imageDisplayControl);
            this.imageDisplayGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageDisplayGroupBox.Location = new System.Drawing.Point(3, 92);
            this.imageDisplayGroupBox.Name = "imageDisplayGroupBox";
            this.imageDisplayGroupBox.Size = new System.Drawing.Size(784, 290);
            this.imageDisplayGroupBox.TabIndex = 4;
            this.imageDisplayGroupBox.TabStop = false;
            this.imageDisplayGroupBox.Text = "Latest Images";
            // 
            // imageDisplayControl
            // 
            this.imageDisplayControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.imageDisplayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageDisplayControl.Location = new System.Drawing.Point(3, 16);
            this.imageDisplayControl.Name = "imageDisplayControl";
            this.imageDisplayControl.Size = new System.Drawing.Size(778, 271);
            this.imageDisplayControl.TabIndex = 0;
            // 
            // loggerTextGroupBox
            // 
            this.loggerTextGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loggerTextGroupBox.Controls.Add(this.logMessagesControl);
            this.loggerTextGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loggerTextGroupBox.Location = new System.Drawing.Point(793, 92);
            this.loggerTextGroupBox.Name = "loggerTextGroupBox";
            this.loggerTextGroupBox.Size = new System.Drawing.Size(391, 290);
            this.loggerTextGroupBox.TabIndex = 5;
            this.loggerTextGroupBox.TabStop = false;
            this.loggerTextGroupBox.Text = "Log Messages";
            // 
            // logMessagesControl
            // 
            this.logMessagesControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.logMessagesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessagesControl.Location = new System.Drawing.Point(3, 16);
            this.logMessagesControl.Name = "logMessagesControl";
            this.logMessagesControl.Size = new System.Drawing.Size(385, 271);
            this.logMessagesControl.TabIndex = 0;
            // 
            // componentStatusGroupBox
            // 
            this.componentStatusGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.componentStatusGroupBox.Controls.Add(this.tableLayoutPanel1);
            this.componentStatusGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentStatusGroupBox.Location = new System.Drawing.Point(793, 388);
            this.componentStatusGroupBox.Name = "componentStatusGroupBox";
            this.componentStatusGroupBox.Size = new System.Drawing.Size(391, 292);
            this.componentStatusGroupBox.TabIndex = 6;
            this.componentStatusGroupBox.TabStop = false;
            this.componentStatusGroupBox.Text = "Component Status";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.biteTestButton, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(385, 273);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // biteTestButton
            // 
            this.biteTestButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.biteTestButton.Location = new System.Drawing.Point(3, 236);
            this.biteTestButton.Name = "biteTestButton";
            this.biteTestButton.Size = new System.Drawing.Size(379, 34);
            this.biteTestButton.TabIndex = 0;
            this.biteTestButton.Text = "Start BITE Test";
            this.biteTestButton.UseVisualStyleBackColor = true;
            // 
            // startCaptureButton
            // 
            this.startCaptureButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startCaptureButton.Location = new System.Drawing.Point(3, 3);
            this.startCaptureButton.Name = "startCaptureButton";
            this.startCaptureButton.Size = new System.Drawing.Size(389, 83);
            this.startCaptureButton.TabIndex = 7;
            this.startCaptureButton.Text = "Start Capture";
            this.startCaptureButton.UseVisualStyleBackColor = true;
            this.startCaptureButton.Click += new System.EventHandler(this.startCaptureButton_Click);
            // 
            // guiUpdateTimer
            // 
            this.guiUpdateTimer.Interval = 1000;
            this.guiUpdateTimer.Tick += new System.EventHandler(this.guiUpdateTimer_Tick);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 683);
            this.Controls.Add(this.form2TopLevelLayoutPanel);
            this.Name = "Form2";
            this.Text = "Form2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.form2TopLevelLayoutPanel.ResumeLayout(false);
            this.form2TopLevelLayoutPanel.PerformLayout();
            this.imageDisplayGroupBox.ResumeLayout(false);
            this.loggerTextGroupBox.ResumeLayout(false);
            this.componentStatusGroupBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel form2TopLevelLayoutPanel;
        private BiteCASPanelControl biteCASPanelControl1;
        private System.Windows.Forms.Timer guiUpdateTimer;
        private System.Windows.Forms.GroupBox imageDisplayGroupBox;
        private ImageDisplayControl imageDisplayControl;
        private System.Windows.Forms.GroupBox loggerTextGroupBox;
        private System.Windows.Forms.GroupBox componentStatusGroupBox;
        private LogMessagesControl logMessagesControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button biteTestButton;
        private System.Windows.Forms.Button startCaptureButton;
    }
}