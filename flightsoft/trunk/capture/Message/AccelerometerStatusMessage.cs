using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class AccelerometerStatusMessage: StatusMessage
    {
        public double accel = 0;
        public AccelerometerStatusMessage(Receiver s, Status nstate, ErrStr mes = 0, double accl=0)
            : base(s, nstate, mes)
        {
            accel = accl;
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exAccelerometerStatusMessage(r, this);
        }
    }
}
