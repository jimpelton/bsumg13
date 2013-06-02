using System;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using uGCapture;

namespace gui
{
    public partial class BiteCASPanelControl : UserControl
    {

        private Status lastAccelState = Status.STAT_ERR;
        private Status lastSpatialState = Status.STAT_ERR;
        private Status lastTemperatureState = Status.STAT_ERR;
        private long last1018update = 0;

        public ConfigData CurrentConfig { get; set; }


        public delegate void broadcast_log(string s);

        public delegate void broadcast_cmd(CommandStr cmd);

        public broadcast_log BroadcastLog { get; set; }
        public broadcast_cmd BroadcastCmd { get; set; }

        public BiteCASPanelControl()
        {
            InitializeComponent();
            
        }

        public void UpdateContents(DataSet<byte> dat)
        {
            updateCASAccel(dat);
            updateCASPhidgets(dat);
            updateCASBattery(dat);
            updateCASLights(dat);
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
                         b_Heater_High.BackColor = Color.Salmon;
                    }
                    else if (temp > 40)
                    {
                         b_Heater_High.BackColor = Color.Red;
                         b_Heater_Auto_Shutoff.BackColor = Color.Red;
                         BroadcastCmd(CommandStr.CMD_NI_HEATER_OFF);
                    }
                    else if (temp > 38)
                    {
                         b_Heater_High.BackColor = Color.Red;
                    }
                    else
                    {
                         b_Heater_Auto_Shutoff.BackColor = Color.Black;
                         b_Heater_High.BackColor = Color.Black;
                         BroadcastCmd(CommandStr.CMD_NI_HEATER_ON);
                    }

                    if (temp < 1)
                    {
                         b_Heater_Low.BackColor = Color.White;
                    }
                    else if (temp < 20)
                    {
                         b_Heater_Low.BackColor = Color.LightBlue;
                    }
                    else if (temp < 35)
                    {
                         b_Heater_Low.BackColor = Color.Red;
                    }
                    else
                    {
                         b_Heater_Low.BackColor = Color.Black;
                    }
                }
                else
                {
                     b_Heater_Low.BackColor = Color.Red;
                     b_Heater_High.BackColor = Color.Red;
                     b_Heater_Auto_Shutoff.BackColor = Color.Red;
                }
                if (pdats.Length > 10)
                {
                    door = bool.Parse(pdats[9]);
                }
                //TODO Make this change based on the capture status.
                bool running = true;
                if (door)
                {
                     b_Doors.BackColor = Color.Black;
                }
                else
                {
                    if (running)
                         b_Doors.BackColor = Color.Red;
                    else
                         b_Doors.BackColor = Color.Yellow;
                }
            }
            catch (FormatException e)
            {
                BroadcastLog(e.Message);
                //a malformed packet has arrived. Tremble in fear.
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
                    if (bat < CurrentConfig.RedBattery)
                    {
                         b_Battery_Level.BackColor = Color.Red;
                         b_Battery_Com.BackColor = Color.Red;
                    }
                    else if (bat < CurrentConfig.YellowBattery)
                    {
                         b_Battery_Level.BackColor = Color.Yellow;
                         b_Battery_Com.BackColor = Color.Yellow;
                    }
                    else if (bat < CurrentConfig.GreenBattery)
                    {
                         b_Battery_Level.BackColor = Color.Green;
                         b_Battery_Com.BackColor = Color.Green;
                    }
                    else
                    {
                         b_Battery_Level.BackColor = Color.Black;
                         b_Battery_Com.BackColor = Color.Black;
                    }

                     b_Battery_Level.Text = "Battery Level (" + UPSdats[4] + ")";
                    if (int.Parse(UPSdats[6]) > 0)
                    {
                        string batstring = BatteryStatusStrings.GetBatteryStatusStr(int.Parse(UPSdats[6]));
                        if (batstring != null)
                             b_Battery_Com.Text = batstring;
                    }
                    else
                    {
                         b_Battery_Com.Text = "Battery Com";
                    }

                }
                catch (FormatException e)
                {
                    BroadcastLog("CAS Battery update handler has encountered a malformed packet");
                    //a malformed packet has arrived. Tremble in fear.
                }
            }
            else
            {
                b_Battery_Com.Text = "Battery Com";
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
                     b_Light_1.BackColor = Color.Red;
                }
                else if (l1 < 200)
                {
                     b_Light_1.BackColor = Color.Yellow;
                }
                else if (l1 < 300)
                {
                     b_Light_1.BackColor = Color.Green;
                }
                else
                {
                     b_Light_1.BackColor = Color.Black;
                }

                if (l2 < 10)
                {
                     b_Light_2.BackColor = Color.Red;
                }
                else if (l2 < 200)
                {
                     b_Light_2.BackColor = Color.Yellow;
                }
                else if (l2 < 300)
                {
                     b_Light_2.BackColor = Color.Green;
                }
                else
                {
                     b_Light_2.BackColor = Color.Black;
                }


            }
            catch (FormatException e)
            {
                BroadcastLog("CAS Light update handler has encountered a malformed packet");
                //a malformed packet has arrived. Tremble in fear.
            }

        }
        private void updateCASAccel(DataSet<byte> dat)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            try
            {
                if (lastAccelState == Status.STAT_GOOD)
                     b_Accel_1.BackColor = Color.Black;
                if (lastSpatialState == Status.STAT_GOOD)
                     b_Accel_2.BackColor = Color.Black;

                //get the difference between accel and spacials acceleration vector magnitudes.
                double x1 = 0, x2 = 0, x3 = 0, y1 = 0, y2 = 0, y3 = 0, z1 = 0, z2 = 0, z3 = 0;
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
                    //Guimain.DataFrames.Add(d);

                }
                else
                {
                     b_Accel_1.BackColor = Color.Red;
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
                     b_Accel_1.BackColor = Color.Red;
                }

                if (AAcceldats.Length > 5)
                {
                    //convert these from voltages to Gs.... 
                    x3 = double.Parse(AAcceldats[3 + 0]);//3 is the zeroth element. x is on 0
                    y3 = double.Parse(AAcceldats[3 + 4]);//y is on 4
                    z3 = double.Parse(AAcceldats[3 + 1]);//z is on 1
                }

                if (Math.Abs(x3 - y3) < 0.01 && Math.Abs(y3 - z3) < 0.01 && Math.Abs(z3 - x3) < 0.01)
                {
                    if ( b_Accel_Aircraft.BackColor != Color.Red)
                         b_Accel_Aircraft.BackColor = Color.Yellow;
                }

                double m1 = Math.Sqrt((x1 * x1) + (y1 * y1) + (z1 * z1));
                double m2 = Math.Sqrt((x2 * x2) + (y2 * y2) + (z2 * z2));
                double dif = Math.Abs(m1 - m2);

                if (dif > 0.5)
                {
                    if ( b_Accel_1.BackColor != Color.Red && lastAccelState == Status.STAT_GOOD)
                         b_Accel_1.BackColor = Color.Green;
                    if ( b_Accel_2.BackColor != Color.Red && lastSpatialState == Status.STAT_GOOD)
                         b_Accel_2.BackColor = Color.Green;
                }
            }
            catch (FormatException e)
            {
                BroadcastLog("Cas Accel update handler has encountered a malformed packet");
                //a malformed packet has arrived. Tremble in fear.
            }
        }



      
    }
}
