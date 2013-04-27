using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms.DataVisualization.Charting;
using uGCapture;


namespace gui
{
    public class GuiUpdater : Receiver
    {
        Form1 mainform = null;
        private GuiMain Guimain = null;
        private Series graph1data = null;
        public GuiUpdater(Form1 f, GuiMain m)
            : base("GuiUpdater", true)
        {         
            mainform = f;
            graph1data = new Series("Points");
            Guimain = m;
            dp.Register(this);
        }

        public void UpdateGUI(object sender, EventArgs e)
        {
            try
            {
                //ExecuteMessageQueue();              
            }
            catch (NullReferenceException nel)
            {
                Console.WriteLine("UpdateGUI in GuiUpdater.cs threw a null pointer exception at ExecuteMessageQueue();  "+nel.ToString());
 
            }
            mainform.chart1.Series.Clear();
            mainform.chart2.Series.Clear();
            List<DataPoint> frames = Guimain.getDataPoints();
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
            dp.Broadcast(new DataRequestMessage(this));
        }

        public override void exLogMessage(Receiver r, Message m) 
        {
            LogMessage lm = m as LogMessage;
            if (lm != null)
                mainform.DebugOutput(lm.message, lm.severity);
        }

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
            dat.WellIntensities405 = dm.WellIntensities405;
            dat.WellIntensities485 = dm.WellIntensities485;
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
    }
}
