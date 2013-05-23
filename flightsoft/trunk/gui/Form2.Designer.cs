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
            this.imageDisplayControl1 = new gui.ImageDisplayControl();
            this.guiUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.form2TopLevelLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // form2TopLevelLayoutPanel
            // 
            this.form2TopLevelLayoutPanel.ColumnCount = 3;
            this.form2TopLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.form2TopLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.form2TopLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.form2TopLevelLayoutPanel.Controls.Add(this.biteCASPanelControl1, 0, 1);
            this.form2TopLevelLayoutPanel.Controls.Add(this.imageDisplayControl1, 0, 0);
            this.form2TopLevelLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.form2TopLevelLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.form2TopLevelLayoutPanel.Name = "form2TopLevelLayoutPanel";
            this.form2TopLevelLayoutPanel.RowCount = 2;
            this.form2TopLevelLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.form2TopLevelLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.form2TopLevelLayoutPanel.Size = new System.Drawing.Size(635, 435);
            this.form2TopLevelLayoutPanel.TabIndex = 0;
            // 
            // biteCASPanelControl1
            // 
            this.biteCASPanelControl1.BroadcastCmd = null;
            this.biteCASPanelControl1.BroadcastLog = null;
            this.form2TopLevelLayoutPanel.SetColumnSpan(this.biteCASPanelControl1, 2);
            this.biteCASPanelControl1.CurrentConfig = null;
            this.biteCASPanelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.biteCASPanelControl1.Location = new System.Drawing.Point(3, 220);
            this.biteCASPanelControl1.Name = "biteCASPanelControl1";
            this.biteCASPanelControl1.Size = new System.Drawing.Size(608, 212);
            this.biteCASPanelControl1.TabIndex = 0;
            // 
            // imageDisplayControl1
            // 
            this.form2TopLevelLayoutPanel.SetColumnSpan(this.imageDisplayControl1, 2);
            this.imageDisplayControl1.Location = new System.Drawing.Point(3, 3);
            this.imageDisplayControl1.Name = "imageDisplayControl1";
            this.imageDisplayControl1.Size = new System.Drawing.Size(608, 211);
            this.imageDisplayControl1.TabIndex = 1;
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
            this.ClientSize = new System.Drawing.Size(635, 435);
            this.Controls.Add(this.form2TopLevelLayoutPanel);
            this.Name = "Form2";
            this.Text = "Form2";
            this.form2TopLevelLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel form2TopLevelLayoutPanel;
        private BiteCASPanelControl biteCASPanelControl1;
        private ImageDisplayControl imageDisplayControl1;
        private System.Windows.Forms.Timer guiUpdateTimer;
    }
}