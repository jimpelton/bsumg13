using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;
using uGCapture;
using Message = uGCapture.Message;

namespace gui
{
    public class GuiUpdater : Receiver
    {
        
        public void Broadcast(Message m)
        {
            dp.Broadcast(m);
        }

        public void BroadcastLogError(string s)
        {
            dp.BroadcastLog(this, s, Status.STAT_ERR);
        }
        
        IList<Control> updatables = new List<Control>();


        Form1 mainform;
        private GuiMain Guimain;
        //private Series graph1data;
        private BiteCASPanel CAS;


        public IList<DataPoint> DataFrames
        {
            set { frames = value; }
            get { return frames; }
        }
        private IList<DataPoint> frames;

        private Status lastAccelState = Status.STAT_ERR;
        private Status lastSpatialState = Status.STAT_ERR;
        private Status lastTemperatureState = Status.STAT_ERR;

        private long last1018update = 0;

        private double[] lowpassfilteredwells ;
        private double[] lowlowpassfilteredwells;
        private double[] lowlowlowpassfilteredwells;
        private double[] lowlowlowlowpassfilteredwells;

        public GuiUpdater(Form1 f, GuiMain m, BiteCASPanel c, string id, bool receiving=true) 
            : base(id, receiving)
        {         
            mainform = f;
            CAS = c;
            //graph1data = new Series("Points");
            Guimain = m;
            dp.Register(this);  //yikes!
        }

        public GuiUpdater(string id, bool receiving = true)
            : base(id, receiving)
        {
            //mainform = f;
            //CAS = c;
            //graph1data = new Series("Points");
            //Guimain = m;
            dp.Register(this);  //yikes!
        }

        public void UpdateGUI(object sender, EventArgs e)
        {
            updateCASPanel();
            updateImages();
            updateFrames();
        }

        private void updateFrames()
        {
            if (frames.Count > 100)
                frames.RemoveAt(0);

            mainform.chart2.Series.Clear();

            if (frames.Count <= 0) return; 
            if (frames.Count > 99)
            {
                //eeeeeee!!!
                Color col = Color.FromArgb(255, 
                (int)Math.Min(255, Math.Max(0, ((frames[99].phidgetTemperature_ProbeTemp - 35)) * (255 / 3))), 
                0, 
                (int)Math.Min(255, Math.Max(0, ((37 - (frames[99].phidgetTemperature_ProbeTemp - 35)) * (255 / 3)))));
            }

            mainform.chart2.Series.Add("Gravity");
            mainform.chart2.ChartAreas["ChartArea1"].AxisY.Maximum = 2.0;
            mainform.chart2.ChartAreas["ChartArea1"].AxisY.Minimum = -1.0;
            mainform.chart2.ChartAreas["ChartArea1"].AxisX.Maximum = frames.Count;
            mainform.chart2.ChartAreas["ChartArea1"].AxisX.Minimum = frames.Count - 2;


            foreach (DataPoint p in frames)
            {
                mainform.chart2.Series["Gravity"].ChartType = SeriesChartType.SplineArea;
                mainform.chart2.Series["Gravity"].Points.AddY(
                    p.accel1rawacceleration[1]
                    );
                mainform.chart2.Series["Gravity"].ChartArea = "ChartArea1";
            }
        }

        private void updateImages()
        {
            DataSet<byte> dat = Guimain.getLatestData();
            byte[] i405 = dat.lastData[BufferType.USHORT_IMAGE405];
            byte[] i485 = dat.lastData[BufferType.USHORT_IMAGE485];

            //test goodness
            /*
            BinaryReader b = new BinaryReader(File.Open("data_485_1000.raw",FileMode.Open));
            i405 = b.ReadBytes(2 * 2592 * 1944);
            b.Close();
            b.Dispose();
            */

            ImageDisplay iee = Guimain.guiImageDisplay;
            if (i405 != null)
            {
                iee.pictureBox1.Image = ConvertCapturedRawImage(i405);
                iee.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
            if (i485 != null)
            {
                iee.pictureBox2.Image = ConvertCapturedRawImage(i485);
                iee.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
        }



        private void updateCASFreeSpace()
        {
            ulong freespace;//freespace was an awesome game.
            if (DriveFreeBytes(Guimain.guiDataPath, out freespace))
            {
                if (freespace < Guimain.CurrentConfig.RedSpace)                   
                {
                    CAS.b_Drive_Full.BackColor = Color.OrangeRed;
                    Guimain.switchToAlternateDrive();
                }
                else if (freespace < Guimain.CurrentConfig.YellowSpace)
                {
                    CAS.b_Drive_Full.BackColor = Color.Yellow;
                }
                else if (freespace < Guimain.CurrentConfig.GreenSpace)
                {
                    CAS.b_Drive_Full.BackColor = Color.Green;
                    
                }
                else
                {
                    CAS.b_Drive_Full.BackColor = Color.Black;
                }

                CAS.b_Drive_Full.Text = "Drive Full (" + (freespace / (1024 * 1024 * 1024)) + ")";
            }
        }

        private void updateCASBattery(DataSet<byte> dat)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string UPSdat = encoding.GetString(dat.lastData[BufferType.UTF8_UPS]);
            string[] UPSdats = UPSdat.Split();
            if (UPSdats.Length > 6)
            {
                try
                {
                    int bat = int.Parse(UPSdats[4]);
                    if (bat < Guimain.CurrentConfig.RedBattery)
                    {
                        CAS.b_Battery_Level.BackColor = Color.OrangeRed;
                        CAS.b_Battery_Com.BackColor = Color.OrangeRed;
                    }
                    else if (bat < Guimain.CurrentConfig.YellowBattery)
                    {
                        CAS.b_Battery_Level.BackColor = Color.Yellow;
                        CAS.b_Battery_Com.BackColor = Color.Yellow;
                    }
                    else if (bat < Guimain.CurrentConfig.GreenBattery)
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
                        string batstring = BatteryStatusStrings.GetBatteryStatusStr(int.Parse(UPSdats[6]));
                        if(batstring!=null)
                        CAS.b_Battery_Com.Text = batstring;
                    }
                    else
                    {
                        CAS.b_Battery_Com.Text = "Battery Com";
                    }

                }
                catch (FormatException e)
                {
                    dp.BroadcastLog(this, "CAS Battery update handler has encountered a malformed packet", 1);
                    //a malformed packet has arrived. Tremble in fear.
                }
            }
            else
            {
                CAS.b_Battery_Com.Text = "Battery Com";
            }
        }


        private void updateCASLights(DataSet<byte> dat)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            try
            {

                string lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                string[] lightdats = lightdat.Split();
                double l1 = 0, l2 = 0;

                if (lightdats.Length > 8)
                {
                    l1 = double.Parse(lightdats[11]);
                    l2 = double.Parse(lightdats[14]);
            }

                if (l1 < 100)
                {
                    CAS.b_Light_1.BackColor = Color.OrangeRed;
                }
                else if (l1 < 200)
                {
                    CAS.b_Light_1.BackColor = Color.Yellow;
                }
                else if (l1 < 300)
                {
                    CAS.b_Light_1.BackColor = Color.Green;
                }
            else
            {
                    CAS.b_Light_1.BackColor = Color.Black;
            }

                if (l2 < 10)
                {
                    CAS.b_Light_2.BackColor = Color.OrangeRed;
        }
                else if (l2 < 200)
                {
                    CAS.b_Light_2.BackColor = Color.Yellow;
                }
                else if (l2 < 300)
                {
                    CAS.b_Light_2.BackColor = Color.Green;
                }
                else
                {
                    CAS.b_Light_2.BackColor = Color.Black;
                }


            }
            catch (FormatException e)
            {
                dp.BroadcastLog(this, "CAS Light update handler has encountered a malformed packet", 1);
                //a malformed packet has arrived. Tremble in fear.
            }

        }

        private void updateCASAccel(DataSet<byte> dat)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            try
            {
                if (lastAccelState == Status.STAT_GOOD)
                    CAS.b_Accel_1.BackColor = Color.Black;
                if (lastSpatialState == Status.STAT_GOOD)
                    CAS.b_Accel_2.BackColor = Color.Black;

                //get the difference between accel and spacials acceleration vector magnitudes.
                double x1=0, x2=0, x3=0, y1=0, y2=0, y3=0, z1=0, z2=0, z3=0;
                string Acceldat = encoding.GetString(dat.lastData[BufferType.UTF8_ACCEL]);
                string[] Acceldats = Acceldat.Split();
                string AAcceldat = encoding.GetString(dat.lastData[BufferType.UTF8_NI6008]);
                string[] AAcceldats = AAcceldat.Split();

                if (Acceldats.Length > 5)
                {
                    x1 = double.Parse(Acceldats[3]);
                    y1 = double.Parse(Acceldats[4]);
                    z1 = double.Parse(Acceldats[5]);

                    DataPoint d = new DataPoint();
                    d.accel1rawacceleration[0] = x1;
                    d.accel1rawacceleration[1] = y1;
                    d.accel1rawacceleration[2] = z1;
                    Guimain.DataFrames.Add(d);

                }
                else
                {
                     CAS.b_Accel_1.BackColor = Color.OrangeRed;
                }
               

                Acceldat = encoding.GetString(dat.lastData[BufferType.UTF8_SPATIAL]);
                Acceldats = Acceldat.Split();
                if (Acceldats.Length > 5)
                {
                    x2 = double.Parse(Acceldats[3]);
                    y2 = double.Parse(Acceldats[4]);
                    z2 = double.Parse(Acceldats[5]);
                }
                else
                {
                    CAS.b_Accel_1.BackColor = Color.OrangeRed;                 
                }

                if (AAcceldats.Length > 5)
                {
                    //convert these from voltages to Gs.... 
                    x3 = double.Parse(AAcceldats[3+0]);//3 is the zeroth element. x is on 0
                    y3 = double.Parse(AAcceldats[3+4]);//y is on 4
                    z3 = double.Parse(AAcceldats[3+1]);//z is on 1
                }

                if (Math.Abs(x3 - y3) < 0.01 && Math.Abs(y3 - z3) < 0.01 && Math.Abs(z3 - x3) < 0.01)
                {
                    if(CAS.b_Accel_Aircraft.BackColor!= Color.OrangeRed)
                        CAS.b_Accel_Aircraft.BackColor = Color.Yellow;  
                }

                double m1 = Math.Sqrt((x1 * x1) + (y1 * y1) + (z1 * z1));
                double m2 = Math.Sqrt((x2 * x2) + (y2 * y2) + (z2 * z2));
                double dif = Math.Abs(m1 - m2);

                if (dif > 0.5)
                {
                    if(CAS.b_Accel_1.BackColor!=Color.OrangeRed && lastAccelState == Status.STAT_GOOD)
                        CAS.b_Accel_1.BackColor = Color.Green;
                    if (CAS.b_Accel_2.BackColor != Color.OrangeRed && lastSpatialState == Status.STAT_GOOD)
                        CAS.b_Accel_2.BackColor = Color.Green;
                }
            }
            catch (FormatException e)
            {
                dp.BroadcastLog(this, "Cas Accel update handler has encountered a malformed packet", 1);
                //a malformed packet has arrived. Tremble in fear.
            }
        }


        private void updateCASPhidgets(DataSet<byte> dat)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            try
            {
                double temp = 0;
                bool door = false;
                string pdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                string[] pdats = pdat.Split();

                if (lastTemperatureState == Status.STAT_GOOD)
                {
                    temp = double.Parse(pdats[6]);
                    if (temp > 2000000000)
                    {
                        CAS.b_Heater_High.BackColor = Color.Salmon;
                    }
                    else if (temp > 40)
                    {
                        CAS.b_Heater_High.BackColor = Color.OrangeRed;
                        CAS.b_Heater_Auto_Shutoff.BackColor = Color.OrangeRed;
                        dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_HEATER_OFF));
                    }
                    else if (temp > 38)
                    {
                        CAS.b_Heater_High.BackColor = Color.OrangeRed;
                    }
                    else
                    {
                        CAS.b_Heater_Auto_Shutoff.BackColor = Color.Black;
                        CAS.b_Heater_High.BackColor = Color.Black;
                        dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_HEATER_ON));
                    }

                    if (temp < 1)
                    {
                        CAS.b_Heater_Low.BackColor = Color.White;
                    }
                    else if (temp < 20)
                    {
                        CAS.b_Heater_Low.BackColor = Color.LightBlue;
                    }
                    else if (temp < 35)
                    {
                        CAS.b_Heater_Low.BackColor = Color.OrangeRed;
                    }
                    else
                    {
                        CAS.b_Heater_Low.BackColor = Color.Black;
                    }
                }
                else
                {
                    CAS.b_Heater_Low.BackColor = Color.OrangeRed;
                    CAS.b_Heater_High.BackColor = Color.OrangeRed;
                    CAS.b_Heater_Auto_Shutoff.BackColor = Color.OrangeRed;
                }
                if (pdats.Length > 10)
                {
                    door = bool.Parse(pdats[9]);
                }
                //TODO Make this change based on the capture status.
                bool running = true;
                if (door)
                {
                    CAS.b_Doors.BackColor = Color.Black;
                }
                else
                {
                    if(running)
                        CAS.b_Doors.BackColor = Color.OrangeRed;
                    else
                        CAS.b_Doors.BackColor = Color.Yellow;
                }
            }
            catch (FormatException e)
            {
                dp.BroadcastLog(this, "CAS Doors update handler has encountered a malformed packet", 1);
                //a malformed packet has arrived. Tremble in fear.
            }
        }
        

        private void updateCASPanel()
        {
            DataSet<byte> dat = Guimain.getLatestData();
            if (dat != null)
            {
                updateCASFreeSpace();
                updateCASBattery(dat);
                updateCASAccel(dat);
                updateCASPhidgets(dat);
                updateCASLights(dat);
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
                mainform.DebugOutput(lm.ToString());
        }

        private static byte[] pixels = new byte[2592 * 1944 * 3];
        static unsafe public Bitmap ConvertCapturedRawImage(byte[] indata)
        {
            Bitmap bitmap = null;
            
            unsafe
            {
                fixed (byte* ptr = indata)
                {
                    
                    int a = 1;
                    int b = 0;
                    while (a < indata.Length)
                    {
                        pixels[b++] = (byte)(Math.Min(indata[a] * 4, 255));
                        pixels[b++] = (byte)(Math.Min(indata[a] * 4, 255));
                        pixels[b++] = (byte)(Math.Min(indata[a] * 4, 255));
                        a+=2;
                    }

                    fixed (byte* ptr2 = pixels)
                    {
                        IntPtr scan0 = new IntPtr(ptr2);
                        bitmap = new Bitmap(2592, 1944, // Image size
                                            2592*3, // Scan size
                                            PixelFormat.Format24bppRgb, scan0);
                    }
                }
            }
            return bitmap;
        }
  

        
        override public void exUPSStatusMessage(Receiver r, Message m) 
        {
            try
            {
                UPSStatusMessage msg = (UPSStatusMessage)m;
                Status st = msg.Stat;
                if (st == Status.STAT_FAIL)
                {
                    CAS.b_Battery_Com.BackColor = Color.OrangeRed;
                    CAS.b_Battery_Com.Text = "Battery Com";    
                }
                else if (st == Status.STAT_GOOD)
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
                dp.BroadcastLog(this, e.Message, 0);
            }

        }

        override public void exAccelStatusMessage(Receiver r, Message m)       
        {
            try
            {
                AccelStatusMessage msg = (AccelStatusMessage)m;
                lastAccelState = msg.Stat;
                if (lastAccelState == Status.STAT_FAIL)
                {
                    CAS.b_Accel_1.BackColor = Color.OrangeRed;
                }
                else if (lastAccelState == Status.STAT_DISC)
                {
                    CAS.b_Accel_1.BackColor = Color.OrangeRed;
                }
                else if (lastAccelState == Status.STAT_ATCH)
                {
                    CAS.b_Accel_1.BackColor = Color.Yellow;
                }
                else if (lastAccelState == Status.STAT_GOOD)
                {
                    //CAS.b_Accel_1.BackColor = Color.Black;// handled when checking the acceleration difference.
                }
            }

            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.Message, 0);
            }
        }

        override public void exSpatialStatusMessage(Receiver r, Message m)
        {
            try
            {
                SpatialStatusMessage msg = (SpatialStatusMessage)m;
                lastSpatialState = msg.Stat;
                if (lastSpatialState == Status.STAT_FAIL)
                {
                    CAS.b_Accel_2.BackColor = Color.OrangeRed;
                }
                else if (lastSpatialState == Status.STAT_DISC)
                {
                    CAS.b_Accel_2.BackColor = Color.OrangeRed;
                }
                else if (lastSpatialState == Status.STAT_ATCH)
                {
                    CAS.b_Accel_2.BackColor = Color.Yellow;
                }
                else if (lastSpatialState == Status.STAT_GOOD)
                {
                    //CAS.b_Accel_2.BackColor = Color.Black;// handled when checking the acceleration difference
                }
            }

            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.Message, 0);
            }
        }

        override public void exVcommStatusMessage(Receiver r, Message m)
        {
            try
            {

            }
            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.Message, 0);
            }
        }

        override public void exNI6008StatusMessage(Receiver r, Message m)
        {
            try
            {
                NI6008StatusMessage msg = (NI6008StatusMessage)m;
                Status status = msg.Stat;

                if (status == Status.STAT_FAIL)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.OrangeRed;             
                }
                else if (status == Status.STAT_DISC)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.OrangeRed;       
                }
                else if (status == Status.STAT_ATCH)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.OrangeRed;
                }
                else if(status == Status.STAT_GOOD)
                {
                    CAS.b_Accel_Aircraft.BackColor = Color.Black;
                }
            }
            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.Message, 0);
            }
        }

        override public void exAptinaStatusMessage(Receiver r, Message m)
        {
            try
            {
                //todo: convert to a switch
                AptinaStatusMessage msg = (AptinaStatusMessage)m;
                Status status = msg.Stat;
                if (status == Status.STAT_FAIL)
                {
                    switch (msg.WaveLength)
                    {
                        case 405:
                            CAS.b_Camera_405.BackColor = Color.OrangeRed;
                            break;
                        case 485:
                            CAS.b_Camera_485.BackColor = Color.OrangeRed;       
                            break;
                        default:
                            break;
                    }
                }
                else if (status == Status.STAT_ERR)
                {
                    switch (msg.WaveLength)
                    {
                        case (405):
                            CAS.b_Camera_405.BackColor = Color.OrangeRed;
                            break;
                        case(485):
                            CAS.b_Camera_485.BackColor = Color.OrangeRed;
                            break;
                        default:
                            break;
                    }
                }
                else if (status == Status.STAT_GOOD)
                {
                    switch (msg.WaveLength)
                    {
                        case (405):
                            CAS.b_Camera_405.BackColor = Color.Black;
                            break;
                        case (485):
                            CAS.b_Camera_485.BackColor = Color.Black;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (msg.WaveLength)
                    {
                        case (405):
                            CAS.b_Camera_405.BackColor = Color.Salmon;
                            break;
                        case (485):
                            CAS.b_Camera_485.BackColor = Color.Salmon;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.Message, 0);
            }
        }

        override public void exPhidgetsStatusMessage(Receiver r, Message m)
        {
            try
            {
                PhidgetsStatusMessage msg = (PhidgetsStatusMessage)m;
                Status status = msg.Stat;
                if (status == Status.STAT_FAIL)
                {
                    CAS.b_Phidgets_1018.BackColor = Color.OrangeRed;
                }
                else if (status == Status.STAT_DISC)
                {
                    CAS.b_Phidgets_1018.BackColor = Color.OrangeRed;
                }
                else if (status == Status.STAT_ATCH)
                {
                    CAS.b_Phidgets_1018.BackColor = Color.Yellow;
                }
                else if (status == Status.STAT_GOOD)
                {
                    long now = DateTime.Now.Ticks;
                    long dist = now - last1018update;
                    DateTime x = new DateTime(dist);
                    if (x.Second > 1)
                        CAS.b_Phidgets_1018.BackColor = Color.Yellow;
                    else
                        CAS.b_Phidgets_1018.BackColor = Color.Black;
                    last1018update = DateTime.Now.Ticks;
                }
                }
            catch (InvalidOperationException e)
            {
                //the wrong thread operated on something...
                dp.BroadcastLog(this, e.Message, 0);
            }
        }

        public override void exPhidgetsTempStatusMessage(Receiver r, Message m)
        {
            PhidgetsTempStatusMessage msg = (PhidgetsTempStatusMessage)m;
            lastTemperatureState = msg.Stat;
        }

    } /*  class GuiUpdater  */
} /*  namespace gui */
