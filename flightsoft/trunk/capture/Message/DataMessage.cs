using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class DataMessage : Message
    {

        public long timestamp;
        public bool[] phidgetsdigitalInputs;
        public bool[] phidgetsdigitalOutputs;
        public int[] phidgetsanalogInputs;
        public double phidgetTemperature_ProbeTemp;
        public double phidgetTemperature_AmbientTemp;
        public double[] accel1rawacceleration;
        public double[] accel1acceleration;
        public double[] accel1vibration;
        public double[] accel2rawacceleration;
        public double[] accel2acceleration;
        public double[] accel2vibration;
        public double[] NIanaloginputs;

        public double vcommHumidity=0;
        public double vcommTemperature1=0;
        public double vcommTemperature2=0;
        public double vcommTemperature3=0;
        public double vcommPressure=0;
        public double vcommIllumination=0;

        public int accel1state=0;
        public int accel2state=0;
        public int phidgets888state=0;
        public int phidgetstempstate=0;
        public int UPSstate=0;
        public int VCommstate=0;
        public Buffer<byte> image405 = null;
        public Buffer<byte> image485 = null;
        public long[] WellIntensities405;
        public long[] WellIntensities485;


        public DataMessage(Receiver s)
            : base(s)
        {
	    WellIntensities405 = new long[192];
	    WellIntensities485 = new long[192];
            phidgetsdigitalInputs = new bool[8];
            phidgetsdigitalOutputs = new bool[8];
            phidgetsanalogInputs = new int[8];
            accel1rawacceleration = new double[3];
            accel1vibration = new double[3];
            accel1acceleration = new double[3];
            accel2rawacceleration = new double[3];
            accel2vibration = new double[3];
            accel2acceleration = new double[3];
            NIanaloginputs = new double[6];
            timestamp = 0;
        }

        public override void execute(Receiver r)
        {
            r.exDataMessage(r, this);
        }
    }
}
