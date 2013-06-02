using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    class AccelerometerStatusMessage: StatusMessage
    {
        public float accel = 0;
        public AccelerometerStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exAccelerometerStatusMessage(r, this);
        }
    }
}
