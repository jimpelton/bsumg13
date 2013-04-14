using System;
using System.Collections.Generic;
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
        public GuiUpdater(Form1 f,GuiMain m)
        {         
            mainform = f;
            graph1data = new Series("Points");
            Guimain = m;
            this.Receiving = true;
            dp.Register(this, "GuiUpdater");
        }

        public void UpdateGUI(object sender, EventArgs e)
        {
            try
            {
                //while (this.msgs.Count > 0)
                //    msgs.Dequeue().execute(this);
                ExecuteMessageQueue();
            }
            catch (NullReferenceException nel)/// temp fix for monday's test. 5 minutes out.
            {
                Console.WriteLine("UpdateGUI in GuiUpdater.cs threw a null pointer exception at msgs.Dequeue().execute(this)");
 
            }
            mainform.chart1.Series.Clear();
            List<DataPoint> frames = Guimain.getDataPoints();
            if (frames.Count > 0)
            {
                mainform.chart1.Series.Add("Graph1");
                mainform.chart1.Series.Add("Graph2");
                mainform.chart1.Series.Add("Graph3");
                foreach (DataPoint p in frames)
                {
                    mainform.chart1.Series["Graph1"].ChartType = SeriesChartType.FastLine;
                    mainform.chart1.Series["Graph1"].Points.AddY(p.phidgetTemperature_AmbientTemp);
                    mainform.chart1.Series["Graph1"].ChartArea = "ChartArea1";
                    mainform.chart1.Series["Graph2"].ChartType = SeriesChartType.FastLine;
                    mainform.chart1.Series["Graph2"].Points.AddY(p.phidgetTemperature_ProbeTemp);
                    mainform.chart1.Series["Graph2"].ChartArea = "ChartArea2";
                    mainform.chart1.Series["Graph3"].ChartType = SeriesChartType.FastLine;
                    mainform.chart1.Series["Graph3"].Points.AddY(p.phidgetsanalogInputs[0]);
                    mainform.chart1.Series["Graph3"].ChartArea = "ChartArea3";
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

        public override void exData(Receiver r, Message m)
        {
            DataPoint dat = new DataPoint();
            dat.image405 = ((DataMessage) m).image405;
            dat.image485 = ((DataMessage) m).image485;

            dat.NIanaloginputs = ((DataMessage) m).NIanaloginputs;
            dat.UPSstate = ((DataMessage)m).UPSstate;
            dat.VCommstate = ((DataMessage)m).VCommstate;
            dat.WellIntensities = ((DataMessage)m).WellIntensities;
            dat.accel1acceleration = ((DataMessage)m).accel1acceleration;
            dat.accel1rawacceleration = ((DataMessage)m).accel1rawacceleration;
            dat.accel1state = ((DataMessage)m).accel1state;
            dat.accel1vibration = ((DataMessage)m).accel1vibration;
            dat.accel2acceleration = ((DataMessage)m).accel2acceleration;
            dat.accel2rawacceleration = ((DataMessage)m).accel2rawacceleration;
            dat.accel2state = ((DataMessage)m).accel2state;
            dat.accel2vibration = ((DataMessage)m).accel2vibration;
            dat.phidgets888state = ((DataMessage)m).phidgets888state;
            dat.phidgetsanalogInputs = ((DataMessage)m).phidgetsanalogInputs;
            dat.phidgetsdigitalInputs = ((DataMessage)m).phidgetsdigitalInputs;
            dat.phidgetsdigitalOutputs = ((DataMessage)m).phidgetsdigitalOutputs;
            dat.phidgetstempstate = ((DataMessage)m).phidgetstempstate;
            dat.phidgetTemperature_AmbientTemp = ((DataMessage) m).phidgetTemperature_AmbientTemp;
            dat.phidgetTemperature_ProbeTemp = ((DataMessage) m).phidgetTemperature_ProbeTemp;
            dat.timestamp = ((DataMessage)m).timestamp;   
            
            Guimain.insertDataPoint(dat);
        }
        public override void exDataRequest(Receiver r, Message m)
        {
            int test = 0;
        }
    }
}
