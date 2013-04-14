using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gui
{
    public class DataPoint
    {
        public long timestamp;
        public long[, ,] WellIntensities;
        public bool[] phidgetsdigitalInputs;
        public bool[] phidgetsdigitalOutputs;
        public int[] phidgetsanalogInputs;
        private double phidgetTemperature_ProbeTemp;
        private double phidgetTemperature_AmbientTemp;
        public double[] accel1rawacceleration;
        public double[] accel1acceleration;
        public double[] accel1vibration;
        public double[] accel2rawacceleration;
        public double[] accel2acceleration;
        public double[] accel2vibration;
        public double[] NIanaloginputs;
        public int accel1state = 0;
        public int accel2state = 0;
        public int phidgets888state = 0;
        public int phidgetstempstate = 0;
        public int UPSstate = 0;
        public int VCommstate = 0;
        public uGCapture.Buffer<byte> image405 = null;
        public uGCapture.Buffer<byte> image485 = null;

        public DataPoint()
        {           
            WellIntensities = new long[2, 16, 12];
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
    }
}
