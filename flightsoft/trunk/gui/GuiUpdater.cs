using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms.DataVisualization.Charting;
using uGCapture;
using System.Runtime.InteropServices;

namespace gui
{
    public class GuiUpdater : Receiver
    {
        Form1 mainform = null;
        private GuiMain Guimain = null;
        private Series graph1data = null;
        private BiteCASPanel CAS = null;
        private Queue<uGCapture.StatusStr> CASQueue = null;


        public GuiUpdater(Form1 f,GuiMain m, BiteCASPanel c, string id, bool receiving=true) 
            : base(id, receiving)
        {         
            mainform = f;
            CAS = c;
            graph1data = new Series("Points");
            Guimain = m;
            dp.Register(this);
        }

        public void UpdateGUI(object sender, EventArgs e)
        {
            updateCASPanel();
            //try
            //{
            //    ExecuteMessageQueue();              
            //}
            //catch (NullReferenceException nel)/// temp fix for monday's test. 5 minutes out.
            //{
            //    Console.WriteLine("UpdateGUI in GuiUpdater.cs threw a null pointer exception at ExecuteMessageQueue();  ");
 
            //}
            mainform.chart1.Series.Clear();
            mainform.chart2.Series.Clear();
            List<DataPoint> frames = Guimain.DataFrames;
            if (frames.Count > 0)
            {

                mainform.chart1.Series.Add("Graph1");
             //   mainform.chart1.Series.Add("Graph2");
              //  mainform.chart1.Series.Add("Graph3");
                mainform.chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 38.0;
                mainform.chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 35.0;
                if (frames.Count > 99)
                {
                    Color col = System.Drawing.Color.FromArgb(255, (int)Math.Min(255, Math.Max(0, ((frames[99].phidgetTemperature_ProbeTemp - 35)) * (255 / 3))), 0, (int)Math.Min(255, Math.Max(0, ((37 - (frames[99].phidgetTemperature_ProbeTemp - 35)) * (255 / 3)))));
                    mainform.chart1.Series["Graph1"].Color = col;
                }
                //mainform.chart1.ChartAreas["ChartArea2"].AxisY.Maximum = 38.0;
                //mainform.chart1.ChartAreas["ChartArea2"].AxisY.Minimum = 10.0;
               
                mainform.chart2.Series.Add("Gravity");
                mainform.chart2.ChartAreas["ChartArea1"].AxisY.Maximum = 2.0;
                mainform.chart2.ChartAreas["ChartArea1"].AxisY.Minimum = -1.0;
                mainform.chart2.ChartAreas["ChartArea1"].AxisX.Maximum = frames.Count;
                mainform.chart2.ChartAreas["ChartArea1"].AxisX.Minimum = frames.Count-2;


                foreach (DataPoint p in frames)
                {
                    mainform.chart1.Series["Graph1"].ChartType = SeriesChartType.SplineArea;                
                    mainform.chart1.Series["Graph1"].Points.AddY(p.phidgetTemperature_ProbeTemp);
                    mainform.chart1.Series["Graph1"].ChartArea = "ChartArea1";
/*
                    mainform.chart1.Series["Graph2"].ChartType = SeriesChartType.SplineArea;
                    mainform.chart1.Series["Graph2"].Points.AddY(p.phidgetTemperature_AmbientTemp);
                    mainform.chart1.Series["Graph2"].ChartArea = "ChartArea2";

                    mainform.chart1.Series["Graph3"].ChartType = SeriesChartType.SplineArea;
                    mainform.chart1.Series["Graph3"].Points.AddY(p.phidgetsanalogInputs[0]);
                    mainform.chart1.Series["Graph3"].ChartArea = "ChartArea3";
                    */
                    mainform.chart2.Series["Gravity"].ChartType = SeriesChartType.SplineArea;
                    mainform.chart2.Series["Gravity"].Points.AddY(
                        p.accel2acceleration[1]
                        );
                    mainform.chart2.Series["Gravity"].ChartArea = "ChartArea1";
                }
            }
            //dp.Broadcast(new DataRequestMessage(this));
        }

        private void updateCASPanel()
        {
            
            ulong freespace;//freespace was an awesome game.
            if(DriveFreeBytes(Guimain.guiDataPath, out freespace))
            {
                if      (freespace < 10000000000L)
                    CAS.b_Drive_Full.BackColor = Color.OrangeRed;
                else if (freespace < 20000000000L)
                    CAS.b_Drive_Full.BackColor = Color.Yellow;
                else if (freespace < 200000000000L)
                    CAS.b_Drive_Full.BackColor = Color.Green;
                else
                    CAS.b_Drive_Full.BackColor = Color.Black;

                CAS.b_Drive_Full.Text = "Drive Full (" +(freespace/(1024*1024*1024))+")";
            }

            UTF8Encoding encoding = new UTF8Encoding();
            DataSet<byte> dat = Guimain.getLatestData();

            if (dat != null)
            {
                string UPSdat = encoding.GetString(dat.lastData[(int)BufferType.UTF8_UPS]);
                string[] UPSdats = UPSdat.Split();
                if (UPSdats.Length > 6)
                {
                    try
                    {
                        int bat = int.Parse(UPSdats[4]);
                        if (bat < 20)
                        {
                            CAS.b_Battery_Level.BackColor = Color.OrangeRed;
                            CAS.b_Battery_Com.BackColor = Color.OrangeRed;
                        }
                        else if (bat < 90)
                        {
                            CAS.b_Battery_Level.BackColor = Color.Yellow;
                            CAS.b_Battery_Com.BackColor = Color.Yellow;
                        }
                        else if (bat < 98)
                        {
                            CAS.b_Battery_Level.BackColor = Color.YellowGreen;
                            CAS.b_Battery_Com.BackColor = Color.Green;
                        }
                        else if (bat < 99)
                        {
                            CAS.b_Battery_Level.BackColor = Color.Green;
                            CAS.b_Battery_Com.BackColor = Color.Green;
                        }
                        else
                        {
                            CAS.b_Battery_Level.BackColor = Color.Black;
                            CAS.b_Battery_Com.BackColor = Color.Black;
                        }

                        CAS.b_Battery_Level.Text = "Battery Level (" + UPSdats[4] + ")";
                        if (int.Parse(UPSdats[6]) > 0)
                        {
                            CAS.b_Battery_Com.Text = BatteryStatusStrings.GetBatteryStatusStr(int.Parse(UPSdats[6]));
                        }
                        else
                        {
                            CAS.b_Battery_Com.Text = "Battery Com";
                        }

                    }
                    catch (FormatException e)
                    {
                        //a malformed packet has arrived. Tremble in fear.
                    }

                }
                else
                {
                    CAS.b_Battery_Com.Text = "Battery Com";
                }

            }
            
        }


        // Stolen! http://stackoverflow.com/questions/1393711/get-free-disk-space
        // Pinvoke for API function
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);

        public static bool DriveFreeBytes(string folderName, out ulong freespace)
        {
            freespace = 0;
            if (string.IsNullOrEmpty(folderName))
            {
                //throw new ArgumentNullException("folderName");
                return false;
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            ulong free = 0, dummy1 = 0, dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
            {
                freespace = free;
                return true;
            }
            else
            {
                return false;
            }
        }//end theivery!


        public override void exLogMessage(Receiver r, Message m) 
        {
            LogMessage lm = m as LogMessage;
            if (lm != null)
                mainform.DebugOutput(lm.message, lm.severity);
        }

        //TODO: Remove
        /*
        public override void exDataMessage(Receiver r, Message m)
        {
            DataPoint dat = new DataPoint();

            DataMessage dm = m as DataMessage;
            if (dm == null) return;

            
            dat.image405 = dm.image405;
            dat.image485 = dm.image485;

            dat.NIanaloginputs = dm.NIanaloginputs;
            dat.UPSstate = dm.UPSstate;
            dat.VCommstate = dm.VCommstate;
            //dat.WellIntensities = dm.WellIntensities;
            dat.WellIntensities405 = dm.WellIntensities405;
            dat.WellIntensities485 = dm.WellIntensities405;
            dat.accel1acceleration = dm.accel1acceleration;
            dat.accel1rawacceleration = dm.accel1rawacceleration;
            dat.accel1state = dm.accel1state;
            dat.accel1vibration = dm.accel1vibration;
            dat.accel2acceleration = dm.accel2acceleration;
            dat.accel2rawacceleration = dm.accel2rawacceleration;
            dat.accel2state = dm.accel2state;
            dat.accel2vibration = dm.accel2vibration;
            dat.phidgets888state = dm.phidgets888state;
            dat.phidgetsanalogInputs = dm.phidgetsanalogInputs;
            dat.phidgetsdigitalInputs = dm.phidgetsdigitalInputs;
            dat.phidgetsdigitalOutputs = dm.phidgetsdigitalOutputs;
            dat.phidgetstempstate = dm.phidgetstempstate;
            dat.phidgetTemperature_AmbientTemp = dm.phidgetTemperature_AmbientTemp;
            dat.phidgetTemperature_ProbeTemp = dm.phidgetTemperature_ProbeTemp;
            dat.timestamp = dm.timestamp;

            dat.vcommHumidity = dm.vcommHumidity;
            dat.vcommIllumination = dm.vcommIllumination;
            dat.vcommPressure = dm.vcommPressure;
            dat.vcommTemperature1 = dm.vcommTemperature1;
            dat.vcommTemperature2 = dm.vcommTemperature2;
            dat.vcommTemperature3 = dm.vcommTemperature3;

            Guimain.insertDataPoint(dat);

        }
        */

        public override void exDataRequestMessage(Receiver r, Message m)
        {   
            
        }
        /*
        static unsafe public Bitmap ConvertCapturedRawImage(byte[] indata)
        {
            Bitmap bitmap = null;
            unsafe
            {
                fixed (byte* ptr = indata)
                {
                    ushort[] pixels = new ushort[indata.Length/2];
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        pixels[i] = (ushort) ((indata[i * 2] * 256) + indata[i]);
                    }
                    uint[] dat = ConvertGray16ToRGB(pixels, 12);
                    fixed (uint* ptr2 = dat)
                    {


                        IntPtr scan0 = new IntPtr(ptr2);
                        bitmap = new Bitmap(2592, 1944, // Image size
                                            2592, // Scan size
                                            PixelFormat.Format24bppRgb, scan0);

        
                    }
                }
            }
            return bitmap;
        }
        */
        public static uint[] ConvertGray16ToRGB(ushort[] grayPixels, int bitsUsed)
        {
            int pixelCount = grayPixels.Length;
            uint[] rgbPixels = new uint[pixelCount];
            int shift = bitsUsed - 8;
            for (int i = 0; i < pixelCount; i++)
            {
                uint gray = (uint)grayPixels[i] >> shift;
                rgbPixels[i] = 0xff000000U | gray | (gray << 8) | (gray <<
                16);
            }
            return (rgbPixels);
        }



        
        override public void exUPSStatusMessage(Receiver r, Message m) 
        {
            try
            {
                UPSStatusMessage msg = (UPSStatusMessage)m;
                if (msg.getState() == uGCapture.StatusStr.STAT_FAIL)
                {
                    CAS.b_Battery_Com.BackColor = Color.OrangeRed;
                    CAS.b_Battery_Com.Text = "Battery Com";
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_GOOD)
                {
                    //CAS.b_Battery_Com.BackColor = Color.Black;
                }
                else
                {
                    CAS.b_Battery_Com.BackColor = Color.OrangeRed;
                    CAS.b_Battery_Com.Text = "Battery Com";
                }
            }
            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.ToString(), 0);
            }

        }
        override public void exAccelStatusMessage(Receiver r, Message m) { ; }
        override public void exVcommStatusMessage(Receiver r, Message m) { ; }

        override public void exNI6008StatusMessage(Receiver r, Message m)
        {
            try
            {
                NI6008StatusMessage msg = (NI6008StatusMessage)m;
                if (msg.getState() == uGCapture.StatusStr.STAT_FAIL_NI6008DAQ)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.OrangeRed;             
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_DISC_NI6008DAQ)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.OrangeRed;       
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_ATCH_NI6008DAQ)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.OrangeRed;
                }
                else if(msg.getState() == uGCapture.StatusStr.STAT_GOOD_NI6008DAQ)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.Black;
                }
            }
            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.ToString(), 0);
            }
        }

        override public void exAptinaStatusMessage(Receiver r, Message m)
        {
            try
            {
                //todo: convert to a switch
                AptinaStatusMessage msg = (AptinaStatusMessage)m;
                if (msg.getState() == uGCapture.StatusStr.STAT_FAIL_405)
                {
                    CAS.b_Camera_405.BackColor = Color.OrangeRed;
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_FAIL_485)
                {
                    CAS.b_Camera_485.BackColor = Color.OrangeRed;
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_ERR_405)
                {
                    CAS.b_Camera_405.BackColor = Color.OrangeRed;
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_ERR_485)
                {
                    CAS.b_Camera_485.BackColor = Color.OrangeRed;
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_GOOD_405)
                {
                    CAS.b_Camera_405.BackColor = Color.Black;
                }
                else if (msg.getState() == uGCapture.StatusStr.STAT_GOOD_485)
                {
                    CAS.b_Camera_485.BackColor = Color.Black;
                }
                else
                {
                    CAS.b_Camera_405.BackColor = Color.Salmon;//Something is fishy here.
                    CAS.b_Camera_485.BackColor = Color.Salmon;
                }
            }
            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.ToString(), 0);
            }
        }

        override public void exSpatialStatusMessage(Receiver r, Message m) { ; }
        override public void exPhidgetsStatusMessage(Receiver r, Message m) { ; }







    }
}
