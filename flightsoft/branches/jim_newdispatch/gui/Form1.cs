﻿// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gui
{
    public partial class Form1 : Form
    {
        private GuiMain guiMain;
        private GuiUpdater guiUpdater;

        public Form1()
        {
            InitializeComponent();
            guiMain = new GuiMain(this, "GuiMain"); //TODO: move ID strings into a lookup table --JP
            guiMain.Startup_Init();
            guiUpdater = guiMain.GuiUpdater;

            Tab_Control_Main.SelectedIndex=4;

            //TODO: this updating should be handled externally to the form. --JP
            //it likes it when the timer is in here...
            DebugUpdateTimer.Tick += guiUpdater.UpdateGUI;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DebugOutput("Starting up...", 1);
        }

        private void rTB_Debug_Output_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_Go_Click(object sender, EventArgs e)
        {
            bool capting = guiMain.ToggleCapture();           
            if(capting)
                btn_Go.Text = "Stop Capture";
            else
                btn_Go.Text = "Start Capture";
            
        }

        public void DebugOutput(String s, int severity)
        {
            DebugOutput(s, SeverityColor.GetColor(severity));
        }

        public void DebugOutput(String s, Color col)
        {
            String s2 = GetTimestamp() + s + "\n";
            RichTextBox box = rTB_Debug_Output;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = col;
            box.AppendText(s2);
            box.SelectionColor = box.ForeColor;           
        }

        private String GetTimestamp()
        {
            return DateTime.Now.ToString("dd:HH:mm:ss ");
        }

    }
}
